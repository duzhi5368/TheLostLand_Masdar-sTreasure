using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [CustomDrawer(typeof(List<Action>))]
    public class ActionListDrawer : ICustomDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            if (EditorTools.RightArrowButton(label, GUILayout.Height(20f)))
            {
                ObjectWindow.ShowWindow("Edit Actions", (IList)Value, SetDirty);
            }
        }
    }
}
