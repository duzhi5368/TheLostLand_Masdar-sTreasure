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
    }
}
