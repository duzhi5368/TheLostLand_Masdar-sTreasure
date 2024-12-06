using System;
//============================================================
namespace FKLib
{
    [Serializable]
    public class NotificationSettings : ISettings
    {
        public override string Name
        {
            get { return "Notification"; }
        }
    }
}
