using UnityEngine;
//============================================================
namespace FKLib
{
    public class HeaderLineAttribute : PropertyAttribute
    {
        public readonly string Header;
        public HeaderLineAttribute(string header)
        {
            Header = header;
        }
    }
}
