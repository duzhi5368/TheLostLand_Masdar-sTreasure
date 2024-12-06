using UnityEngine;
//============================================================
namespace FKLib
{
    public class DebugInfo
    {
        public string Metadata { get; set; }
        public Color Color { get; set; }

        public DebugInfo(string metadata, Color color)
        {
            Color = color;
            Metadata = metadata;
        }
    }
}
