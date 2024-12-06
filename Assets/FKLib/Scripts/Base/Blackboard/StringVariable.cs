using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StringVariable : IVariable
    {
        [SerializeField]
        private string _value = string.Empty;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (string)value; }
        }

        public override Type Type
        {
            get { return typeof(string); }
        }

        public StringVariable() { }
        public StringVariable(string name) : base(name) { }
        public static implicit operator StringVariable(string value)
        {
            return new StringVariable() { _value = value };
        }
        public static implicit operator string(StringVariable v) {
            return v.Value;
        }
    }
}
