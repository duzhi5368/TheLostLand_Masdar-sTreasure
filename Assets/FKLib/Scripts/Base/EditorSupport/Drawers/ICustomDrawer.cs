using System.Reflection;
using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class ICustomDrawer
    {
        public object DeclaringObject;
        public object Value;
        public FieldInfo FieldInfo;
        public bool IsDirty = false;

        public virtual void OnGUI(GUIContent label)
        {

        }

        public void SetDirty()
        {
            IsDirty = true;
        }
    }
}
