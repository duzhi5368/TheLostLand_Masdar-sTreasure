using System;
//============================================================
namespace FKLib
{
    public class IconAttribute : Attribute
    {
        public readonly Type Type;
        public readonly string Path;

        public IconAttribute(Type type)
        {
            Type = type;
        }

        public IconAttribute(string path)
        {
            Path = path;
        }
    }
}
