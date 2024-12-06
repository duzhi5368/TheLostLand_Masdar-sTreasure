using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public static class EditorToolsStyles
    {
        public static GUIStyle Seperator;
        public static Texture2D RightArrow;
        public static GUIStyle LeftTextButton;
        public static GUIStyle LeftTextToolbarButton;
        public static GUIStyle InspectorTitle;
        public static GUIStyle InspectorTitleText;
        public static GUIStyle InspectorBigTitle;

        static EditorToolsStyles()
        {
            EditorToolsStyles.Seperator = new GUIStyle("IN Title")
            {
                fixedHeight = 1f
            };
            EditorToolsStyles.RightArrow = ((GUIStyle)"AC RightArrow").normal.background;
            EditorToolsStyles.LeftTextButton = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleLeft
            };
            EditorToolsStyles.LeftTextToolbarButton = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleLeft
            };
            EditorToolsStyles.InspectorTitle = new GUIStyle("IN Foldout")
            {
                overflow = new RectOffset(0, 0, -3, 0),
                fixedWidth = 0,
                fixedHeight = 20
            };
            EditorToolsStyles.InspectorTitleText = new GUIStyle("IN TitleText");
            EditorToolsStyles.InspectorBigTitle = new GUIStyle("IN BigTitle");
        }
    }
}
