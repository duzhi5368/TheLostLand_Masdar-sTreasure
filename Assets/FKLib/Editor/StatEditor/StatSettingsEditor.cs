using System;
using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StatSettingsEditor : ScriptableObjectCollectionEditor<ISettings>
    {
        public override string ToolbarName { get { return "Settings"; } }

        protected override bool CanAdd => false;
        protected override bool CanRemove => false;
        protected override bool CanDuplicate => false;

        public StatSettingsEditor(UnityEngine.Object target, List<ISettings> items)
            : base(target, items)
        {
            _target = target;
            _items = items;
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(ISettings).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract).ToArray();
            foreach (Type type in types)
            {
                if (Items.Where(x => x.GetType() == type).FirstOrDefault() == null)
                {
                    CreateItem(type);
                }
            }
        }

        protected override bool MatchesSearch(ISettings item, string search)
        {
            return (item.Name.ToLower().Contains(search.ToLower()) || search.ToLower() == item.GetType().Name.ToLower());
        }

        protected override string ButtonLabel(int index, ISettings item)
        {
            return "  " + GetSidebarLabel(item);
        }
    }
}
