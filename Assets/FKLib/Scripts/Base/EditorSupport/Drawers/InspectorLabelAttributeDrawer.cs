using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [CustomPropertyDrawer(typeof(InspectorLabelAttribute))]
    public class InspectorLabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorLabelAttribute attr = attribute as InspectorLabelAttribute;
            EditorGUI.PropertyField(position, property, new GUIContent(attr.Label, attr.Tooltip));
        }
    }
}
