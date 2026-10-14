using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.Events;

namespace HonkAPI.InputSystem
{
    public static class InputAPI
    {
        private static readonly Dictionary<KeyCode, Action> hotkeys = new Dictionary<KeyCode, Action>();
        private static bool registeredUpdate = false;

        public static bool GetKeyDown(KeyCode key)
        {
            return UnityEngine.Input.GetKeyDown(key);
        }

        public static bool GetKey(KeyCode key)
        {
            return UnityEngine.Input.GetKey(key);
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return UnityEngine.Input.GetKeyUp(key);
        }

        /// <summary>True on the frame the honk button goes down for the given goose (default player 0), using the game's own bindings.</summary>
        public static bool IsHonkPressed(int playerNumber = 0)
        {
            Goose g = HonkAPI.GooseApi.GooseAPI.Get(playerNumber);
            if (g != null && g.player != null)
            {
                return g.player.GetButtonDown(1);
            }
            return UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetMouseButtonDown(0);
        }

        public static bool IsSprintPressed()
        {
            return UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
        }

        public static bool IsDuckPressed()
        {
            return UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.C);
        }

        /// <summary>Raw Rewired access for a goose: button held, pressed, released and analog axis by action id.</summary>
        public static bool GetButton(int playerNumber, int actionId)
        {
            Goose g = HonkAPI.GooseApi.GooseAPI.Get(playerNumber);
            return g != null && g.player != null && g.player.GetButton(actionId);
        }

        public static bool GetButtonDown(int playerNumber, int actionId)
        {
            Goose g = HonkAPI.GooseApi.GooseAPI.Get(playerNumber);
            return g != null && g.player != null && g.player.GetButtonDown(actionId);
        }

        public static bool GetButtonUp(int playerNumber, int actionId)
        {
            Goose g = HonkAPI.GooseApi.GooseAPI.Get(playerNumber);
            return g != null && g.player != null && g.player.GetButtonUp(actionId);
        }

        public static float GetAxis(int playerNumber, int actionId)
        {
            Goose g = HonkAPI.GooseApi.GooseAPI.Get(playerNumber);
            return (g != null && g.player != null) ? g.player.GetAxis(actionId) : 0f;
        }

        /// <summary>Mouse position projected onto the ground plane (y = planeY), or null when there is no camera.</summary>
        public static Vector3? GetMouseGroundPoint(float planeY = 0f)
        {
            Camera cam = HonkAPI.CameraSystem.CameraAPI.Current;
            if (cam == null) return null;
            Ray ray = cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));
            float enter;
            if (plane.Raycast(ray, out enter)) return ray.GetPoint(enter);
            return null;
        }

        private static readonly List<KeyValuePair<KeyValuePair<KeyCode, KeyCode>, Action>> chords = new List<KeyValuePair<KeyValuePair<KeyCode, KeyCode>, Action>>();

        /// <summary>Runs the callback when the key goes down while the modifier is held.</summary>
        public static void RegisterChord(KeyCode modifier, KeyCode key, Action callback)
        {
            if (!registeredUpdate)
            {
                HonkEvents.OnUpdate += CheckHotkeys;
                registeredUpdate = true;
            }
            chords.Add(new KeyValuePair<KeyValuePair<KeyCode, KeyCode>, Action>(new KeyValuePair<KeyCode, KeyCode>(modifier, key), callback));
        }

        public static void UnregisterAll()
        {
            hotkeys.Clear();
            chords.Clear();
        }

        public static void RegisterHotkey(KeyCode key, Action callback)
        {
            if (!registeredUpdate)
            {
                HonkEvents.OnUpdate += CheckHotkeys;
                registeredUpdate = true;
            }
            hotkeys[key] = callback;
        }

        public static void UnregisterHotkey(KeyCode key)
        {
            if (hotkeys.ContainsKey(key))
            {
                hotkeys.Remove(key);
            }
        }

        private static void CheckHotkeys()
        {
            if (hotkeys.Count > 0)
            {
                List<KeyValuePair<KeyCode, Action>> snapshot = new List<KeyValuePair<KeyCode, Action>>(hotkeys);
                foreach (KeyValuePair<KeyCode, Action> kvp in snapshot)
                {
                    if (UnityEngine.Input.GetKeyDown(kvp.Key) && kvp.Value != null)
                    {
                        kvp.Value();
                    }
                }
            }
            if (chords.Count > 0)
            {
                List<KeyValuePair<KeyValuePair<KeyCode, KeyCode>, Action>> snapshot2 = new List<KeyValuePair<KeyValuePair<KeyCode, KeyCode>, Action>>(chords);
                foreach (KeyValuePair<KeyValuePair<KeyCode, KeyCode>, Action> c in snapshot2)
                {
                    if (UnityEngine.Input.GetKey(c.Key.Key) && UnityEngine.Input.GetKeyDown(c.Key.Value) && c.Value != null)
                    {
                        c.Value();
                    }
                }
            }
        }
    }
}
