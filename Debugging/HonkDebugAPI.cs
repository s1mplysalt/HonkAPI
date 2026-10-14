using System;
using UnityEngine;

namespace HonkAPI.Debugging
{
    public static class HonkDebugAPI
    {
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1.0f)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }

        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 1.0f)
        {
            UnityEngine.Debug.DrawRay(start, dir, color, duration);
        }

        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[HonkAPI] " + message);
        }

        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning("[HonkAPI] " + message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError("[HonkAPI] " + message);
        }
    }
}
