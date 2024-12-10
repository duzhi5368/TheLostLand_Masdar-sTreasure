using TMPro;
using UnityEngine;
using UnityEngine.UI;
//============================================================
namespace FKLib
{
    public class ExampleNotification : MonoBehaviour
    {
        private Notification _notification;
        public NotificationOptions[] options;

        public void Start()
        {
            _notification = WidgetUtility.Find<Notification>("Notification");
        }

        public void AddRandomNitification()
        {
            NotificationOptions option = options[Random.Range(0, options.Length)];
            _notification.AddItem(option);
        }

        public void AddNotification(InputField input)
        {
            _notification.AddItem(input.text);
        }

        public void AddNotification(TMP_InputField input)
        {
            _notification.AddItem(input.text);
        }

        public void AddNotification(float index)
        {
            NotificationOptions option = options[Mathf.RoundToInt(index)];
            _notification.AddItem(option);
        }
    }
}
