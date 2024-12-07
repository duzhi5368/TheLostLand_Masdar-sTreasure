using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class EffectCollectionEditor : ScriptableObjectCollectionEditor<IStatEffect>
    {
        [SerializeField]
        protected List<string> _searchFilters;
        [SerializeField]
        protected string _searchFilter = "All";

        public override string ToolbarName { get { return "Effects"; } }

        public EffectCollectionEditor(UnityEngine.Object target, List<IStatEffect> items, List<string> searchFilters)
            : base(target, items, false)
        {
            _target = target;
            _items = items;
            _searchFilters = searchFilters;
            _searchFilter.Insert(0, "All");
            _searchString = "All";
        }

        protected override void DoSearchGUI()
        {
            string[] searchResult = EditorTools.SearchField(_searchString, _searchFilter, _searchFilters);
            _searchFilter = searchResult[0];
            _searchString = string.IsNullOrEmpty(searchResult[1]) ? _searchFilter : searchResult[1];
        }

        protected override bool MatchesSearch(IStatEffect item, string search)
        {
            return (item.Name.ToLower().Contains(search.ToLower()) ||
                _searchString == _searchFilter || search.ToLower() == item.GetType().Name.ToLower());
        }
    }
}
