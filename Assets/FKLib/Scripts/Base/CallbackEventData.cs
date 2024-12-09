using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class CallbackEventData
    {
        private Dictionary<string, object> _properties;

        public CallbackEventData() { 
            _properties = new Dictionary<string, object>();
        }

        public void AddData(string key, object value)
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = value;
            }
            else
            {
                _properties.Add(key, value);
            }
        }

        public object GetData(string key)
        {
            if (_properties.ContainsKey(key))
            {
                return _properties[key];
            }
            else
            {
                return null;
            }
        }
    }
}
