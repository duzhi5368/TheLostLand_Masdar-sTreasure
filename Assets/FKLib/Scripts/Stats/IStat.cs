using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class IStat : ScriptableObject, INameable, IJsonSerializable /*, IGraphProvider */
    {
        public System.Action OnValueChange;

        private System.Action _onValueChangeInternal;

        #region 编辑器中的数据
        [InspectorLabel("名称")]
        [SerializeField]
        protected string _statName = "新属性";
        public string Name { get => this._statName; set => this._statName = value; }

        [InspectorLabel("基础值")]
        [SerializeField]
        protected float _baseValue;

        [InspectorLabel("最大值")]
        [SerializeField]
        protected float _maxValue = -1;

        [SerializeReference]
        protected List<StatCallback> _callbacks = new List<StatCallback>();
        #endregion

        [NonSerialized]
        protected float _value;
        public float Value { get => _value; }

        protected List<StatModifier> _statModifiers = new List<StatModifier>();
        protected StatsHandler _statsHandler;

        public virtual void Initialize(StatsHandler handler, StatOverride statOverride)
        {

        }

        public virtual void ApplyStartValues() 
        {
            CalculateValue();
        }

        public void Set(float amount)
        {
            _baseValue = amount;
            _baseValue = Mathf.Clamp(_baseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void Add(float amount)
        {
            _baseValue += amount;
            _baseValue = Mathf.Clamp(_baseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void Sub(float amount)
        {
            _baseValue -= amount;
            _baseValue = Mathf.Clamp(_baseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void CalculateValue()
        {
            CalculateValue(true);
        }

        public void CalculateValue(bool invokeCallbacks)
        {
            float finalValue = _baseValue;
            float sumPercentAdd = 0f;
            _statModifiers.Sort((x, y) => x.Type.CompareTo(y.Type));

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                StatModifier statModifier = _statModifiers[i];
                if (statModifier.Type == ENUM_StatModType.eSMT_Flat)
                {
                    finalValue += statModifier.Value;
                }
                else if (statModifier.Type == ENUM_StatModType.eSMT_PercentAdd)
                {
                    sumPercentAdd += statModifier.Value;
                    if (i + 1 >= _statModifiers.Count || _statModifiers[i + 1].Type != ENUM_StatModType.eSMT_PercentAdd)
                    {
                        finalValue *= 1f + sumPercentAdd;
                        sumPercentAdd = 0f;
                    }
                }
                else if(statModifier.Type == ENUM_StatModType.eSMT_PercentMult)
                {
                    finalValue *= 1f + statModifier.Value;
                }
            }

            if(_maxValue >= 0)
                finalValue = Mathf.Clamp(finalValue, 0, _maxValue);

            if(_value != finalValue)
            {
                _value = finalValue;
                if (invokeCallbacks)
                    OnValueChange?.Invoke();
                _onValueChangeInternal?.Invoke();
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            _statModifiers.Add(modifier);
            CalculateValue();
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (_statModifiers.Remove(modifier))
            {
                CalculateValue();
                return true;
            }
            return false;
        }

        public bool RemoveModifiersFromSource(object source)
        {
            int numRemovals = _statModifiers.RemoveAll(mod => mod.Source == source);
            if (numRemovals > 0)
            {
                CalculateValue();
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return _statName + ": " + Value.ToString();
        }

        public virtual void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", _statName);
            data.Add("BaseValue", _baseValue);
        }

        public virtual void SetObjectData(Dictionary<string, object> data)
        {
            _baseValue = Convert.ToSingle(data["BaseValue"]);
            CalculateValue(false);
        }
    }
}
