using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [CustomPropertyDrawer(typeof(HeaderLineAttribute))]
    public class HeaderLineAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        public override void OnGUI(Rect position)
        {
            position.y = position.y + 2f;
            position = EditorGUI.IndentedRect(position);
            GUI.Label(position, (base.attribute as HeaderLineAttribute).Header, TextStyle);
            position.y += EditorGUIUtility.singleLineHeight + 2f;
            Color color = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 0.2f) : new Color(0.047f, 0.047f, 0.047f, 1f);
            GUI.Label(position, "", LineStyle);
            GUI.color = color;
        }

        private GUIStyle _lineStyle;
        private GUIStyle LineStyle
        {
            get
            {
                if (this._lineStyle == null)
                {
                    this._lineStyle = new GUIStyle();
                    this._lineStyle.fixedHeight = 1f;
                    this._lineStyle.margin = new RectOffset();
                    this._lineStyle.padding = new RectOffset();

                    this._lineStyle.normal.background = EditorGUIUtility.whiteTexture;
                }
                return this._lineStyle;
            }
        }

        private GUIStyle _textStyle;
        private GUIStyle TextStyle
        {
            get
            {
                if (this._textStyle == null)
                {
                    this._textStyle = new GUIStyle(EditorStyles.boldLabel);
                    this._textStyle.fontSize = 12;
                }
                return this._textStyle;
            }
        }
    }
}
