using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class NotificationOptions
    {
        public string Title = string.Empty;     // the notification's title
        public string Text = string.Empty;      // the message to display
        public Color Color = Color.white;       // the text's color
        public Sprite Icon;                     // the icon to display
        public float Delay = 2.0f;              // the delay before fading
        public float Duration = 2.0f;           // the duration of fading
        public bool IsIgnoreTimeScale = true;   // is ignore timescale

        public NotificationOptions() { }
        public NotificationOptions(NotificationOptions other)
        {
            Title = other.Title;
            Text = other.Text;
            Icon = other.Icon;
            Color = other.Color;
            Delay = other.Delay;
            Duration = other.Duration;
            IsIgnoreTimeScale = other.IsIgnoreTimeScale;
        }
    }
}
