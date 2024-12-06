using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class BoolVariable : IVariable
    {
        [SerializeField]
        private bool _value;

        public bool Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (bool)value; }
        }

        public override Type Type
        {
            get { return typeof(bool); }
        }

        public BoolVariable() { }
        public BoolVariable(string name) : base(name) { }
        public static implicit operator BoolVariable(bool value) {
            return new BoolVariable() { Value = value };
        }
        public static implicit operator bool(BoolVariable value) {
            return value.Value; 
        }
    }
}
