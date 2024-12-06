using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StatInspector
    {
        [SerializeField]
        private int _toolbarIndex;
        private StatDatabase _database;                 // 属性数据库对象
        private List<ICollectionEditor> _childEditors;  // 子编辑器页面

        private string[] ToolbarNames
        {
            get
            {
                string[] items = new string[_childEditors.Count];
                for (int i = 0; i < _childEditors.Count; i++)
                    items[i] = _childEditors[i].ToolbarName;
                return items;
            }
        }

        public void OnEnable()
        {
            _database = AssetDatabase.LoadAssetAtPath<StatDatabase>(EditorPrefs.GetString("StatDatabasePath"));
            if (_database == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:StatDatabase");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    this._database = AssetDatabase.LoadAssetAtPath<StatDatabase>(path);
                }
            }
            _toolbarIndex = EditorPrefs.GetInt("StatToolbarIndex");
            ResetChildEditors();
        }

        public void OnDisable()
        {
            if(_database != null)
                EditorPrefs.SetString("StatDatabasePath", AssetDatabase.GetAssetPath(_database));
            EditorPrefs.SetInt("StatToolbarIndex", _toolbarIndex);
            if(_childEditors != null)
            {
                for(int i = 0; i < _childEditors.Count;i++)
                    _childEditors[i].OnDisable();
            }
        }

        public void OnDestroy()
        {
            if(_childEditors != null)
            {
                for(int i = 0; i < _childEditors.Count;i++)
                    _childEditors[i].OnDestroy();
            }
        }

        public void OnGUI(Rect position)
        {
            DoToolbar();
            if (_childEditors != null)
                _childEditors[_toolbarIndex].OnGUI(new Rect(0f, 30f, position.width, position.height - 30f));
        }

        private void DoToolbar()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            SelectDatabaseButton();

            if (_childEditors != null)
                _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, ToolbarNames, GUILayout.MinWidth(200));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void SelectDatabaseButton()
        {
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(_database != null ? this._database.name : "Null");
            Rect buttonRect = GUILayoutUtility.GetRect(180f, 18f);
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(StatDatabase),
                    (UnityEngine.Object obj) =>
                    {
                        _database = obj as StatDatabase;
                        ResetChildEditors();
                    },
                    () =>
                    {
                        StatDatabase db = EditorTools.CreateAsset<StatDatabase>(true);
                        if (db != null)
                        {
                            _database = db;
                            ResetChildEditors();
                        }
                    });
            }
        }

        private void ResetChildEditors()
        {
            if(_database != null)
            {
                _childEditors = new List<ICollectionEditor>();
                _childEditors.Add(new StatCollectionEditor(_database, _database.Items, new List<string>()));
                _childEditors.Add(new ScriptableObjectCollectionEditor<StatEffect>("Effects", _database, _database.Effects, false));
                _childEditors.Add(new StatSettingsEditor(_database, _database.Settings));

                for (int i = 0; i < _childEditors.Count; i++)
                {
                    _childEditors[i].OnEnable();
                }
            }
        }
    }
}
