using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class StatAttribute : IStat
    {
        public System.Action OnCurrentValueChange;

        [SerializeField]
        [Range(0f, 1f)]
        protected float _startCurrentValue = 1f;

        protected float _currentValue;
        public float CurrentValue
        {
            get { return _currentValue; }
            set
            {
                float singe = Mathf.Clamp(value, 0, Value);
                if (_currentValue != singe) 
                {
                    _currentValue = singe;
                    OnCurrentValueChange?.Invoke();
                }
            }
        }

        public override void Initialize(StatsHandler handler, StatOverride statOverride)
        {
            base.Initialize(handler, statOverride);
            OnValueChange += () =>
            {
                CurrentValue = Mathf.Clamp(_currentValue, 0, Value);
            };
        }

        public override void ApplyStartValues()
        {
            base.ApplyStartValues();
            _currentValue = _value * _startCurrentValue;
        }

        public override string ToString()
        {
            return this._statName + ": " + this.CurrentValue.ToString() + "/" + this.Value.ToString();
        }

        public override void GetObjectData(Dictionary<string, object> data)
        {
            base.GetObjectData(data);
            data.Add("CurrentValue", _currentValue);
        }

        public override void SetObjectData(Dictionary<string, object> data)
        {
            base.SetObjectData(data);
            this._currentValue = Convert.ToSingle(data["CurrentValue"]);
            CalculateValue(false);
        }
    }
}
