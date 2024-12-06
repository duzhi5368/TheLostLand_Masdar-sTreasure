using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class Vector3Variable : IVariable
    {
        [SerializeField]
        private Vector3 _value;

        public Vector3 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (Vector3)value; }
        }

        public override Type Type
        {
            get { return typeof(Vector3); }
        }

        public Vector3Variable() { }
        public Vector3Variable(string name) : base(name) { }
        public static implicit operator Vector3Variable(Vector3 value) {
            return new Vector3Variable() { _value = value };
        }
        public static implicit operator Vector3(Vector3Variable value)
        {
            return value.Value;
        }
    }
}
