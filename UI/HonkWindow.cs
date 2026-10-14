using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.UI
{
    public interface IHonkUIElement
    {
        void Draw(HonkUITheme theme);
    }

    public class HonkUILabel : IHonkUIElement
    {
        public string Text;
        public bool IsBold;

        public HonkUILabel(string text, bool bold)
        {
            Text = text;
            IsBold = bold;
        }

        public void Draw(HonkUITheme theme)
        {
            GUILayout.Label(Text, theme.CreateLabelStyle(IsBold));
        }
    }

    public class HonkUIHeader : IHonkUIElement
    {
        public string Text;

        public HonkUIHeader(string text)
        {
            Text = text;
        }

        public void Draw(HonkUITheme theme)
        {
            GUILayout.Label(Text, theme.CreateHeaderStyle());
        }
    }

    public class HonkUIButton : IHonkUIElement
    {
        public string Text;
        public Action OnClick;

        public HonkUIButton(string text, Action onClick)
        {
            Text = text;
            OnClick = onClick;
        }

        public void Draw(HonkUITheme theme)
        {
            if (GUILayout.Button(Text, theme.CreateButtonStyle(), GUILayout.Height(38)))
            {
                if (OnClick != null)
                {
                    OnClick();
                }
            }
        }
    }

    public class HonkUIToggle : IHonkUIElement
    {
        public string Label;
        public bool Value;
        public Action<bool> OnChange;

        public HonkUIToggle(string label, bool initial, Action<bool> onChange)
        {
            Label = label;
            Value = initial;
            OnChange = onChange;
        }

        public void Draw(HonkUITheme theme)
        {
            bool newVal = GUILayout.Toggle(Value, " " + Label, theme.CreateToggleStyle());
            if (newVal != Value)
            {
                Value = newVal;
                if (OnChange != null)
                {
                    OnChange(Value);
                }
            }
        }
    }

    public class HonkUISlider : IHonkUIElement
    {
        public string Label;
        public float Value;
        public float Min;
        public float Max;
        public Action<float> OnChange;

        public HonkUISlider(string label, float initial, float min, float max, Action<float> onChange)
        {
            Label = label;
            Value = initial;
            Min = min;
            Max = max;
            OnChange = onChange;
        }

        public void Draw(HonkUITheme theme)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0}: {1:F2}", Label, Value), theme.CreateLabelStyle(false), GUILayout.Width(170));
            float newVal = GUILayout.HorizontalSlider(Value, Min, Max);
            GUILayout.EndHorizontal();

            if (Math.Abs(newVal - Value) > 0.0001f)
            {
                Value = newVal;
                if (OnChange != null)
                {
                    OnChange(Value);
                }
            }
        }
    }

    public class HonkUITextField : IHonkUIElement
    {
        public string Label;
        public string Text;
        public Action<string> OnChange;

        public HonkUITextField(string label, string initial, Action<string> onChange)
        {
            Label = label;
            Text = initial ?? string.Empty;
            OnChange = onChange;
        }

        public void Draw(HonkUITheme theme)
        {
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(Label))
            {
                GUILayout.Label(Label, theme.CreateLabelStyle(false), GUILayout.Width(120));
            }
            string newText = GUILayout.TextField(Text, theme.CreateTextFieldStyle());
            GUILayout.EndHorizontal();

            if (newText != Text)
            {
                Text = newText;
                if (OnChange != null)
                {
                    OnChange(Text);
                }
            }
        }
    }

    public class HonkUISeparator : IHonkUIElement
    {
        public void Draw(HonkUITheme theme)
        {
            GUILayout.Space(4);
            Rect r = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(1));
            GUI.DrawTexture(r, HonkUITheme.GetColoredTexture(theme.BorderColor));
            GUILayout.Space(4);
        }
    }

    public class HonkUISpace : IHonkUIElement
    {
        public float Height;

        public HonkUISpace(float height)
        {
            Height = height;
        }

        public void Draw(HonkUITheme theme)
        {
            GUILayout.Space(Height);
        }
    }

    public class HonkUITabs : IHonkUIElement
    {
        public string[] TabNames;
        public int SelectedIndex;
        public Action<int> OnTabChanged;

        public HonkUITabs(string[] names, Action<int> onTabChanged)
        {
            TabNames = names;
            SelectedIndex = 0;
            OnTabChanged = onTabChanged;
        }

        public void Draw(HonkUITheme theme)
        {
            if (TabNames == null || TabNames.Length == 0) return;

            GUILayout.BeginHorizontal();
            for (int i = 0; i < TabNames.Length; i++)
            {
                GUIStyle btnStyle = theme.CreateButtonStyle();
                if (i == SelectedIndex)
                {
                    btnStyle.normal.background = HonkUITheme.GetColoredTexture(theme.AccentColor);
                    btnStyle.normal.textColor = Color.black;
                }

                if (GUILayout.Button(TabNames[i], btnStyle, GUILayout.Height(36)))
                {
                    if (SelectedIndex != i)
                    {
                        SelectedIndex = i;
                        if (OnTabChanged != null)
                        {
                            OnTabChanged(SelectedIndex);
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
    }

    public class HonkWindow
    {
        private static int nextWindowId = 50000;

        public int WindowId { get; private set; }
        public string Title { get; set; }
        public Rect WindowRect;
        public bool IsVisible { get; set; }
        public bool IsDraggable { get; set; }
        public bool IsClosable { get; set; }
        public KeyCode ToggleKey { get; set; }
        public HonkUITheme Theme { get; set; }

        private readonly List<IHonkUIElement> elements = new List<IHonkUIElement>();
        private Action customDrawCallback;
        private Vector2 scrollPosition = Vector2.zero;

        public HonkWindow(string title, Rect initialRect)
        {
            WindowId = nextWindowId++;
            Title = title;
            WindowRect = initialRect;
            IsVisible = true;
            IsDraggable = true;
            IsClosable = true;
            ToggleKey = KeyCode.None;
            Theme = HonkUITheme.Default;
        }

        public HonkWindow WithHotkey(KeyCode key)
        {
            ToggleKey = key;
            return this;
        }

        public HonkWindow AddLabel(string text, bool bold = false)
        {
            elements.Add(new HonkUILabel(text, bold));
            return this;
        }

        public HonkWindow AddHeader(string text)
        {
            elements.Add(new HonkUIHeader(text));
            return this;
        }

        public HonkWindow AddButton(string text, Action onClick)
        {
            elements.Add(new HonkUIButton(text, onClick));
            return this;
        }

        public HonkWindow AddToggle(string label, bool initial, Action<bool> onChange)
        {
            elements.Add(new HonkUIToggle(label, initial, onChange));
            return this;
        }

        public HonkWindow AddSlider(string label, float initial, float min, float max, Action<float> onChange)
        {
            elements.Add(new HonkUISlider(label, initial, min, max, onChange));
            return this;
        }

        public HonkWindow AddTextField(string label, string initial, Action<string> onChange)
        {
            elements.Add(new HonkUITextField(label, initial, onChange));
            return this;
        }

        public HonkWindow AddTabs(string[] tabs, Action<int> onSelected)
        {
            elements.Add(new HonkUITabs(tabs, onSelected));
            return this;
        }

        public HonkWindow AddSeparator()
        {
            elements.Add(new HonkUISeparator());
            return this;
        }

        public HonkWindow AddSpace(float pixels = 6f)
        {
            elements.Add(new HonkUISpace(pixels));
            return this;
        }

        public HonkWindow OnDraw(Action callback)
        {
            customDrawCallback = callback;
            return this;
        }

        public void Toggle()
        {
            IsVisible = !IsVisible;
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public void Clear()
        {
            elements.Clear();
        }

        public void Render()
        {
            if (!IsVisible) return;

            GUIStyle winStyle = Theme.CreateWindowStyle();
            WindowRect = GUI.Window(WindowId, WindowRect, DrawWindowContents, Title, winStyle);

            WindowRect.x = Mathf.Clamp(WindowRect.x, 0, Screen.width - WindowRect.width);
            WindowRect.y = Mathf.Clamp(WindowRect.y, 0, Screen.height - WindowRect.height);
        }

        private void DrawWindowContents(int id)
        {
            if (IsClosable)
            {
                Rect closeBtnRect = new Rect(WindowRect.width - 34, 6, 26, 26);
                if (GUI.Button(closeBtnRect, "X", GUI.skin.button))
                {
                    IsVisible = false;
                }
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Draw(Theme);
            }

            if (customDrawCallback != null)
            {
                customDrawCallback();
            }

            GUILayout.EndScrollView();

            if (IsDraggable)
            {
                GUI.DragWindow(new Rect(0, 0, WindowRect.width, 36));
            }
        }
    }
}
