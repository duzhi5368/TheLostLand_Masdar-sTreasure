using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class ObjectVariable : IVariable
    {
        [SerializeField]
        private UnityEngine.Object _value;

        public UnityEngine.Object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (UnityEngine.Object)value; }
        }

        public override Type Type
        {
            get { return typeof(UnityEngine.Object); }
        }

        public ObjectVariable() { }
        public ObjectVariable(string name) : base(name) { }
        public static implicit operator ObjectVariable(UnityEngine.Object value){
            return new ObjectVariable() { _value = value };
        }
        public static implicit operator UnityEngine.Object(ObjectVariable value)
        {
            return value.Value;
        }
    }
}
