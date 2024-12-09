using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class SingleInstance : MonoBehaviour
    {
        private static Dictionary<string, GameObject> _sInstances = new Dictionary<string, GameObject> ();

        private void Awake()
        {
            GameObject instance = null;
            _sInstances.TryGetValue(name, out instance);
            if (instance == null) 
            {
                DontDestroyOnLoad(gameObject);
                _sInstances[name] = gameObject;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public static List<GameObject> GetInstanceObjects()
        {
            return _sInstances.Values.Where(x => x != null).ToList();
        }
    }
}
