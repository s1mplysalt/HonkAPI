using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.Weather
{
    /// <summary>Lighting and atmosphere: ambient light, fog, sun, sky tint, time of day and global light scaling. Every change can be undone with ResetAtmosphere.</summary>
    public static class WeatherAPI
    {
        private static bool captured;
        private static Color origAmbient, origFogColor, origSkyTint;
        private static float origAmbientIntensity, origFogDensity, origFogStart, origFogEnd, origSunIntensity;
        private static bool origFog;
        private static FogMode origFogMode;
        private static Color origSunColor;
        private static Quaternion origSunRotation;
        private static Material skyMat;
        private static string skyTintProp;
        private static readonly Dictionary<Light, float> originalIntensities = new Dictionary<Light, float>();

        public static Light Sun
        {
            get
            {
                if (RenderSettings.sun != null) return RenderSettings.sun;
                foreach (Light l in UnityEngine.Object.FindObjectsOfType<Light>())
                {
                    if (l != null && l.type == LightType.Directional) return l;
                }
                return null;
            }
        }

        private static void Capture()
        {
            if (captured) return;
            captured = true;
            origAmbient = RenderSettings.ambientLight;
            origAmbientIntensity = RenderSettings.ambientIntensity;
            origFog = RenderSettings.fog;
            origFogColor = RenderSettings.fogColor;
            origFogDensity = RenderSettings.fogDensity;
            origFogStart = RenderSettings.fogStartDistance;
            origFogEnd = RenderSettings.fogEndDistance;
            origFogMode = RenderSettings.fogMode;
            Light sun = Sun;
            if (sun != null)
            {
                origSunColor = sun.color;
                origSunIntensity = sun.intensity;
                origSunRotation = sun.transform.rotation;
            }
            skyMat = RenderSettings.skybox;
            if (skyMat != null)
            {
                string[] names = new string[] { "_Tint", "_SkyTint", "_Color" };
                foreach (string n in names)
                {
                    if (skyMat.HasProperty(n)) { skyTintProp = n; origSkyTint = skyMat.GetColor(n); break; }
                }
            }
        }

        public static void SetAmbientLight(Color color)
        {
            Capture();
            RenderSettings.ambientLight = color;
        }

        public static void SetAmbientIntensity(float intensity)
        {
            Capture();
            RenderSettings.ambientIntensity = Mathf.Max(0f, intensity);
        }

        public static void SetFog(bool enabled, Color color, float density)
        {
            Capture();
            RenderSettings.fog = enabled;
            RenderSettings.fogColor = color;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = density;
        }

        public static void SetLinearFog(Color color, float start, float end)
        {
            Capture();
            RenderSettings.fog = true;
            RenderSettings.fogColor = color;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = start;
            RenderSettings.fogEndDistance = end;
        }

        public static void SetSun(Color color, float intensity)
        {
            Capture();
            Light sun = Sun;
            if (sun == null) return;
            sun.color = color;
            sun.intensity = Mathf.Max(0f, intensity);
        }

        public static void SetSunRotation(Vector3 euler)
        {
            Capture();
            Light sun = Sun;
            if (sun != null) sun.transform.rotation = Quaternion.Euler(euler);
        }

        /// <summary>Tints the skybox when its material exposes a tint property. Returns false otherwise.</summary>
        public static bool SetSkyTint(Color color)
        {
            Capture();
            if (skyMat == null || skyTintProp == null) return false;
            skyMat.SetColor(skyTintProp, color);
            DynamicGI.UpdateEnvironment();
            return true;
        }

        /// <summary>Multiplies the intensity of every light in the scene relative to its original value.</summary>
        public static void ScaleAllLights(float multiplier)
        {
            foreach (Light l in UnityEngine.Object.FindObjectsOfType<Light>())
            {
                if (l == null) continue;
                if (!originalIntensities.ContainsKey(l)) originalIntensities[l] = l.intensity;
                l.intensity = originalIntensities[l] * Mathf.Max(0f, multiplier);
            }
        }

        /// <summary>
        /// Sets a time of day from 0 to 24 hours: moves and colors the sun, and adjusts ambient light.
        /// Noon is 12.
        /// </summary>
        public static void SetTimeOfDay(float hours)
        {
            Capture();
            float h = Mathf.Repeat(hours, 24f);
            float angle = (h / 24f) * 360f - 90f;
            Light sun = Sun;
            float daylight = Mathf.Clamp01(Mathf.Sin((h - 6f) / 12f * Mathf.PI));
            float warm = 1f - Mathf.Abs(Mathf.Clamp01(Mathf.Sin((h - 6f) / 12f * Mathf.PI)) * 2f - 1f);
            if (sun != null)
            {
                Vector3 e = sun.transform.eulerAngles;
                sun.transform.rotation = Quaternion.Euler(angle, e.y, 0f);
                Color day = Color.Lerp(origSunColor, new Color(1f, 0.55f, 0.3f), warm * 0.8f);
                sun.color = Color.Lerp(new Color(0.2f, 0.25f, 0.5f), day, daylight);
                sun.intensity = Mathf.Lerp(0.08f, origSunIntensity, daylight);
            }
            RenderSettings.ambientLight = Color.Lerp(new Color(0.07f, 0.08f, 0.16f), origAmbient, daylight);
        }

        public static void SetNightMode()
        {
            SetTimeOfDay(0f);
            SetFog(true, new Color(0.08f, 0.08f, 0.18f), 0.015f);
        }

        public static void SetSunset()
        {
            SetTimeOfDay(18.3f);
            SetFog(true, new Color(0.9f, 0.5f, 0.35f), 0.006f);
        }

        /// <summary>Restores every lighting value this API has changed to what the game set up.</summary>
        public static void ResetAtmosphere()
        {
            if (!captured) return;
            RenderSettings.ambientLight = origAmbient;
            RenderSettings.ambientIntensity = origAmbientIntensity;
            RenderSettings.fog = origFog;
            RenderSettings.fogColor = origFogColor;
            RenderSettings.fogDensity = origFogDensity;
            RenderSettings.fogStartDistance = origFogStart;
            RenderSettings.fogEndDistance = origFogEnd;
            RenderSettings.fogMode = origFogMode;
            Light sun = Sun;
            if (sun != null)
            {
                sun.color = origSunColor;
                sun.intensity = origSunIntensity;
                sun.transform.rotation = origSunRotation;
            }
            if (skyMat != null && skyTintProp != null) skyMat.SetColor(skyTintProp, origSkyTint);
            foreach (KeyValuePair<Light, float> kv in originalIntensities)
            {
                if (kv.Key != null) kv.Key.intensity = kv.Value;
            }
            originalIntensities.Clear();
            DynamicGI.UpdateEnvironment();
        }
    }
}
