using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.UI
{
    public static class HonkUI
    {
        private static readonly List<HonkNotification> notifications = new List<HonkNotification>();
        private static readonly List<HonkWindow> activeWindows = new List<HonkWindow>();
        private static GameObject uiHost;
        private static bool isCinematicMode = false;

        public static bool CinematicMode
        {
            get { return isCinematicMode; }
            set
            {
                isCinematicMode = value;
                if (InGameUI.instance != null && InGameUI.instance.overlayToggler != null)
                {
                    InGameUI.instance.overlayToggler.gameObject.SetActive(!value);
                }
            }
        }

        public static HonkWindow CreateWindow(string title, Rect initialRect)
        {
            EnsureHost();
            HonkWindow win = new HonkWindow(title, initialRect);
            activeWindows.Add(win);
            return win;
        }

        public static void RemoveWindow(HonkWindow window)
        {
            if (activeWindows.Contains(window))
            {
                activeWindows.Remove(window);
            }
        }

        public static void ShowNotification(string message, NotificationType type = NotificationType.Info, float duration = 3.5f)
        {
            EnsureHost();
            lock (notifications)
            {
                notifications.Add(new HonkNotification(message, type, duration));
            }
        }

        public static void ShowSuccess(string message, float duration = 3.5f)
        {
            ShowNotification(message, NotificationType.Success, duration);
        }

        public static void ShowWarning(string message, float duration = 3.5f)
        {
            ShowNotification(message, NotificationType.Warning, duration);
        }

        public static void ShowError(string message, float duration = 3.5f)
        {
            ShowNotification(message, NotificationType.Danger, duration);
        }

        public static void ShowHonk(string message, float duration = 3.5f)
        {
            ShowNotification(message, NotificationType.Honk, duration);
        }

        public static void ShowConfirm(string title, string message, Action<bool> callback, string ok = "Confirm", string cancel = "Cancel")
        {
            EnsureHost();
            HonkModal.ShowConfirm(title, message, callback, ok, cancel);
        }

        public static void ShowInput(string title, string prompt, Action<string> callback, string defaultValue = "")
        {
            EnsureHost();
            HonkModal.ShowInput(title, prompt, callback, defaultValue);
        }

        public static void ToggleConsole()
        {
            EnsureHost();
            HonkConsole.Toggle();
        }

        public static void ShowHonkScreen(bool show)
        {
            if (InGameUI.instance != null)
            {
                InGameUI.SetHonkScreenShowing(show);
            }
        }

        public static void OpenGoalsNotebook(bool open)
        {
            if (InGameUI.instance != null)
            {
                InGameUI.SetGoalsMenuOpen(open);
            }
        }

        public static void OpenPauseMenu(bool open)
        {
            if (InGameUI.instance != null)
            {
                InGameUI.SetPauseMenuOpen(open);
            }
        }

        private static void EnsureHost()
        {
            if (uiHost == null)
            {
                uiHost = new GameObject("HonkUIHost");
                UnityEngine.Object.DontDestroyOnLoad(uiHost);
                uiHost.AddComponent<HonkUIDrawer>();
                HonkConsole.Initialize();
            }
        }

        private class HonkUIDrawer : MonoBehaviour
        {
            private GUIStyle notifyStyle;
            private GUIStyle notifyPrefixStyle;

            private void Update()
            {
                HonkHUD.Update();

                if (Input.GetKeyDown(KeyCode.F10) || Input.GetKeyDown(KeyCode.BackQuote))
                {
                    HonkConsole.Toggle();
                }

                for (int i = 0; i < activeWindows.Count; i++)
                {
                    HonkWindow win = activeWindows[i];
                    if (win != null && win.ToggleKey != KeyCode.None && Input.GetKeyDown(win.ToggleKey))
                    {
                        win.Toggle();
                    }
                }

                lock (notifications)
                {
                    for (int i = notifications.Count - 1; i >= 0; i--)
                    {
                        notifications[i].TimeRemaining -= Time.unscaledDeltaTime;
                        if (notifications[i].TimeRemaining <= 0f)
                        {
                            notifications.RemoveAt(i);
                        }
                    }
                }
            }

            private void OnGUI()
            {
                if (isCinematicMode) return;

                HonkHUD.Render();

                for (int i = 0; i < activeWindows.Count; i++)
                {
                    if (activeWindows[i] != null)
                    {
                        activeWindows[i].Render();
                    }
                }

                HonkConsole.Render();
                HonkModal.Render();

                DrawNotifications();
            }

            private void DrawNotifications()
            {
                if (notifications.Count == 0) return;

                if (notifyStyle == null)
                {
                    notifyStyle = new GUIStyle(GUI.skin.label);
                    notifyStyle.fontSize = 13;
                    notifyStyle.fontStyle = FontStyle.Bold;
                    notifyStyle.normal.textColor = Color.white;
                    notifyStyle.alignment = TextAnchor.MiddleLeft;

                    notifyPrefixStyle = new GUIStyle(notifyStyle);
                    notifyPrefixStyle.alignment = TextAnchor.MiddleCenter;
                }

                lock (notifications)
                {
                    float y = 24f;
                    for (int i = 0; i < notifications.Count; i++)
                    {
                        HonkNotification item = notifications[i];
                        string prefix = item.GetPrefix();
                        string msg = item.Message;
                        Color accent = item.GetAccentColor();

                        GUIContent content = new GUIContent(prefix + " " + msg);
                        Vector2 size = notifyStyle.CalcSize(content);
                        float w = Mathf.Max(280f, size.x + 36f);
                        float h = 38f;
                        float x = (Screen.width - w) / 2f;

                        Rect bgRect = new Rect(x, y, w, h);
                        GUI.DrawTexture(bgRect, HonkUITheme.GetColoredTexture(new Color(0.10f, 0.11f, 0.13f, 0.95f)));

                        Rect barRect = new Rect(x, y, 4f, h);
                        GUI.DrawTexture(barRect, HonkUITheme.GetColoredTexture(accent));

                        notifyPrefixStyle.normal.textColor = accent;
                        GUI.Label(new Rect(x + 10f, y + 2f, 65f, h - 4f), prefix, notifyPrefixStyle);

                        GUI.Label(new Rect(x + 75f, y + 2f, w - 85f, h - 4f), msg, notifyStyle);

                        y += h + 8f;
                    }
                }
            }
        }
    }
}
