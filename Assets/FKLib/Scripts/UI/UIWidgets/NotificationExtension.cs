using UnityEngine.Events;
//============================================================
namespace FKLib
{
    public static class NotificationExtension
    {
        public static void Show(this NotificationOptions options, UnityAction<int> result, params string[] buttons)
        {
            if (StatsManager.UISettings.DialogBox != null)
            {
                StatsManager.UISettings.DialogBox.Show(options, result, buttons);
            }
        }

        public static void Show(this NotificationOptions options, params string[] replacements)
        {
            if (StatsManager.UISettings.Notification != null)
            {
                StatsManager.UISettings.Notification.AddItem(options, replacements);
            }
        }

        public static void SimpleDialogBoxNotice(string title, string text)
        {
            if (StatsManager.UISettings.DialogBox != null)
            {
                string[] buttons = { "OK" };
                StatsManager.UISettings.DialogBox.Show(title, text, buttons);
            }
        }

        public static void SimpleNotification(string text)
        {
            if (StatsManager.UISettings.Notification != null)
            {
                StatsManager.UISettings.Notification.AddItem(text);
            }
        }
    }
}
