using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class Vector2Variable : IVariable
    {
        [SerializeField]
        private Vector2 _value;

        public Vector2 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (Vector2)value; }
        }

        public override Type Type
        {
            get { return typeof(Vector2); }
        }

        public Vector2Variable() { }
        public Vector2Variable(string name) : base(name) { }
        public static implicit operator Vector2Variable(Vector2 value) { 
            return new Vector2Variable() { Value = value };
        }
        public static implicit operator Vector2(Vector2Variable value)
        {
            return value.Value;
        }
    }
}
