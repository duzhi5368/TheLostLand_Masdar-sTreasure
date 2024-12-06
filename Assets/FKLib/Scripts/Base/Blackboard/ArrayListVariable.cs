using System;
using System.Collections;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class ArrayListVariable : IVariable
    {
        private ArrayList _value = new ArrayList();

        public ArrayList Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get
            {
                if (_value == null)
                {
                    _value = new ArrayList();
                }
                return _value;
            }
            set
            {
                _value = (ArrayList)value;
            }
        }

        public override Type Type
        {
            get { return typeof(ArrayList); }
        }

        public ArrayListVariable() { }
        public ArrayListVariable(string name) : base(name) { }
        public static implicit operator ArrayListVariable(ArrayList value)
        {
            return new ArrayListVariable() { _value = value };
        }
        public static implicit operator ArrayList(ArrayListVariable value)
        {
            return value.Value;
        }
    }
}
