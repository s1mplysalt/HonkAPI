using System;
using UnityEngine;

namespace HonkAPI.World
{
    public static class WorldAPI
    {
        public static bool IsGameActive
        {
            get { return BaseManager.gameWorldActive; }
        }

        public static string CurrentArea
        {
            get
            {
                if (GameManager.instance != null && GameManager.instance.areaTracker != null)
                {
                    return AreaTracker.mostRecentArea.ToString();
                }
                return "Unknown";
            }
        }

        public static float TimePlayed
        {
            get
            {
                if (GameManager.instance != null)
                {
                    return GameManager.instance.totalPlayTime;
                }
                return Time.time;
            }
        }

        public static Camera MainCamera
        {
            get
            {
                if (GameManager.instance != null && GameManager.instance.mainCamera != null)
                {
                    return GameManager.instance.mainCamera;
                }
                return Camera.main;
            }
        }

        public static CameraManager CameraManager
        {
            get
            {
                if (GameManager.instance != null)
                {
                    return GameManager.instance.cameraManager;
                }
                return UnityEngine.Object.FindObjectOfType<CameraManager>();
            }
        }

        public static void SetCameraDistance(float distance)
        {
            CameraManager cm = CameraManager;
            if (cm == null) return;
            cm.defaultDistanceStandard = distance;
            cm.defaultDistanceZoomedIn = distance * 0.65f;
            cm.defaultDistanceZoom = distance * 2.2f;
        }

        public static void SetCameraTarget(Transform target)
        {
            CameraManager cm = CameraManager;
            if (cm == null || target == null) return;
            cm.nonHumanFallbackTarget = target;
        }

        public static void ResetCamera()
        {
            CameraManager cm = CameraManager;
            if (cm == null) return;
            cm.defaultDistanceStandard = 22.5f;
            cm.defaultDistanceZoomedIn = 15f;
            cm.defaultDistanceZoom = 50f;
            cm.nonHumanFallbackTarget = null;
        }

        public static void SetSlowMotion(float timeScale)
        {
            Time.timeScale = Mathf.Clamp(timeScale, 0.05f, 5f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        public static void ResetTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        public static void PauseGame(bool pause)
        {
            if (InGameUI.instance != null)
            {
                InGameUI.SetPaused(pause);
            }
            else
            {
                Time.timeScale = pause ? 0f : 1f;
            }
        }

        public static bool IsPaused
        {
            get { return Time.timeScale <= 0f; }
        }

        public static GoalListArea CurrentAreaId
        {
            get { return AreaTracker.mostRecentArea; }
        }

        /// <summary>Height of the ground below a point, found by raycasting down. Returns false when nothing is hit.</summary>
        public static bool TryGetGroundHeight(Vector3 position, out float height, float maxDistance = 200f)
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hit, maxDistance + 50f))
            {
                height = hit.point.y;
                return true;
            }
            height = position.y;
            return false;
        }

        /// <summary>Nearest walkable NavMesh point within a radius, as humans would be able to reach.</summary>
        public static bool TryGetNavMeshPoint(Vector3 position, float radius, out Vector3 result)
        {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(position, out hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            result = position;
            return false;
        }

        /// <summary>Spawn position the game uses for geese in an area, or null when it has none.</summary>
        public static Vector3? GetAreaSpawn(GoalListArea area)
        {
            if (GameCollections.Instance == null || GameCollections.Instance.gooseSpawns == null) return null;
            if (!GameCollections.Instance.gooseSpawns.ContainsKey(area)) return null;
            GooseSpawn sp = GameCollections.Instance.gooseSpawns[area];
            if (sp == null) return null;
            return sp.pose != null ? sp.pose.transform.position : sp.transform.position;
        }
    }
}
