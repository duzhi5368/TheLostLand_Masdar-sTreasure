using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public static class CollectionEditorStyles
    {
        public static GUIStyle      MinusButton;
        public static GUIStyle      SelectButton;
        public static GUIStyle      Background;
        public static GUIStyle      SelectButtonText;
        public static Color         NormalColor;
        public static Color         HoverColor;
        public static Color         ActiveColor;
        public static Color         WarningColor;
        public static GUIStyle      DragInsertion;
        public static Texture2D     ErrorIcon;
        public static GUIStyle      indicatorColor;

        private static GUIStyle     _leftPaneDark;
        private static GUIStyle     _leftPaneLight;
        private static GUIStyle     _centerPaneDark;
        private static GUIStyle     _centerPaneLight;
        private static GUISkin      _skin;

        public static GUIStyle  LeftPane
        {
            get { return EditorGUIUtility.isProSkin ? _leftPaneDark : _leftPaneLight; }
        }

        public static GUIStyle CenterPane
        {
            get { return EditorGUIUtility.isProSkin ? _centerPaneDark : _centerPaneLight; }
        }

        static CollectionEditorStyles()
        {
            //_skin = Resources.Load<GUISkin>("EditorSkin");
            _skin = AssetDatabase.LoadAssetAtPath<GUISkin>(Path.EDITOR_GUI_SKIN_FILE);
            if (_skin == null)
            {
                Debug.LogError("Can't load EditorSkin GUISkin...");
            }
            else
            {
                Debug.Log("EditorSkin GUISkin has custom styles number: " +_skin.customStyles.Length);
            }
            _leftPaneLight = _skin.GetStyle("Left Pane");
            _centerPaneLight = _skin.GetStyle("Center Pane");
            _leftPaneDark = _skin.GetStyle("Left Pane Dark");
            _centerPaneDark = _skin.GetStyle("Center Pane Dark");
            

            NormalColor = EditorGUIUtility.isProSkin ? new Color(0.219f, 0.219f, 0.219f, 1f) : new Color(0.796f, 0.796f, 0.796f, 1f);
            HoverColor = EditorGUIUtility.isProSkin ? new Color(0.266f, 0.266f, 0.266f, 1f) : new Color(0.69f, 0.69f, 0.69f, 1f);
            ActiveColor = EditorGUIUtility.isProSkin ? new Color(0.172f, 0.364f, 0.529f, 1f) : new Color(0.243f, 0.459f, 0.761f, 1f);
            WarningColor = new Color(0.9f, 0.37f, 0.32f, 1f);
            ErrorIcon = EditorGUIUtility.LoadRequired("console.erroricon") as Texture2D;    // º”‘ÿƒ⁄÷√ICON

            MinusButton = new GUIStyle("OL Minus")
            {
                margin = new RectOffset(0, 0, 4, 0)
            };
            SelectButton = new GUIStyle("MeTransitionSelectHead")
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 0, 0, 0),
                overflow = new RectOffset(0, -1, 0, 0),
            };
            SelectButton.normal.background = ((GUIStyle)"ColorPickerExposureSwatch").normal.background;

            SelectButtonText = new GUIStyle("MeTransitionSelectHead")
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 0, 0, 0),
                overflow = new RectOffset(0, -1, 0, 0),
                richText = true
            };
            SelectButtonText.normal.background = null;
            SelectButtonText.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
            Background = new GUIStyle("PopupCurveSwatchBackground");
            DragInsertion = new GUIStyle("PR Insertion");
            indicatorColor = new GUIStyle(SelectButton);
            indicatorColor.margin = new RectOffset(0, 0, 4, 0);
            indicatorColor.padding = new RectOffset(1, 1, 1, 1);
        }
    }
}
