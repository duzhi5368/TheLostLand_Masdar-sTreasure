using System;
//============================================================
namespace FKLib
{
    [Serializable]
    public class UISettings : ISettings
    {
        public override string Name
        {
            get { return "UI"; }
        }

        [InspectorLabel("Notification", "Name of Notification widget.")]
        public string NotificationName = "Notification";
        [InspectorLabel("Dialog Box", "Name of the dialog box widget.")]
        public string DialogBoxName = "Dialog Box";

        // todo: use custom ui system or not, need to think about it...
    }
}
