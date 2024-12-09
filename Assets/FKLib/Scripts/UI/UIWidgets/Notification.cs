//============================================================
namespace FKLib
{
    public class Notification : TUIContainer<NotificationOptions>
    {
        public bool IsFade = true;
        public string TimeFormat = "HH:mm:ss";

        public virtual bool AddItem(NotificationOptions item, params string[] replacements)
        {
            NotificationOptions options = new NotificationOptions(item);
            for (int i = 0; i < replacements.Length; i++)
            {
                options.Text = options.Text.Replace("{" + i + "}", replacements[i]);
            }
            return base.AddItem(options);
        }

        public virtual bool AddItem(string text, params string[] replacements)
        {
            NotificationOptions options = new NotificationOptions();
            options.Text = text;
            for (int i = 0; i < replacements.Length; i++)
            {
                options.Text = options.Text.Replace("{" + i + "}", replacements[i]);
            }
            return base.AddItem(options);
        }

        public override bool CanAddItem(NotificationOptions item, out TUISlot<NotificationOptions> slot, bool createSlot = false)
        {
            slot = null;
            return gameObject.activeInHierarchy && base.CanAddItem(item, out slot, createSlot);
        }
    }
}
