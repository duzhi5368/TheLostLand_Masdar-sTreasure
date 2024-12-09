using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class WidgetInputHandler : MonoBehaviour
    {
        private static Dictionary<KeyCode, List<IUIWidget>> _sWidgetKeyBindings;

        private void Update()
        {
            if (_sWidgetKeyBindings == null)
                return;
            foreach (KeyValuePair<KeyCode, List<IUIWidget>> kvp in _sWidgetKeyBindings)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    for (int i = 0; i < kvp.Value.Count; i++)
                    {
                        kvp.Value[i].Toggle();

                    }
                }
            }
        }

        public static void RegisterInput(KeyCode key, IUIWidget widget)
        {
            if (_sWidgetKeyBindings == null)
            {
                WidgetInputHandler handler = GameObject.FindFirstObjectByType<WidgetInputHandler>();
                if (handler == null)
                {
                    GameObject handlerObject = new GameObject("WidgetInputHandler");
                    handlerObject.AddComponent<WidgetInputHandler>();
                    handlerObject.AddComponent<SingleInstance>();
                }
                _sWidgetKeyBindings = new Dictionary<KeyCode, List<IUIWidget>>();
            }
            if (key == KeyCode.None)
            {
                return;
            }

            List<IUIWidget> widgets;
            if (!_sWidgetKeyBindings.TryGetValue(key, out widgets))
            {
                _sWidgetKeyBindings.Add(key, new List<IUIWidget>() { widget });
            }
            else
            {
                widgets.RemoveAll(x => x == null);
                widgets.Add(widget);
                _sWidgetKeyBindings[key] = widgets;
            }
        }

        public static void UnregisterInput(KeyCode key, IUIWidget widget) 
        {
            if (_sWidgetKeyBindings == null)
                return;
            List<IUIWidget> widgets;
            if(_sWidgetKeyBindings.TryGetValue(key, out widgets))
                widgets.Remove(widget);
        }
    }
}
