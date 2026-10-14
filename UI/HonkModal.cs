using System;
using UnityEngine;

namespace HonkAPI.UI
{
    public static class HonkModal
    {
        private static bool isShowing = false;
        private static bool isInputModal = false;
        private static string title = string.Empty;
        private static string message = string.Empty;
        private static string inputText = string.Empty;
        private static string confirmText = "Confirm";
        private static string cancelText = "Cancel";
        private static Action<bool> confirmCallback;
        private static Action<string> inputCallback;

        public static void ShowConfirm(string modalTitle, string modalMessage, Action<bool> callback, string ok = "Confirm", string cancel = "Cancel")
        {
            title = modalTitle;
            message = modalMessage;
            confirmCallback = callback;
            confirmText = ok;
            cancelText = cancel;
            isInputModal = false;
            isShowing = true;
        }

        public static void ShowInput(string modalTitle, string promptMessage, Action<string> callback, string defaultValue = "")
        {
            title = modalTitle;
            message = promptMessage;
            inputCallback = callback;
            inputText = defaultValue ?? string.Empty;
            confirmText = "Submit";
            cancelText = "Cancel";
            isInputModal = true;
            isShowing = true;
        }

        public static void Close()
        {
            isShowing = false;
            confirmCallback = null;
            inputCallback = null;
        }

        public static void Render()
        {
            if (!isShowing) return;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), HonkUITheme.GetColoredTexture(new Color(0f, 0f, 0f, 0.6f)));

            float w = 360f;
            float h = isInputModal ? 190f : 160f;
            float x = (Screen.width - w) / 2f;
            float y = (Screen.height - h) / 2f;
            Rect modalRect = new Rect(x, y, w, h);

            HonkUITheme theme = HonkUITheme.Default;
            GUI.Box(modalRect, GUIContent.none, theme.CreateWindowStyle());

            GUILayout.BeginArea(new Rect(x + 16, y + 16, w - 32, h - 32));

            GUILayout.Label(title, theme.CreateHeaderStyle());
            GUILayout.Space(6);
            GUILayout.Label(message, theme.CreateLabelStyle(false));
            GUILayout.Space(8);

            if (isInputModal)
            {
                inputText = GUILayout.TextField(inputText, theme.CreateTextFieldStyle());
                GUILayout.Space(8);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(confirmText, theme.CreateButtonStyle(), GUILayout.Height(30)))
            {
                isShowing = false;
                if (isInputModal && inputCallback != null)
                {
                    inputCallback(inputText);
                }
                else if (!isInputModal && confirmCallback != null)
                {
                    confirmCallback(true);
                }
            }

            if (GUILayout.Button(cancelText, theme.CreateButtonStyle(), GUILayout.Height(30)))
            {
                isShowing = false;
                if (!isInputModal && confirmCallback != null)
                {
                    confirmCallback(false);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
