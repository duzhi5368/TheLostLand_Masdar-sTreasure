using System;
//============================================================
namespace FKLib
{
    public sealed class ComponentMenuAttribute : Attribute
    {
        private string _componentMenu;

        public string componentMenu
        {
            get
            {
                return this._componentMenu;
            }
        }

        public ComponentMenuAttribute(string menuName)
        {
            this._componentMenu = menuName;
        }
    }
}
