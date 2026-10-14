using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.GooseApi;
using HonkAPI.World;

namespace HonkAPI.UI
{
    public class HonkHUDItem
    {
        public string Id;
        public Func<string> TextGetter;
        public TextAnchor Anchor;

        public HonkHUDItem(string id, Func<string> getter, TextAnchor anchor)
        {
            Id = id;
            TextGetter = getter;
            Anchor = anchor;
        }
    }

    public static class HonkHUD
    {
        private static readonly Dictionary<string, HonkHUDItem> items = new Dictionary<string, HonkHUDItem>();
        private static bool showFps = false;
        private static bool showCoords = false;
        private static bool showSpeed = false;
        private static bool showCrosshair = false;
        private static float fpsTimer = 0f;
        private static int fpsCount = 0;
        private static int currentFps = 60;

        public static void AddLabel(string id, Func<string> textGetter, TextAnchor anchor = TextAnchor.UpperLeft)
        {
            items[id] = new HonkHUDItem(id, textGetter, anchor);
        }

        public static void RemoveLabel(string id)
        {
            if (items.ContainsKey(id))
            {
                items.Remove(id);
            }
        }

        public static void Clear()
        {
            items.Clear();
        }

        public static void SetShowFPS(bool show)
        {
            showFps = show;
            if (show)
            {
                AddLabel("__honk_fps", GetFpsString, TextAnchor.UpperRight);
            }
            else
            {
                RemoveLabel("__honk_fps");
            }
        }

        public static void SetShowCoordinates(bool show)
        {
            showCoords = show;
            if (show)
            {
                AddLabel("__honk_coords", GetCoordsString, TextAnchor.UpperLeft);
            }
            else
            {
                RemoveLabel("__honk_coords");
            }
        }

        public static void SetShowSpeedometer(bool show)
        {
            showSpeed = show;
            if (show)
            {
                AddLabel("__honk_speed", GetSpeedString, TextAnchor.LowerLeft);
            }
            else
            {
                RemoveLabel("__honk_speed");
            }
        }

        public static void SetShowCrosshair(bool show)
        {
            showCrosshair = show;
        }

        public static void Update()
        {
            if (showFps)
            {
                fpsTimer += Time.unscaledDeltaTime;
                fpsCount++;
                if (fpsTimer >= 0.5f)
                {
                    currentFps = (int)(fpsCount / fpsTimer);
                    fpsTimer = 0f;
                    fpsCount = 0;
                }
            }
        }

        private static GUIStyle cachedHudStyle;

        private static string GetFpsString()
        {
            return string.Format("FPS: {0}", currentFps);
        }

        private static string GetCoordsString()
        {
            try
            {
                if (GooseAPI.Current != null && GooseAPI.Current.transform != null)
                {
                    Vector3 p = GooseAPI.Current.transform.position;
                    return string.Format("Goose Pos: ({0:F1}, {1:F1}, {2:F1}) | Area: {3}", p.x, p.y, p.z, WorldAPI.CurrentArea);
                }
            }
            catch (Exception)
            {
            }
            return "Goose: Inactive";
        }

        private static string GetSpeedString()
        {
            try
            {
                if (GooseAPI.Current != null && GooseAPI.Current.rb != null)
                {
                    float spd = GooseAPI.Current.rb.velocity.magnitude;
                    return string.Format("Speed: {0:F1} m/s", spd);
                }
            }
            catch (Exception)
            {
            }
            return "Speed: 0.0 m/s";
        }

        public static void Render()
        {
            if (items.Count == 0 && !showCrosshair) return;

            if (cachedHudStyle == null)
            {
                cachedHudStyle = new GUIStyle(GUI.skin.label);
                cachedHudStyle.fontSize = 12;
                cachedHudStyle.fontStyle = FontStyle.Bold;
                cachedHudStyle.normal.textColor = Color.white;
            }
            GUIStyle hudStyle = cachedHudStyle;

            float pad = 12f;
            float topY = pad;
            float bottomY = Screen.height - 30f;

            foreach (KeyValuePair<string, HonkHUDItem> kvp in items)
            {
                HonkHUDItem item = kvp.Value;
                if (item == null || item.TextGetter == null) continue;

                string text = item.TextGetter();
                if (string.IsNullOrEmpty(text)) continue;

                GUIContent gc = new GUIContent(text);
                Vector2 sz = hudStyle.CalcSize(gc);
                Rect r;

                switch (item.Anchor)
                {
                    case TextAnchor.UpperRight:
                        r = new Rect(Screen.width - sz.x - pad - 12f, topY, sz.x + 12f, sz.y + 4f);
                        topY += sz.y + 6f;
                        break;
                    case TextAnchor.LowerLeft:
                        r = new Rect(pad, bottomY, sz.x + 12f, sz.y + 4f);
                        bottomY -= (sz.y + 6f);
                        break;
                    case TextAnchor.LowerRight:
                        r = new Rect(Screen.width - sz.x - pad - 12f, bottomY, sz.x + 12f, sz.y + 4f);
                        bottomY -= (sz.y + 6f);
                        break;
                    default:
                        r = new Rect(pad, topY, sz.x + 12f, sz.y + 4f);
                        topY += sz.y + 6f;
                        break;
                }

                GUI.DrawTexture(r, HonkUITheme.GetColoredTexture(new Color(0f, 0f, 0f, 0.65f)));
                GUI.Label(new Rect(r.x + 6f, r.y + 2f, sz.x, sz.y), text, hudStyle);
            }

            if (showCrosshair)
            {
                float cx = Screen.width / 2f;
                float cy = Screen.height / 2f;
                float size = 8f;
                Color prev = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.8f);
                GUI.DrawTexture(new Rect(cx - size, cy - 1f, size * 2f, 2f), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(cx - 1f, cy - size, 2f, size * 2f), Texture2D.whiteTexture);
                GUI.color = prev;
            }
        }
    }
}
