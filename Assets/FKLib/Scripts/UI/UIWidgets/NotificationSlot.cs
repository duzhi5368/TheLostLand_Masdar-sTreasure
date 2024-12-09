using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//============================================================
namespace FKLib
{
    public class NotificationSlot : TUISlot<NotificationOptions>
    {
        [Tooltip("Notification text to display")]
        [SerializeField]
        protected Text _text;

        [Tooltip("Notification time to display")]
        [SerializeField]
        protected Text _time;

        [Tooltip("Notification icon to display")]
        [SerializeField]
        protected Image _icon;


        public override void Repaint()
        {
            if (ObservedItem != null)
            {
                Notification container = Container as Notification;
                if (_text != null)
                {
                    _text.text = WidgetUtility.ColorString(ObservedItem.Text, ObservedItem.Color);
                    DelayCrossFade(_text, ObservedItem);
                }
                if (_time != null)
                {
                    _time.text = (string.IsNullOrEmpty(container.TimeFormat) ? "" : "[" + DateTime.Now.ToString(container.TimeFormat) + "] ");
                    DelayCrossFade(_time, ObservedItem);
                }
                if (_icon != null)
                {
                    _icon.gameObject.SetActive(ObservedItem.Icon != null);
                    if (ObservedItem.Icon != null)
                    {
                        _icon.overrideSprite = ObservedItem.Icon;
                        DelayCrossFade(_icon, ObservedItem);
                    }
                }
            }
        }

        private void DelayCrossFade(Graphic graphic, NotificationOptions options)
        {
            if ((Container as Notification).IsFade)
                StartCoroutine(DelayCrossFade(graphic, options.Delay, options.Duration, options.IsIgnoreTimeScale));
        }

        private IEnumerator DelayCrossFade(Graphic graphic, float delay, float duration, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);
            graphic.CrossFadeAlpha(0f, duration, ignoreTimeScale);
        }
    }
}
