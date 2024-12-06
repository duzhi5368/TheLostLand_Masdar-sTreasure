using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class ObjectPickerWindow : EditorWindow
    {
        private static ObjectPickerWindowStyles _sStyles;

        private string _searchString = string.Empty;
        private bool IsSearching { get { return !string.IsNullOrEmpty(_searchString); } }
        private Vector2 _scrollPosition;
        private Type _type;
        private bool _isSelectChildren = false;
        private UnityEngine.Object _root;
        private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> _selectableObjects;
        private bool _isAcceptNull;

        public delegate void SelectCallbackDelegate(UnityEngine.Object obj);
        public SelectCallbackDelegate OnSelectCallback;
        public delegate void CreateCallbackDelegate();
        public CreateCallbackDelegate OnCreateCallback;

        public static void ShowWindow<T>(Rect buttonRect, SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull = false)
        {
            ShowWindow(buttonRect, typeof(T), selectCallback, createCallback, acceptNull);
        }

        public static void ShowWindow(Rect buttonRect, Type type, SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull = false)
        {
            ObjectPickerWindow window = ScriptableObject.CreateInstance<ObjectPickerWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window._type = type;
            window.BuildSelectableObjects(type);
            window.OnSelectCallback = selectCallback;
            window.OnCreateCallback = createCallback;
            window._isAcceptNull = acceptNull;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
        }

        public static void ShowWindow(Rect buttonRect, Type type, Dictionary<UnityEngine.Object, List<UnityEngine.Object>> selectableObjects,
            SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull = false)
        {
            ObjectPickerWindow window = ScriptableObject.CreateInstance<ObjectPickerWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window._selectableObjects = selectableObjects;
            window._type = type;
            window._isSelectChildren = true;
            window.OnSelectCallback = selectCallback;
            window.OnCreateCallback = createCallback;
            window._isAcceptNull = acceptNull;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (ObjectPickerWindow._sStyles == null)
            {
                ObjectPickerWindow._sStyles = new ObjectPickerWindowStyles();
            }
            GUILayout.Space(5f);
            _searchString = SearchField(_searchString);
            Header();

            DrawSelectableObjects();

            if (Event.current.type == EventType.Repaint)
            {
                ObjectPickerWindow._sStyles.Background.Draw(new Rect(0, 0, position.width, position.height), false, false, false, false);
            }
        }

        private void Header()
        {
            GUIContent content = new GUIContent(_root == null ? "Select " + ObjectNames.NicifyVariableName(_type.Name) : _root.name);
            Rect headerRect = GUILayoutUtility.GetRect(content, ObjectPickerWindow._sStyles.Header);
            if (GUI.Button(headerRect, content, ObjectPickerWindow._sStyles.Header))
            {
                _root = null;
            }
        }

        private void BuildSelectableObjects(Type type)
        {
            _selectableObjects = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();

            string[] guids = AssetDatabase.FindAssets("t:" + type.FullName);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, type);
                this._selectableObjects.Add(obj, new List<UnityEngine.Object>());
            }
        }

        private bool SearchMatch(UnityEngine.Object element)
        {
            if (IsSearching && (element == null || !element.name.ToLower().Contains(_searchString.ToLower())))
            {
                return false;
            }
            return true;
        }

        private static Rect GUIToScreenRect(Rect guiRect)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
            guiRect.x = vector.x;
            guiRect.y = vector.y;
            return guiRect;
        }

        private string SearchField(string search, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            string before = search;

            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, "ToolbarSeachTextField", options);
            rect.x += 2f;
            rect.width -= 2f;
            Rect buttonRect = rect;
            buttonRect.x = rect.width - 14;
            buttonRect.width = 14;

            if (!string.IsNullOrEmpty(before))
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);

            if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition) || before == "Search..." && GUI.GetNameOfFocusedControl() == "SearchTextFieldFocus")
            {
                before = "";
                GUI.changed = true;
                GUI.FocusControl(null);

            }
            GUI.SetNextControlName("SearchTextFieldFocus");
            GUIStyle style = new GUIStyle("ToolbarSeachTextField");
            if (before == "Search...")
            {
                style.normal.textColor = Color.gray;
                style.hover.textColor = Color.gray;
            }
            string after = EditorGUI.TextField(rect, "", before, style);
            EditorGUI.FocusTextInControl("SearchTextFieldFocus");

            GUI.Button(buttonRect, GUIContent.none, (after != "" && after != "Search...") ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty");
            EditorGUILayout.EndHorizontal();
            return after;
        }

        private void DrawSelectableObjects()
        {
            List<UnityEngine.Object> selectableObjects = _root == null ? _selectableObjects.Keys.ToList() : _selectableObjects[_root];

            _scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition);
            foreach (UnityEngine.Object obj in selectableObjects)
            {
                if (!SearchMatch(obj))
                    continue;

                Color backgroundColor = GUI.backgroundColor;
                Color textColor = ObjectPickerWindow._sStyles.ElementButton.normal.textColor;
                int padding = ObjectPickerWindow._sStyles.ElementButton.padding.left;
                GUIContent label = new GUIContent(obj.name);
                Rect rect = GUILayoutUtility.GetRect(label, ObjectPickerWindow._sStyles.ElementButton, GUILayout.Height(20f));
                GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));
                ObjectPickerWindow._sStyles.ElementButton.normal.textColor = (rect.Contains(Event.current.mousePosition) ? Color.white : textColor);

                Texture2D icon = EditorGUIUtility.LoadRequired("d_ScriptableObject Icon") as Texture2D;
                IconAttribute iconAttribute = obj.GetType().GetCustomAttribute<IconAttribute>();
                if (iconAttribute != null)
                {
                    if (iconAttribute.Type != null)
                    {
                        icon = AssetPreview.GetMiniTypeThumbnail(iconAttribute.Type);
                    }
                    else
                    {
                        icon = Resources.Load<Texture2D>(iconAttribute.Path);
                    }
                }

                ObjectPickerWindow._sStyles.ElementButton.padding.left = (icon != null ? 22 : padding);

                if (GUI.Button(rect, label, ObjectPickerWindow._sStyles.ElementButton))
                {
                    if (_root != null && _selectableObjects[_root].Count > 0)
                    {
                        OnSelectCallback?.Invoke(obj);
                        Close();
                    }
                    _root = obj;
                    if (!_isSelectChildren)
                    {
                        OnSelectCallback?.Invoke(_root);
                        Close();
                    }

                }
                GUI.backgroundColor = backgroundColor;
                ObjectPickerWindow._sStyles.ElementButton.normal.textColor = textColor;
                ObjectPickerWindow._sStyles.ElementButton.padding.left = padding;

                if (icon != null)
                {
                    GUI.Label(new Rect(rect.x, rect.y, 20f, 20f), icon);
                }
            }

            if (_root == null)
            {
                if (_isAcceptNull)
                {
                    GUIContent nullContent = new GUIContent("Null");
                    Rect rect2 = GUILayoutUtility.GetRect(nullContent, ObjectPickerWindow._sStyles.ElementButton, GUILayout.Height(20f));
                    GUI.backgroundColor = (rect2.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));

                    if (GUI.Button(rect2, nullContent, ObjectPickerWindow._sStyles.ElementButton))
                    {
                        OnSelectCallback?.Invoke(null);
                        Close();
                    }
                    GUI.Label(new Rect(rect2.x, rect2.y, 20f, 20f), EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);
                }

                GUIContent createContent = new GUIContent("Create New " + _type.Name);
                Rect rect1 = GUILayoutUtility.GetRect(createContent, ObjectPickerWindow._sStyles.ElementButton, GUILayout.Height(20f));
                GUI.backgroundColor = (rect1.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));

                if (GUI.Button(rect1, createContent, ObjectPickerWindow._sStyles.ElementButton))
                {
                    OnCreateCallback?.Invoke();
                    Close();
                }
                GUI.Label(new Rect(rect1.x, rect1.y, 20f, 20f), EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);


            }
            EditorGUILayout.EndScrollView();
        }
    }
}
