using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class FloatVariable : IVariable
    {
        [SerializeField]
        private float _value;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = Convert.ToSingle(value); }
        }

        public override Type Type
        {
            get { return typeof(float); }
        }

        public FloatVariable() { }
        public FloatVariable(string name) : base(name) { }
        public static implicit operator FloatVariable(float value) {
            return new FloatVariable() { _value = value };
        }
        public static implicit operator float(FloatVariable value) {
            return value.Value;
        }
    }
}
