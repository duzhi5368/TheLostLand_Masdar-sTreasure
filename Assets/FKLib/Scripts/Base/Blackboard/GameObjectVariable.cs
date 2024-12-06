using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class GameObjectVariable : IVariable
    {
        [SerializeField]
        private GameObject _value = null;

        public GameObject Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object RawValue
        {
            get { return _value; }
            set { _value = (GameObject)value; }
        }

        public override Type Type
        {
            get { return typeof(GameObject); }
        }

        public GameObjectVariable() { }
        public GameObjectVariable(string name) : base(name) { }
        public static implicit operator GameObjectVariable(GameObject value)
        {
            return new GameObjectVariable() { _value = value };
        }
        public static implicit operator GameObject(GameObjectVariable value) {
            return value.Value;
        }
    }
}
