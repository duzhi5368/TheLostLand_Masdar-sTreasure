using UnityEngine;
//============================================================
namespace FKLib
{
    public class InspectorLabelAttribute : PropertyAttribute
    {
        public readonly string Label;
        public readonly string Tooltip;

        public InspectorLabelAttribute(string label) : this(label, string.Empty)
        {
        }

        public InspectorLabelAttribute(string label, string tooltip)
        {
            this.Label = label;
            this.Tooltip = tooltip;
        }
    }
}
