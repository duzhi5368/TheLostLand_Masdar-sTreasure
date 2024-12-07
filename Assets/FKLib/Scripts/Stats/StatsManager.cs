using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
//============================================================
namespace FKLib
{
    public class StatsManager : MonoBehaviour
    {
        // 自身单例对象
        private static StatsManager sStatsManagerInstance;
        public static StatsManager Instance
        {
            get
            {
                Assert.IsNotNull(sStatsManagerInstance, "Create one from Windows > FK状态管理器 > Create Stats Manager!\"");
                return sStatsManagerInstance;
            }
        }

        public bool IsDontDestoryOnLoad = true;
        private List<StatsHandler> _statsHandlers;


        // 状态数据库对象
        [SerializeField]
        private StatDatabase _statDatabase = null;
        public static StatDatabase StatDatabase
        {
            get
            {
                if(sStatsManagerInstance == null)
                    return null;

                Assert.IsNotNull(sStatsManagerInstance._statDatabase, "Please assign StatDatabase to the Stats Manager!");
                return sStatsManagerInstance._statDatabase;
            }
        }


        // 编辑器中 -> Setting -> Default设定区
        private static DefaultSettings _defaultSettings;
        public static DefaultSettings DefaultSettings 
        {
            get
            {
                if(_defaultSettings == null)
                    _defaultSettings = GetSetting<DefaultSettings>();
                return _defaultSettings;
            }
        }
        // 编辑器中 -> Setting -> UI设定区
        private static UISettings _uiSettings;
        public static UISettings UISettings
        {
            get
            {
                if (_uiSettings == null)
                    _uiSettings = GetSetting<UISettings>();
                return _uiSettings;
            }
        }
        // 编辑器中 -> Setting -> Notification设定区
        private static NotificationSettings _notificationSettings;
        public static NotificationSettings NotificationSettings
        {
            get
            {
                if (_notificationSettings == null)
                    _notificationSettings = GetSetting<NotificationSettings>();
                return _notificationSettings;
            }
        }
        // 编辑器中 -> Setting -> Saving&Loading设定区
        private static SavingLoadingSettings _savingLoadingSettings;
        public static SavingLoadingSettings SavingLoadingSettings
        {
            get
            {
                if (_savingLoadingSettings == null)
                    _savingLoadingSettings = GetSetting<SavingLoadingSettings>();
                return _savingLoadingSettings;
            }
        }

        private static T GetSetting<T>() where T : ISettings
        {
            if(StatDatabase != null)
                return (T)StatDatabase.Settings.Where(x => x.GetType() == typeof(T)).FirstOrDefault();
            return default(T);
        }

        private void Awake()
        {
            if(sStatsManagerInstance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                sStatsManagerInstance = this;
                if (IsDontDestoryOnLoad)
                {
                    if(transform.parent != null)
                    {
                        if(DefaultSettings.IsShowDebugMessages)
                            Debug.Log("Stats Manager with DontDestroyOnLoad can't be a child transform. Unparent!");
                        transform.parent = null;
                    }
                    DontDestroyOnLoad(gameObject);
                }

                _statsHandlers = new List<StatsHandler>();
                if (SavingLoadingSettings.IsAutoSave)
                {
                    StartCoroutine(RepeatSaving(SavingLoadingSettings.SavingRate));
                }
                if(DefaultSettings.IsShowDebugMessages)
                    Debug.Log("Stats Manager initialized.");
            }
        }

        private void Start()
        {
            if (_savingLoadingSettings.IsAutoSave)
                StartCoroutine(DelayedLoading(1f));
        }

        public static void Save()
        {
            string key = PlayerPrefs.GetString(SavingLoadingSettings.SavingKey, SavingLoadingSettings.SavingKey);
            Save(key);
        }

        public static void Save(string key)
        {
            StatsHandler[] results = Object.FindObjectsByType<StatsHandler>(FindObjectsSortMode.None).Where(x => x.IsSaveable).ToArray();
            if (results.Length > 0)
            {
                string data = JsonSerializer.Serialize(results);
                foreach(StatsHandler handler in results)
                {
                    foreach(IStat stat in handler.Stats)
                    {
                        PlayerPrefs.SetFloat(key + ".Stats." + handler.HandlerName + "." + stat.Name + ".Value", stat.Value);
                        if(stat is StatAttribute attribute)
                            PlayerPrefs.SetFloat(key + ".Stats." + handler.HandlerName + "." + stat.Name + ".CurrentValue", attribute.CurrentValue);
                    }
                }

                PlayerPrefs.SetString(key + ".Stats", data);

                List<string> keys = PlayerPrefs.GetString("StatSystemSavedKeys").Split(';').ToList();
                keys.RemoveAll(x => string.IsNullOrEmpty(x));
                if(!keys.Contains(key))
                    keys.Add(key);
                PlayerPrefs.SetString("StatSystemSavedKeys", string.Join(";", keys));

                if(DefaultSettings.IsShowDebugMessages)
                    Debug.Log("[Stat System] Stats saved: " + data);
            }
        }

        public static void Load()
        {
            string key = PlayerPrefs.GetString(SavingLoadingSettings.SavingKey, SavingLoadingSettings.SavingKey);
            Load(key);
        }

        public static void Load(string key)
        {
            string data = PlayerPrefs.GetString(key + ".Stats");
            if (string.IsNullOrEmpty(data))
                return;

            List<StatsHandler> results = Object.FindObjectsByType<StatsHandler>(FindObjectsSortMode.None).Where(x => x.IsSaveable).ToList();
            List<object> list = MiniJSON.Deserialize(data) as List<object>;

            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> handlerData = list[i] as Dictionary<string, object>;
                string handlerName = (string)handlerData["Name"];
                StatsHandler handler = results.Find(x => x.HandlerName == handlerName);
                if (handler != null)
                    handler.SetObjectData(handlerData);
            }

            if(DefaultSettings.IsShowDebugMessages)
                Debug.Log("[Stat System] Stats loaded: " + data);
        }

        private IEnumerator DelayedLoading(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Load();
        }

        private IEnumerator RepeatSaving(float seconds)
        {
            while (true) { 
                yield return new WaitForSeconds(seconds);
                Save();
            }
        }

        public static void RegisterStatsHandler(StatsHandler handler)
        {
            if(!sStatsManagerInstance._statsHandlers.Contains(handler))
                sStatsManagerInstance._statsHandlers.Add(handler);
        }

        public static StatsHandler GetStatsHandler(string key)
        {
            return sStatsManagerInstance._statsHandlers.Find(x => x.HandlerName == key);
        }
    }
}
