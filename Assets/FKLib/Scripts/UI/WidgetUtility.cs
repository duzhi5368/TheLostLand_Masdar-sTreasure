using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
//============================================================
namespace FKLib
{
    public static class WidgetUtility
    {
        // the widget cache
        private static Dictionary<string, List<IUIWidget>> _sWidgetCache = new Dictionary<string, List<IUIWidget>>();

        private static AudioSource _sAudioSource;


        static WidgetUtility()
        {
            SceneManager.activeSceneChanged += ChangedActiveScene;
        }
        private static void ChangedActiveScene(Scene current, Scene next)
        {
            _sWidgetCache.Clear();
        }

        // Get an IUIWidget by name
        public static T Find<T>(string name) where T : IUIWidget
        {
            return (T)(FindAll<T>(name).FirstOrDefault());
        }

        // Get IUIWidgets by name
        public static T[] FindAll<T>(string name) where T : IUIWidget
        {
            List<IUIWidget> current = null;
            if (!_sWidgetCache.TryGetValue(name, out current) || current.Count == 0)
            {
                current = new List<IUIWidget>();
                Canvas[] canvas = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                for (int c = 0; c < canvas.Length; c++)
                {
                    T[] windows = canvas[c].GetComponentsInChildren<T>(true);
                    current.AddRange(windows.Where(x => x.Name == name).OrderByDescending(y => y.Priority).Cast<IUIWidget>());
                }
                current = current.Distinct().ToList();
                if (!_sWidgetCache.ContainsKey(name))
                {
                    _sWidgetCache.Add(name, current);
                }
                else
                {
                    _sWidgetCache[name] = current;
                }
            }
            return current.Where(x => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToArray();
        }

        public static T[] FindAll<T>() where T : IUIWidget
        {
            List<IUIWidget> current = new List<IUIWidget>();
            Canvas[] canvas = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            for (int c = 0; c < canvas.Length; c++)
            {
                T[] windows = canvas[c].GetComponentsInChildren<T>(true);
                current.AddRange(windows.OrderByDescending(y => y.Priority).Cast<IUIWidget>());
            }
            return current.Distinct().Where(x => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToArray();
        }

        // Play an AudioClip
        public static void PlaySound(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }
            if (_sAudioSource == null)
            {
                AudioListener listener = GameObject.FindFirstObjectByType<AudioListener>();
                if (listener != null)
                {
                    _sAudioSource = listener.GetComponent<AudioSource>();
                    if (_sAudioSource == null)
                    {
                        _sAudioSource = listener.gameObject.AddComponent<AudioSource>();
                    }
                }
            }
            if (_sAudioSource != null)
            {
                _sAudioSource.PlayOneShot(clip, volume);
            }
        }

        // Converts a color to hex.
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        // Converts a hex string to color.
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");
            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        // Colors the string.
        public static string ColorString(string value, Color color)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return "<color=#" + ColorToHex(color) + ">" + value + "</color>";
        }
    }
}
