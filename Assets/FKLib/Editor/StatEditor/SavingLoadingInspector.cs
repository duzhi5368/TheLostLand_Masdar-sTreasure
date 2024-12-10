using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
//============================================================
namespace FKLib
{
    [CustomEditor(typeof(SavingLoadingSettings))]
    public class SavingLoadingInspector : Editor
    {
        private SerializedProperty  _autoSave;
        private AnimBool            _isShowSave;
        private SerializedProperty  _savingKey;
        private SerializedProperty  _savingRate;
        private SerializedProperty  _saveScript;
        private SerializedProperty  _loadScript;

        protected virtual void OnEnable()
        {
            if (target == null) 
                return;
            _autoSave = serializedObject.FindProperty("IsAutoSave");
            _isShowSave = new AnimBool(_autoSave.boolValue);
            _isShowSave.valueChanged.AddListener(new UnityAction(Repaint));
            _savingKey = serializedObject.FindProperty("SavingKey");
            _savingRate = serializedObject.FindProperty("SavingRate");
            _saveScript = serializedObject.FindProperty("SaveScript");
            _loadScript = serializedObject.FindProperty("LoadScript");
        }

        public override void OnInspectorGUI()
        {
            if (target == null) 
                return;

            serializedObject.Update();
            EditorGUILayout.PropertyField(_autoSave);
            _isShowSave.target = _autoSave.boolValue;
            if (EditorGUILayout.BeginFadeGroup(_isShowSave.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(_savingKey);
                EditorGUILayout.PropertyField(_savingRate);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.Space(2f);
            EditorTools.Seperator();
            SavedDataGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void SavedDataGUI()
        {
            List<string> keys = PlayerPrefs.GetString("StatSystemSavedKeys").Split(';').ToList();
            keys.RemoveAll(x => string.IsNullOrEmpty(x));

            if (EditorTools.Foldout("StatSystemSavedData", new GUIContent("已保存数据数量: " + keys.Count)))
            {
                EditorTools.BeginIndent(1, true);
                if (keys.Count == 0)
                {
                    GUILayout.Label("本设备尚无数据被保存.");
                }

                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i];
                    GenericMenu keyMenu = new GenericMenu();
                    keyMenu.AddItem(new GUIContent("Delete Key"), false, () =>
                    {
                        List<string> allKeys = new List<string>(keys);
                        allKeys.Remove(key);
                        PlayerPrefs.SetString("StatSystemSavedKeys", string.Join(";", allKeys));
                        PlayerPrefs.DeleteKey(key + ".Stats");
                    });

                    if (EditorTools.Foldout(key, new GUIContent(key), keyMenu))
                    {
                        EditorTools.BeginIndent(1, true);
                        string data = PlayerPrefs.GetString(key + ".Stats");
                        GUILayout.Label(data, EditorStyles.wordWrappedLabel);
                        EditorTools.EndIndent();
                    }
                }
                EditorTools.EndIndent();
            }
        }
    }
}
