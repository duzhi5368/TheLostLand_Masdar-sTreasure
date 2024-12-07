using UnityEngine;
//============================================================
namespace FKLib
{
    public class CompoundAttribute : PropertyAttribute
    {
        public readonly string PropertyPath;
        public CompoundAttribute(string propertyPath)
        {
            PropertyPath = propertyPath;
        }
    }
}
