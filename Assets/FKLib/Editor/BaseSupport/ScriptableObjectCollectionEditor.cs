using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    // A collection class for ScriptableObjects.
    [Serializable]
    public class ScriptableObjectCollectionEditor<T> : TCollectionEditor<T> where T : ScriptableObject, INameable
    {
        [SerializeField]
        protected List<T> _items;
        protected override List<T> Items { get { return _items; } }

        [SerializeField]
        protected UnityEngine.Object _target;
        protected Editor _editor;
        protected bool _isUseInspectorDefaultMargins = false;
        protected override bool IsUseInspectorDefaultMargins => _isUseInspectorDefaultMargins;

        public ScriptableObjectCollectionEditor(UnityEngine.Object target, List<T> items, bool isUseInspectorDefaultMargins = true)
            : this(string.Empty, target, items, isUseInspectorDefaultMargins) { }

        public ScriptableObjectCollectionEditor(string title, UnityEngine.Object target, List<T> items, bool isUseInspectorDefaultMargins = true)
            : base(title) {
            _target = target;
            _items = items;
            _isUseInspectorDefaultMargins = isUseInspectorDefaultMargins;
        }

        protected override bool MatchesSearch(T item, string search)
        {
            return (!item.Name.ToLower().Contains(search.ToLower()) || search.ToLower() == item.GetType().Name.ToLower());
        }

        protected override void Create()
        {
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract).ToArray();
            if (types.Length > 1)
            {
                GenericMenu menu = new GenericMenu();
                foreach (Type type in types)
                {
                    Type mType = type;
                    menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(mType.Name)), false, delegate ()
                    {
                        CreateItem(mType);
                    });
                }
                menu.ShowAsContext();
            }
            else
            {
                CreateItem(types[0]);
            }
        }

        protected virtual void CreateItem(Type type)
        {
            T item = (T)ScriptableObject.CreateInstance(type);
            item.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(item, _target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Items.Add(item);
            Select(item);

            EditorUtility.SetDirty(_target);
        }

        protected override void Remove(T item)
        {
            if (EditorUtility.DisplayDialog("Delete Item", "Are you sure you want to delete " + item.Name + "?", "Yes", "No"))
            {
                GameObject.DestroyImmediate(item, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Items.Remove(item);
                base._selectedItemIndex = -1;
                if (_editor != null)
                    ScriptableObject.DestroyImmediate(_editor);
                EditorUtility.SetDirty(_target);
            }
        }

        protected override void Duplicate(T item)
        {
            T duplicate = (T)ScriptableObject.Instantiate(item);
            duplicate.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(duplicate, _target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Items.Add(duplicate);
            Select(duplicate);
            EditorUtility.SetDirty(_target);
        }

        protected override void Select(T item)
        {
            base.Select(item);
            if (_editor != null)
                ScriptableObject.DestroyImmediate(_editor);
            _editor = Editor.CreateEditor(item);
        }

        protected override void DrawItem(T item)
        {
            if (_editor != null)
                _editor.OnInspectorGUI();
        }

        public override void OnDestroy()
        {
            if(_editor != null)
                ScriptableObject.DestroyImmediate(_editor);
        }

        protected override string GetSidebarLabel(T item)
        {
            return item.Name;
        }

        protected override void AddContextItem(GenericMenu menu)
        {
            base.AddContextItem(menu);
            menu.AddItem(new GUIContent("Sort/A-Z"), false, delegate
            {
                T selected = SelectedItem;
                Items.Sort(delegate (T a, T b) { return a.Name.CompareTo(b.Name); });
                Select(selected);
            });
            menu.AddItem(new GUIContent("Sort/Type"), false, delegate
            {
                T selected = SelectedItem;
                Items.Sort(delegate (T a, T b)
                {
                    return a.GetType().FullName.CompareTo(b.GetType().FullName);
                });
                Select(selected);
            });
        }
    }
}
