using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.UI
{
    public class HonkUITheme
    {
        private static readonly Dictionary<Color, Texture2D> textureCache = new Dictionary<Color, Texture2D>();

        public Color WindowBackground = new Color(0.11f, 0.12f, 0.14f, 0.94f);
        public Color HeaderBackground = new Color(0.18f, 0.19f, 0.22f, 0.98f);
        public Color AccentColor = new Color(1.0f, 0.48f, 0.0f, 1.0f);
        public Color TextColor = Color.white;
        public Color TextMutedColor = new Color(0.7f, 0.7f, 0.75f, 1.0f);
        public Color ButtonBackground = new Color(0.22f, 0.24f, 0.28f, 1.0f);
        public Color ButtonHover = new Color(0.32f, 0.35f, 0.40f, 1.0f);
        public Color InputBackground = new Color(0.08f, 0.08f, 0.10f, 0.95f);
        public Color BorderColor = new Color(0.28f, 0.30f, 0.35f, 0.8f);

        private static HonkUITheme defaultTheme;
        public static HonkUITheme Default
        {
            get
            {
                if (defaultTheme == null)
                {
                    defaultTheme = new HonkUITheme();
                }
                return defaultTheme;
            }
        }

        public static Texture2D GetColoredTexture(Color color)
        {
            if (textureCache.ContainsKey(color))
            {
                return textureCache[color];
            }

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            textureCache[color] = tex;
            return tex;
        }

        private GUIStyle windowStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle labelBoldStyle;
        private GUIStyle headerStyle;
        private GUIStyle textFieldStyle;
        private GUIStyle toggleStyle;

        public GUIStyle CreateWindowStyle()
        {
            if (windowStyle == null)
            {
                windowStyle = new GUIStyle(GUI.skin.window);
                windowStyle.normal.background = GetColoredTexture(WindowBackground);
                windowStyle.onNormal.background = GetColoredTexture(WindowBackground);
                windowStyle.normal.textColor = TextColor;
                windowStyle.fontStyle = FontStyle.Bold;
                windowStyle.fontSize = 18;
                windowStyle.padding = new RectOffset(16, 16, 38, 16);
                windowStyle.border = new RectOffset(2, 2, 2, 2);
            }
            return windowStyle;
        }

        public GUIStyle CreateButtonStyle()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.normal.background = GetColoredTexture(ButtonBackground);
                buttonStyle.hover.background = GetColoredTexture(ButtonHover);
                buttonStyle.active.background = GetColoredTexture(AccentColor);
                buttonStyle.normal.textColor = TextColor;
                buttonStyle.hover.textColor = Color.white;
                buttonStyle.active.textColor = Color.black;
                buttonStyle.fontSize = 15;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.margin = new RectOffset(4, 4, 4, 4);
                buttonStyle.padding = new RectOffset(12, 12, 8, 8);
            }
            return buttonStyle;
        }

        public GUIStyle CreateLabelStyle(bool bold = false)
        {
            if (bold)
            {
                if (labelBoldStyle == null)
                {
                    labelBoldStyle = new GUIStyle(GUI.skin.label);
                    labelBoldStyle.normal.textColor = TextColor;
                    labelBoldStyle.fontSize = 15;
                    labelBoldStyle.fontStyle = FontStyle.Bold;
                    labelBoldStyle.margin = new RectOffset(4, 4, 3, 3);
                }
                return labelBoldStyle;
            }
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = TextColor;
                labelStyle.fontSize = 15;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.margin = new RectOffset(4, 4, 3, 3);
            }
            return labelStyle;
        }

        public GUIStyle CreateHeaderStyle()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(GUI.skin.label);
                headerStyle.normal.textColor = AccentColor;
                headerStyle.fontSize = 18;
                headerStyle.fontStyle = FontStyle.Bold;
                headerStyle.margin = new RectOffset(4, 4, 12, 6);
            }
            return headerStyle;
        }

        public GUIStyle CreateTextFieldStyle()
        {
            if (textFieldStyle == null)
            {
                textFieldStyle = new GUIStyle(GUI.skin.textField);
                textFieldStyle.normal.background = GetColoredTexture(InputBackground);
                textFieldStyle.normal.textColor = Color.white;
                textFieldStyle.focused.textColor = Color.white;
                textFieldStyle.fontSize = 15;
                textFieldStyle.padding = new RectOffset(8, 8, 6, 6);
                textFieldStyle.margin = new RectOffset(4, 4, 4, 4);
            }
            return textFieldStyle;
        }

        public GUIStyle CreateToggleStyle()
        {
            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(GUI.skin.toggle);
                toggleStyle.normal.textColor = TextColor;
                toggleStyle.onNormal.textColor = AccentColor;
                toggleStyle.fontSize = 15;
                toggleStyle.fontStyle = FontStyle.Bold;
                toggleStyle.margin = new RectOffset(4, 4, 4, 4);
            }
            return toggleStyle;
        }
    }
}
