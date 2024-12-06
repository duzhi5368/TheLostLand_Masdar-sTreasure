using System;
//============================================================
namespace FKLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GraphPortInputAttribute : Attribute
    {
        public readonly bool IsLabel = true;
        public readonly bool IsPort = true;

        public GraphPortInputAttribute() { }
        public GraphPortInputAttribute(bool isLabel, bool isPort)
        {
            this.IsLabel = isLabel;
            this.IsPort = isPort;
        }
    }
}
