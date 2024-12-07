using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class AddObjectWindowStyles
    {
        public GUIStyle Header = new GUIStyle("DD HeaderStyle");
        public GUIStyle RightArrow = "AC RightArrow";
        public GUIStyle LeftArrow = "AC LeftArrow";
        public GUIStyle ElementButton = new GUIStyle("MeTransitionSelectHead");
        public GUIStyle Background = "grey_border";

        public AddObjectWindowStyles()
        {
            Header.stretchWidth = true;
            Header.margin = new RectOffset(1, 1, 0, 4);
            ElementButton.alignment = TextAnchor.MiddleLeft;
            ElementButton.padding.left = 22;
            ElementButton.margin = new RectOffset(1, 1, 0, 0);
            ElementButton.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
        }
    }
}
