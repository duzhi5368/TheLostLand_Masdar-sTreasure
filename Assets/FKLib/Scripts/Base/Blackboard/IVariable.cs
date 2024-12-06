using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public abstract class IVariable
    {
        [SerializeField]
        private string _name = string.Empty;

        public virtual string Name { 
            get { return _name; }
            set { _name = value; }
        }

        [SerializeField]
        private bool _isShared;

        public virtual bool IsShared
        {
            get { return _isShared; }
            set { _isShared = value; }
        }

        public virtual bool IsNone
        {
            get { return (_name == "None" || string.IsNullOrEmpty(_name)) && _isShared; }
        }

        public abstract Type Type { get; }
        public abstract object RawValue { get; set; }

        public IVariable() { }
        public IVariable(string name) { _name = name; }
    }
}
