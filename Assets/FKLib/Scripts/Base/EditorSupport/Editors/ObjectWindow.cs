using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class ObjectWindow : EditorWindow
    {
        private static object           _sObjectToCopy;

        private SerializedObject        _serializedObject;
        private SerializedProperty      _serializedProperty;
        private GameObject              _gameObject;
        private int                     _componentInstanceID;
        private UnityEngine.Object      _target;
        private string                  _propertyPath;
        private IList                   _list;
        private string                  _fieldName;
        private string                  _elementTypeName;
        private Type                    _elementType;
        private Vector2                 _scrollPosition;
        private System.Action           _onChange;

        public static void ShowWindow(string title, SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            ObjectWindow[] objArray = Resources.FindObjectsOfTypeAll<ObjectWindow>();
            ObjectWindow window = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<ObjectWindow>() : objArray[0]);

            window.hideFlags = HideFlags.HideAndDontSave;
            window.minSize = new Vector2(260f, 200f);
            window.titleContent = new GUIContent(title);
            window.Initialize(serializedObject, serializedProperty);
            window.ShowUtility();
        }

        public static void ShowWindow(string title, IList list, System.Action onChange)
        {
            ObjectWindow[] objArray = Resources.FindObjectsOfTypeAll<ObjectWindow>();
            ObjectWindow window = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<ObjectWindow>() : objArray[0]);

            window.hideFlags = HideFlags.HideAndDontSave;
            window.minSize = new Vector2(260f, 200f);
            window.titleContent = new GUIContent(title);

            window.Initialize(list, onChange);
            window.ShowUtility();
        }

        private void Initialize(IList list, System.Action onChange)
        {
            this._list = list;
            this._elementType = Utility.GetElementType(this._list.GetType());
            this._elementTypeName = this._elementType.Name;
            this._onChange = onChange;
        }

        private void Initialize(SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            this._serializedObject = serializedObject;
            this._serializedProperty = serializedProperty;
            this._propertyPath = serializedProperty.propertyPath;
            this._target = serializedObject.targetObject;
            this._list = serializedProperty.GetValue() as IList;
            this._elementType = Utility.GetElementType(this._list.GetType());
            this._elementTypeName = this._elementType.Name;
            FieldInfo[] fields = this._target.GetType().GetSerializedFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object temp = fields[i].GetValue(this._target);
                if (temp == this._list)
                    this._fieldName = fields[i].Name;
            }
        }

        void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChange;
            Selection.selectionChanged += OnSelectionChange;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChange;
            Selection.selectionChanged -= OnSelectionChange;
            GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(_target);
            if (prefab != null)
                PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition);
            GUILayout.Space(1f);
            if (this._target != null)
            {
                DoSerializedPropertyGUI();
            }
            else
            {
                DoListGUI();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            DoAddButton();
            GUILayout.Space(10f);
        }

        private void DoListGUI()
        {
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < this._list.Count; i++)
            {
                object value = this._list[i];
                if (EditorTools.Titlebar(value, GetObjectMenu(i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", EditorTools.FindMonoScript(value.GetType()), typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    EditorTools.DrawFields(value);
                    EditorGUI.indentLevel -= 1;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                _onChange.Invoke();
            }
        }

        private void DoSerializedPropertyGUI()
        {
            this._serializedObject.Update();
            for (int i = 0; i < this._list.Count; i++)
            {
                object value = this._list[i];
                EditorGUI.BeginChangeCheck();
                if (this._target != null)
                    Undo.RecordObject(this._target, "ObjectWindow");

                if (EditorTools.Titlebar(value, GetObjectMenu(i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", EditorTools.FindMonoScript(value.GetType()), typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    SerializedProperty element = this._serializedProperty.GetArrayElementAtIndex(i);
                    if (EditorTools.HasCustomPropertyDrawer(value.GetType()))
                    {
                        EditorGUILayout.PropertyField(element, true);
                    }
                    else
                    {
                        foreach (var child in element.EnumerateChildProperties())
                        {
                            EditorGUILayout.PropertyField(
                                child,
                                includeChildren: true
                            );
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(this._target);
                }
            }
            this._serializedObject.ApplyModifiedProperties();
        }

        private void DoAddButton()
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add " + this._elementType.Name);
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.width = buttonStyle.fixedWidth;
            buttonRect.x = position.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AddObjectWindow.ShowWindow(buttonRect, this._elementType, Add, CreateScript);
            }
        }

        private void CreateScript(string scriptName)
        {
            Debug.LogWarning("This is not implemented yet!");
        }

        private void Add(Type type)
        {
            object value = System.Activator.CreateInstance(type);
            if (this._target != null)
            {
                this._serializedObject.Update();
                this._serializedProperty.arraySize++;
                this._serializedProperty.GetArrayElementAtIndex(this._serializedProperty.arraySize - 1).managedReferenceValue = value;
                this._serializedObject.ApplyModifiedProperties();
                GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(_target);
                if (prefab != null)
                    PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
            }
            else
            {
                this._list.Add(value);
                _onChange.Invoke();
            }
        }

        private GenericMenu GetObjectMenu(int index)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, delegate
            {
                object value = System.Activator.CreateInstance(this._list[index].GetType());
                this._list[index] = value;
                if (_onChange != null)
                    _onChange.Invoke();
            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove " + this._elementType.Name), false, delegate {
                this._list.RemoveAt(index);
                if (this._target != null)
                    EditorUtility.SetDirty(this._target);

                if (_onChange != null)
                    _onChange.Invoke();
            });

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate
                {
                    object value = this._list[index];
                    this._list.RemoveAt(index);
                    this._list.Insert(index - 1, value);
                    if (this._target != null)
                        EditorUtility.SetDirty(this._target);
                    if (_onChange != null)
                        _onChange.Invoke();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < this._list.Count - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                    object value = this._list[index];
                    this._list.RemoveAt(index);
                    this._list.Insert(index + 1, value);
                    if (this._target != null)
                        EditorUtility.SetDirty(this._target);
                    if (_onChange != null)
                        _onChange.Invoke();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            menu.AddItem(new GUIContent("Copy " + this._elementType.Name), false, delegate
            {
                object value = this._list[index];
                ObjectWindow._sObjectToCopy = value;
                if (_onChange != null)
                    _onChange.Invoke();
            });

            if (ObjectWindow._sObjectToCopy != null)
            {
                menu.AddItem(new GUIContent("Paste " + this._elementType.Name + " As New"), false, delegate
                {
                    object instance = System.Activator.CreateInstance(ObjectWindow._sObjectToCopy.GetType());
                    FieldInfo[] fields = instance.GetType().GetSerializedFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value = fields[i].GetValue(ObjectWindow._sObjectToCopy);
                        fields[i].SetValue(instance, value);
                    }
                    this._list.Insert(index + 1, instance);
                    if (this._target != null)
                        EditorUtility.SetDirty(this._target);

                    if (_onChange != null)
                        _onChange.Invoke();
                });

                if (this._list[index].GetType() == ObjectWindow._sObjectToCopy.GetType())
                {
                    menu.AddItem(new GUIContent("Paste " + this._elementType.Name + " Values"), false, delegate
                    {
                        object instance = this._list[index];
                        FieldInfo[] fields = instance.GetType().GetSerializedFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            object value = fields[i].GetValue(ObjectWindow._sObjectToCopy);
                            fields[i].SetValue(instance, value);
                        }
                        if (this._target != null)
                            EditorUtility.SetDirty(this._target);

                        if (_onChange != null)
                            _onChange.Invoke();
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste " + this._elementType.Name + " Values"));
                }
            }

            MonoScript script = EditorTools.FindMonoScript(this._list[index].GetType());
            if (script != null)
            {
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Edit Script"), false, delegate { AssetDatabase.OpenAsset(script); });
            }
            return menu;
        }

        private void OnPlaymodeStateChange(PlayModeStateChange stateChange)
        {
            Reload();
        }

        public void OnBeforeAssemblyReload()
        {
            if (this._target != null && this._target is Component)
            {
                this._gameObject = (this._target as Component).gameObject;
                this._componentInstanceID = (this._target as Component).GetInstanceID();
            }
        }

        public void OnAfterAssemblyReload()
        {
            Reload();
        }

        private void OnSelectionChange()
        {
            Reload();
        }

        private void Reload()
        {
            if (this._target == null)
            {
                Close();
                return;
            }

            if (this._gameObject != null)
            {
                Component[] components = this._gameObject.GetComponents(typeof(Component));
                this._target = Array.Find(components, x => x.GetInstanceID() == this._componentInstanceID);
            }
            this._serializedObject = new SerializedObject(this._target);
            this._serializedProperty = this._serializedObject.FindProperty(this._propertyPath);
            this._elementType = Utility.GetType(this._elementTypeName);
            this._list = this._target.GetType().GetSerializedField(this._fieldName).GetValue(this._target) as IList;
        }
    }
}
