using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StatCollectionEditor : ScriptableObjectCollectionEditor<IStat>
    {
        [SerializeField]
        protected List<string> _searchFilters;
        [SerializeField]
        protected string _searchFilter = "All";

        public override string ToolbarName { get { return "Stats"; } }

        public StatCollectionEditor(UnityEngine.Object target, List<IStat> items, List<string> searchFilters)
            : base(target, items)
        {
            _target = target;
            _items = items;
            _searchFilters = searchFilters;
            _searchFilter.Insert(0, "All");
            _searchString = "All";
        }

        protected override void Create()
        {
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(IStat).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract).ToArray();
            if (types.Length > 1)
            {
                GenericMenu menu = new GenericMenu();
                foreach (Type type in types)
                {
                    Type mType = type;
                    menu.AddItem(new GUIContent(mType.Name), false, delegate ()
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

        protected override void DoSearchGUI()
        {
            string[] searchResult = EditorTools.SearchField(_searchString, _searchFilter, _searchFilters);
            _searchFilter = searchResult[0];
            _searchString = string.IsNullOrEmpty(searchResult[1]) ? _searchFilter : searchResult[1];
        }

        protected override bool MatchesSearch(IStat item, string search)
        {
            return (item.Name.ToLower().Contains(search.ToLower()) ||
                _searchString == _searchFilter || search.ToLower() == item.GetType().Name.ToLower());
        }

        protected override string HasConfigurationErrors(IStat item)
        {
            if (string.IsNullOrEmpty(item.Name))
                return "Name field can't be empty. Please enter a unique name.";

            if (Items.Any(x => !x.Equals(item) && x.Name == item.Name))
                return "Duplicate name. Stat names need to be unique.";

            return string.Empty;
        }

        protected override void Duplicate(IStat item)
        {
            IStat duplicate = (IStat)ScriptableObject.Instantiate(item);
            duplicate.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(duplicate, _target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Items.Add(duplicate);
            EditorUtility.SetDirty(_target);
            Select(duplicate);
        }

        protected override void CreateItem(Type type)
        {
            // 实例化
            IStat item = (IStat)ScriptableObject.CreateInstance(type);
            item.hideFlags = HideFlags.HideInHierarchy;
            StatDatabase database = _target as StatDatabase;
            if (database == null)
                return;
            // 添加这个对象到数据库中
            AssetDatabase.AddObjectToAsset(item, _target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 添加到UI上
            Items.Add(item);
            Select(item);

            EditorUtility.SetDirty(_target);
        }

        protected override void DrawItemLabel(int index, IStat item)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.Name, CollectionEditorStyles.SelectButtonText);
            GUILayout.EndHorizontal();
        }
    }
}
