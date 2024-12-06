using System;
//============================================================
namespace FKLib
{
    [Serializable]
    public class IGraphEdge
    {
        public string NodeId;
        public string FieldName;

        [NonSerialized]
        public IGraphPort Port;
    }
}
