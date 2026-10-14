using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.GooseApi;
using HonkAPI.Props;
using HonkAPI.World;

namespace HonkAPI.UI
{
    public class HonkLogEntry
    {
        public string Message;
        public LogType Type;
        public string TimeStr;

        public HonkLogEntry(string msg, LogType type)
        {
            Message = msg;
            Type = type;
            TimeStr = DateTime.Now.ToString("HH:mm:ss");
        }
    }

    public static class HonkConsole
    {
        private static readonly List<HonkLogEntry> logs = new List<HonkLogEntry>();
        private static readonly Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase);
        private static bool isOpen = false;
        private static bool initialized = false;
        private static string lastLogCondition = null;
        private static int duplicateCount = 0;
        private static string commandInput = string.Empty;
        private static Vector2 scrollPos = Vector2.zero;
        private static Rect consoleRect = new Rect(20, 20, 680, 360);

        public static bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            Application.logMessageReceived += HandleUnityLog;

            RegisterCommand("help", delegate(string[] args)
            {
                Log("Available commands: help, clear, honk, speed <mult>, size <mult>, tp <garden|shops|backyards|pub|finale>, props", LogType.Log);
            });

            RegisterCommand("clear", delegate(string[] args)
            {
                logs.Clear();
            });

            RegisterCommand("honk", delegate(string[] args)
            {
                GooseAPI.Honk();
                Log("Honk triggered!", LogType.Log);
            });

            RegisterCommand("speed", delegate(string[] args)
            {
                if (args.Length > 0)
                {
                    float spd;
                    if (float.TryParse(args[0], out spd))
                    {
                        GooseAPI.SetSpeedMultiplier(spd);
                        Log(string.Format("Speed multiplier set to {0:F1}", spd), LogType.Log);
                        return;
                    }
                }
                Log("Usage: speed <multiplier>", LogType.Warning);
            });

            RegisterCommand("size", delegate(string[] args)
            {
                if (args.Length > 0)
                {
                    float sz;
                    if (float.TryParse(args[0], out sz))
                    {
                        GooseAPI.SetScale(sz);
                        Log(string.Format("Goose scale set to {0:F1}", sz), LogType.Log);
                        return;
                    }
                }
                Log("Usage: size <multiplier>", LogType.Warning);
            });

            RegisterCommand("props", delegate(string[] args)
            {
                int count = PropAPI.GetAll().Count;
                Log(string.Format("Active props in world: {0}", count), LogType.Log);
            });

            RegisterCommand("tp", delegate(string[] args)
            {
                if (args.Length > 0)
                {
                    string target = args[0].ToLower();
                    if (target == "garden") GooseAPI.Teleport(new Vector3(4.8f, 0.1f, -12.4f));
                    else if (target == "shops") GooseAPI.Teleport(new Vector3(-13.4f, 1.4f, -44.5f));
                    else if (target == "backyards") GooseAPI.Teleport(new Vector3(-43.2f, 1.1f, -27.8f));
                    else if (target == "pub") GooseAPI.Teleport(new Vector3(-70.1f, 1.5f, 2.3f));
                    else if (target == "finale") GooseAPI.Teleport(new Vector3(-25.2f, 0.4f, 40.7f));
                    else Log("Unknown area. Valid: garden, shops, backyards, pub, finale", LogType.Warning);
                    return;
                }
                Log("Usage: tp <garden|shops|backyards|pub|finale>", LogType.Warning);
            });
        }

        public static void RegisterCommand(string name, Action<string[]> handler)
        {
            commands[name] = handler;
        }

        public static void Log(string message, LogType type = LogType.Log)
        {
            if (string.IsNullOrEmpty(message)) return;
            lock (logs)
            {
                if (message == lastLogCondition && logs.Count > 0)
                {
                    duplicateCount++;
                    logs[logs.Count - 1].Message = string.Format("{0} (x{1})", message, duplicateCount + 1);
                    return;
                }
                lastLogCondition = message;
                duplicateCount = 0;
                if (logs.Count > 250)
                {
                    logs.RemoveAt(0);
                }
                logs.Add(new HonkLogEntry(message, type));
                scrollPos.y = float.MaxValue;
            }
        }

        private static void HandleUnityLog(string condition, string stackTrace, LogType type)
        {
            if (condition != null && condition.StartsWith("[Honk"))
            {
                Log(condition, type);
            }
            else if (type == LogType.Exception || type == LogType.Error)
            {
                Log(condition, type);
            }
        }

        public static void Toggle()
        {
            IsOpen = !IsOpen;
        }

        public static void Render()
        {
            if (!IsOpen) return;

            HonkUITheme theme = HonkUITheme.Default;
            consoleRect = GUI.Window(99999, consoleRect, DrawConsoleContents, "Honk Developer Console", theme.CreateWindowStyle());

            consoleRect.x = Mathf.Clamp(consoleRect.x, 0, Screen.width - consoleRect.width);
            consoleRect.y = Mathf.Clamp(consoleRect.y, 0, Screen.height - consoleRect.height);
        }

        private static void DrawConsoleContents(int windowId)
        {
            HonkUITheme theme = HonkUITheme.Default;

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(consoleRect.height - 75));

            GUIStyle entryStyle = new GUIStyle(GUI.skin.label);
            entryStyle.fontSize = 11;

            lock (logs)
            {
                for (int i = 0; i < logs.Count; i++)
                {
                    HonkLogEntry e = logs[i];
                    switch (e.Type)
                    {
                        case LogType.Error:
                        case LogType.Exception:
                            entryStyle.normal.textColor = new Color(1.0f, 0.3f, 0.3f);
                            break;
                        case LogType.Warning:
                            entryStyle.normal.textColor = new Color(1.0f, 0.8f, 0.2f);
                            break;
                        default:
                            entryStyle.normal.textColor = Color.white;
                            break;
                    }
                    GUILayout.Label(string.Format("[{0}] {1}", e.TimeStr, e.Message), entryStyle);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("HonkConsoleInput");
            commandInput = GUILayout.TextField(commandInput, theme.CreateTextFieldStyle(), GUILayout.Height(24));

            if (GUILayout.Button("Execute", theme.CreateButtonStyle(), GUILayout.Width(80), GUILayout.Height(24)) ||
                (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "HonkConsoleInput"))
            {
                ExecuteCommand(commandInput);
                commandInput = string.Empty;
                Event.current.Use();
            }

            if (GUILayout.Button("Close", theme.CreateButtonStyle(), GUILayout.Width(60), GUILayout.Height(24)))
            {
                IsOpen = false;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, consoleRect.width, 24));
        }

        private static void ExecuteCommand(string cmdLine)
        {
            if (string.IsNullOrEmpty(cmdLine)) return;
            string trimmed = cmdLine.Trim();
            Log("> " + trimmed, LogType.Log);

            string[] parts = trimmed.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string cmd = parts[0];
            string[] args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);

            if (commands.ContainsKey(cmd))
            {
                try
                {
                    commands[cmd](args);
                }
                catch (Exception ex)
                {
                    Log(string.Format("Command error: {0}", ex.Message), LogType.Error);
                }
            }
            else
            {
                Log(string.Format("Unknown command: '{0}'. Type 'help' for commands.", cmd), LogType.Warning);
            }
        }
    }
}
