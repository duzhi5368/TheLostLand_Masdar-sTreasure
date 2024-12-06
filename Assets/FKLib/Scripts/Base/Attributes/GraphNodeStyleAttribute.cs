using System;
//============================================================
namespace FKLib
{
    // 图中的节点属性
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphNodeStyleAttribute : Attribute
    {
        public readonly string iconPath;
        public readonly bool displayHeader;
        public readonly string category;
        public GraphNodeStyleAttribute(bool displayHeader) : this(string.Empty, displayHeader, string.Empty) { }
        public GraphNodeStyleAttribute(bool displayHeader, string category) : this(string.Empty, displayHeader, category) { }
        public GraphNodeStyleAttribute(string iconPath) : this(iconPath, true, string.Empty) { }
        public GraphNodeStyleAttribute(string iconPath, bool displayHeader, string category)
        {
            this.iconPath = iconPath;
            this.displayHeader = displayHeader;
            this.category = category;
        }
    }
}
