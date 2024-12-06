using System;
//============================================================
namespace FKLib
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CustomDrawerAttribute : Attribute
    {
        private Type _type;
        public Type Type
        {
            get
            {
                return this._type;
            }
        }

        private bool _isUseForChildren;
        public bool IsUseForChildren
        {
            get
            {
                return this._isUseForChildren;
            }
        }

        public CustomDrawerAttribute(Type type)
        {
            this._type = type;
        }

        public CustomDrawerAttribute(Type type, bool isUseForChildren)
        {
            this._type = type;
            this._isUseForChildren = isUseForChildren;
        }
    }
}
