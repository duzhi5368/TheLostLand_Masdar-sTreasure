using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//============================================================
namespace FKLib
{
    public class DialogBox : IUIWidget
    {
        [Tooltip("Closes the window when a button is clicked.")]
        public bool IsAutoClose = true;

        [Tooltip("The title component reference.")]
        public Text Title;

        [Tooltip("The text component reference.")]
        public Text Text;

        [Tooltip("The icon sprite reference.")]
        public Image Icon;

        [Tooltip("The button prefab reference.")]
        public Button Button;

        protected List<Button> _buttonCache = new List<Button>();
        protected GameObject _iconParent;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (Icon != null)
                _iconParent = Icon.GetComponentInParent<LayoutElement>().gameObject;

        }

        public virtual void Show(NotificationOptions settings, UnityAction<int> result, params string[] buttons)
        {
            Show(settings.Title, WidgetUtility.ColorString(settings.Text, settings.Color), settings.Icon, result, buttons);
        }

        public virtual void Show(string title, string text, params string[] buttons)
        {
            Show(title, text, null, null, buttons);
        }

        public virtual void Show(string title, string text, UnityAction<int> result, params string[] buttons)
        {
            Show(title, text, null, result, buttons);
        }

        public virtual void Show(string title, string text, Sprite icon, UnityAction<int> result, params string[] buttons)
        {
            for (int i = 0; i < _buttonCache.Count; i++)
            {
                _buttonCache[i].onClick.RemoveAllListeners();
                _buttonCache[i].gameObject.SetActive(false);
            }
            if (Title != null)
            {
                if (!string.IsNullOrEmpty(title))
                {
                    Title.text = title;
                    Title.gameObject.SetActive(true);
                }
                else
                {
                    Title.gameObject.SetActive(false);
                }
            }
            if (Text != null)
            {
                Text.text = text;
            }

            if (Icon != null)
            {
                if (icon != null)
                {
                    Icon.overrideSprite = icon;
                    _iconParent.SetActive(true);
                }
                else
                {
                    _iconParent.SetActive(false);
                }
            }
            base.Show();
            Button.gameObject.SetActive(false);
            for (int i = 0; i < buttons.Length; i++)
            {
                string caption = buttons[i];
                int index = i;
                AddButton(caption).onClick.AddListener(delegate ()
                {
                    if (IsAutoClose)
                    {
                        base.Close();
                    }
                    if (result != null)
                    {
                        result.Invoke(index);
                    }
                });
            }
        }

        private Button AddButton(string text)
        {
            Button tempButton = _buttonCache.Find(x => !x.isActiveAndEnabled);
            if (tempButton == null)
            {
                tempButton = Instantiate(Button) as Button;
                _buttonCache.Add(tempButton);
            }
            tempButton.gameObject.SetActive(true);
            tempButton.onClick.RemoveAllListeners();
            tempButton.transform.SetParent(Button.transform.parent, false);
            Text[] buttonTexts = tempButton.GetComponentsInChildren<Text>(true);
            if (buttonTexts.Length > 0)
            {
                buttonTexts[0].text = text;
            }
            return tempButton;
        }
    }
}
