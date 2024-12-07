using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [CustomPropertyDrawer(typeof(CompoundAttribute))]
    public class CompoundAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.wideMode = true;
            string propertyPath = (attribute as CompoundAttribute).PropertyPath;
            SerializedProperty compoundProperty = property.serializedObject.FindProperty(propertyPath);
            if (compoundProperty.boolValue)
            {
                position.x += 15f;
                position.width -= 15f;
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string propertyPath = (attribute as CompoundAttribute).PropertyPath;
            SerializedProperty compoundProperty = property.serializedObject.FindProperty(propertyPath);
            return compoundProperty.boolValue ? base.GetPropertyHeight(property, label) : 0f;
        }
    }
}
