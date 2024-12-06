using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public abstract class IGraphNode
    {
        public string Name;
        [HideInInspector]
        public string Id;
        [HideInInspector]
        public Vector2 Position;
        [NonSerialized]
        public IGraph Graph;

        public IGraphNode()
        {
            Id = Guid.NewGuid().ToString();
        }

        public virtual void OnAfterDeserialize() { }
        public virtual void OnBeforeSerialize() { }
    }
}
