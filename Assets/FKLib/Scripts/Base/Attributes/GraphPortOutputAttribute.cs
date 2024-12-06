using System;
//============================================================
namespace FKLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GraphPortOutputAttribute : Attribute
    {
        public GraphPortOutputAttribute() { }
    }
}