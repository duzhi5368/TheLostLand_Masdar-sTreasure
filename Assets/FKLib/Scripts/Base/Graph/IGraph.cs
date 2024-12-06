using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class IGraph : ISerializationCallbackReceiver
    {
        public string SerializationData;

        [HideInInspector]
        public List<UnityEngine.Object> SerializedObjects = new List<UnityEngine.Object>();

        [NonSerialized]
        public List<IGraphNode> Nodes = new List<IGraphNode>();

        public List<T> FindNodeOfType<T>() where T : IGraphNode
        {
            return Nodes.Where(x => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToList();
        }

        public void OnAfterDeserialize() {
            GraphUtility.Save(this);
        }
        public void OnBeforeSerialize() {
            GraphUtility.Load(this);
        }
    }
}
