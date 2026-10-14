using System;
using UnityEngine;

namespace HonkAPI.UI
{
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Danger,
        Honk
    }

    public class HonkNotification
    {
        public string Message;
        public NotificationType Type;
        public float Duration;
        public float TimeRemaining;

        public HonkNotification(string message, NotificationType type, float duration)
        {
            Message = message;
            Type = type;
            Duration = duration;
            TimeRemaining = duration;
        }

        public Color GetAccentColor()
        {
            switch (Type)
            {
                case NotificationType.Success:
                    return new Color(0.2f, 0.8f, 0.35f, 1.0f);
                case NotificationType.Warning:
                    return new Color(1.0f, 0.72f, 0.1f, 1.0f);
                case NotificationType.Danger:
                    return new Color(0.95f, 0.25f, 0.25f, 1.0f);
                case NotificationType.Honk:
                    return new Color(1.0f, 0.48f, 0.0f, 1.0f);
                default:
                    return new Color(0.3f, 0.65f, 1.0f, 1.0f);
            }
        }

        public string GetPrefix()
        {
            switch (Type)
            {
                case NotificationType.Success:
                    return "[OK] ";
                case NotificationType.Warning:
                    return "[!] ";
                case NotificationType.Danger:
                    return "[X] ";
                case NotificationType.Honk:
                    return "[HONK] ";
                default:
                    return "[i] ";
            }
        }
    }
}
