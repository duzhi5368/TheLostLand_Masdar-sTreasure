using System;
using UnityEngine;
using UnityEngine.Assertions;
//============================================================
namespace FKLib
{
    [Serializable]
    public class UISettings : ISettings
    {
        public override string Name
        {
            get { return "UI设置"; }
        }

        [InspectorLabel("日志框名称", "Name of Notification widget.")]
        public string NotificationName = "Notification";
        [InspectorLabel("提示框名称", "Name of the dialog box widget.")]
        public string DialogBoxName = "Dialog Box";

        private Notification _notifocation;
        public Notification Notification
        {
            get
            {
                if(_notifocation == null)
                {
                    _notifocation = WidgetUtility.Find<Notification>(NotificationName);
                    Debug.Log(_notifocation);
                }
                Assert.IsNotNull(_notifocation, "Notification widget with name " + NotificationName + " is not present in scene.");
                return _notifocation;
            }
        }

        private DialogBox _dialogBox;
        public DialogBox DialogBox
        {
            get
            {
                if (_dialogBox == null)
                {
                    _dialogBox = WidgetUtility.Find<DialogBox>(DialogBoxName);
                }
                Assert.IsNotNull(_dialogBox, "DialogBox widget with name " + DialogBoxName + " is not present in scene.");
                return _dialogBox;
            }
        }
    }
}
