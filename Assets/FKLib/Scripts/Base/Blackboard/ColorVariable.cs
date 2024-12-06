using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class ColorVariable : IVariable
    {
        [SerializeField]
        private Color _value = Color.white;

        public Color Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (Color)value; }
        }

        public override Type Type 
        {
            get { return typeof(Color); }
        }

        public ColorVariable() { }
        public ColorVariable(string name) : base(name) { }
        public static implicit operator ColorVariable(Color value) {
            return new ColorVariable() { _value = value };
        }
        public static implicit operator Color(ColorVariable value) {
            return value.Value;
        }
    }
}
