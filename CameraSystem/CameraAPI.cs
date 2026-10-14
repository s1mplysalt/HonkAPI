using System;
using UnityEngine;
using HonkAPI.Events;

namespace HonkAPI.CameraSystem
{
    /// <summary>Control over the game's CameraManager: distance, angle, offset, following, damping, shake and field of view.</summary>
    public static class CameraAPI
    {
        private static bool captured;
        private static float origStandard, origZoomedIn, origZoom, origDamp, origAngleDamp, origFov;
        private static Vector3 origOffset, origOffsetZoomedIn;
        private static bool origUseBounds, origDontDamp;

        private static float shakeEnd;
        private static float shakeMagnitude;
        private static bool shaking;
        private static Vector3 shakeBaseOffset;
        private static bool updateHooked;

        public static CameraManager Manager
        {
            get
            {
                if (GameManager.instance != null && GameManager.instance.cameraManager != null)
                    return GameManager.instance.cameraManager;
                return UnityEngine.Object.FindObjectOfType<CameraManager>();
            }
        }

        public static Camera Current
        {
            get
            {
                CameraManager cm = Manager;
                if (cm != null && cm.myCamera != null) return cm.myCamera;
                return Camera.main;
            }
        }

        private static CameraManager Capture()
        {
            CameraManager cm = Manager;
            if (cm == null) return null;
            if (!captured)
            {
                captured = true;
                origStandard = cm.defaultDistanceStandard;
                origZoomedIn = cm.defaultDistanceZoomedIn;
                origZoom = cm.defaultDistanceZoom;
                origDamp = cm.dampTime;
                origAngleDamp = cm.angleDampTime;
                origOffset = cm.offset;
                origOffsetZoomedIn = cm.offsetZoomedIn;
                origUseBounds = cm.useBounds;
                origDontDamp = cm.dontDamp;
                Camera c = Current;
                origFov = c != null ? c.fieldOfView : 60f;
            }
            return cm;
        }

        public static void SetFieldOfView(float fov)
        {
            Capture();
            Camera c = Current;
            if (c != null) c.fieldOfView = fov;
        }

        public static void ResetFieldOfView()
        {
            if (!captured) return;
            Camera c = Current;
            if (c != null) c.fieldOfView = origFov;
        }

        /// <summary>Sets how far the camera sits from the action at normal, zoomed-in and zoomed-out levels.</summary>
        public static void SetDistances(float standard, float zoomedIn, float zoomedOut)
        {
            CameraManager cm = Capture();
            if (cm == null) return;
            cm.defaultDistanceStandard = standard;
            cm.defaultDistanceZoomedIn = zoomedIn;
            cm.defaultDistanceZoom = zoomedOut;
        }

        public static void SetOffset(Vector3 offset, Vector3 offsetZoomedIn)
        {
            CameraManager cm = Capture();
            if (cm == null) return;
            cm.offset = offset;
            cm.offsetZoomedIn = offsetZoomedIn;
            shakeBaseOffset = offset;
        }

        public static void SetRotation(Vector3 eulerAngles)
        {
            CameraManager cm = Capture();
            if (cm != null) cm.SetRotation(eulerAngles);
        }

        public static void SetDamping(float positionDamp, float angleDamp)
        {
            CameraManager cm = Capture();
            if (cm == null) return;
            cm.dampTime = Mathf.Max(0f, positionDamp);
            cm.angleDampTime = Mathf.Max(0f, angleDamp);
        }

        public static void SetInstant(bool instant)
        {
            CameraManager cm = Capture();
            if (cm != null) cm.dontDamp = instant;
        }

        public static void SetUseBounds(bool useBounds)
        {
            CameraManager cm = Capture();
            if (cm != null) cm.useBounds = useBounds;
        }

        /// <summary>Makes the camera follow a transform when no humans or geese are being tracked.</summary>
        public static void Follow(Transform target)
        {
            CameraManager cm = Capture();
            if (cm != null) cm.nonHumanFallbackTarget = target;
        }

        public static void SetFrozen(bool frozen)
        {
            CameraManager cm = Capture();
            if (cm == null) return;
            cm.updatePosition = !frozen;
            cm.updateRotation = !frozen;
        }

        public static void CatchUp()
        {
            CameraManager cm = Manager;
            if (cm != null) cm.CatchUp();
        }

        public static void SetBlur(bool blurred)
        {
            CameraManager.SetBlurred(blurred);
        }

        /// <summary>Shakes the camera by jittering its offset for the given duration.</summary>
        public static void Shake(float duration, float magnitude)
        {
            CameraManager cm = Capture();
            if (cm == null) return;
            if (!shaking) shakeBaseOffset = cm.offset;
            shaking = true;
            shakeEnd = Time.time + duration;
            shakeMagnitude = magnitude;
            if (!updateHooked)
            {
                HonkEvents.OnUpdate += UpdateShake;
                updateHooked = true;
            }
        }

        private static void UpdateShake()
        {
            if (!shaking) return;
            CameraManager cm = Manager;
            if (cm == null) { shaking = false; return; }
            if (Time.time >= shakeEnd)
            {
                cm.offset = shakeBaseOffset;
                shaking = false;
                return;
            }
            float fade = Mathf.Clamp01((shakeEnd - Time.time) / 0.5f);
            cm.offset = shakeBaseOffset + UnityEngine.Random.insideUnitSphere * shakeMagnitude * fade;
        }

        /// <summary>Restores every camera value this API has changed.</summary>
        public static void Reset()
        {
            if (!captured) return;
            CameraManager cm = Manager;
            shaking = false;
            if (cm != null)
            {
                cm.defaultDistanceStandard = origStandard;
                cm.defaultDistanceZoomedIn = origZoomedIn;
                cm.defaultDistanceZoom = origZoom;
                cm.dampTime = origDamp;
                cm.angleDampTime = origAngleDamp;
                cm.offset = origOffset;
                cm.offsetZoomedIn = origOffsetZoomedIn;
                cm.useBounds = origUseBounds;
                cm.dontDamp = origDontDamp;
                cm.updatePosition = true;
                cm.updateRotation = true;
                cm.nonHumanFallbackTarget = null;
            }
            ResetFieldOfView();
        }
    }
}
