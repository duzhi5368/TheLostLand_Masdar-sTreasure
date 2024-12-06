using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class IntVariable : IVariable
    {
        [SerializeField]
        private int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue {
            get { return _value; }
            set { _value =  (int)value; }
        }

        public override Type Type
        {
            get { return typeof(int); }
        }

        public IntVariable() { }
        public IntVariable(string name) : base(name) { }
        public static implicit operator IntVariable(int value) {
            return new IntVariable() { _value = value };
        }
        public static implicit operator int(IntVariable value) { 
            return value.Value;
        }
    }
}
