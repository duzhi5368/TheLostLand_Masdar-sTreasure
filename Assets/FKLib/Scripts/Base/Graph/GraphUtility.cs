using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;
//============================================================
namespace FKLib
{
    public static class GraphUtility
    {
        public static T AddNode<T>(IGraph graph) where T : IGraphNode
        {
            return AddNode(graph, typeof(T)) as T;
        }

        public static IGraphNode AddNode(IGraph graph, System.Type type)
        {
            IGraphNode node = System.Activator.CreateInstance(type) as IGraphNode;
            if (typeof(IFlowGraphNode).IsAssignableFrom(type))
            {
                CreatePorts(node as IFlowGraphNode);
            }
            node.Name = NicifyVariableName(type.Name);
            node.Graph = graph;
            graph.Nodes.Add(node);
            Save(graph);
            return node;
        }

        private static string NicifyVariableName(string name)
        {
            string result = "";
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) == true && i != 0)
                {
                    result += " ";
                }
                result += name[i];
            }
            return result;
        }

        public static void RemoveNodes(IGraph graph, IFlowGraphNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                IFlowGraphNode node = nodes[i];
                node.DisconnectAllPorts();
            }
            graph.Nodes.RemoveAll(x => nodes.Any(y => y == x));
            Save(graph);
        }

        public static void RemoveNodes(IGraph graph, IGraphNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                IGraphNode node = nodes[i];
            }
            graph.Nodes.RemoveAll(x => nodes.Any(y => y == x));
            Save(graph);
        }

        private static void CreatePorts(IFlowGraphNode node)
        {
            FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.HasAttribute<GraphPortInputAttribute>())
                {
                    GraphPortInputAttribute inputAttribute = field.GetCustomAttribute<GraphPortInputAttribute>();
                    IGraphPort port = new IGraphPort(node, field.Name, field.FieldType, 
                        ENUM_PortCapacity.ePC_Single, ENUM_PortDirection.ePD_Input);
                    port.IsDrawPort = inputAttribute.IsPort;
                    port.IsLabel = inputAttribute.IsLabel;
                    node.AddPort(port);
                }
                else if (field.HasAttribute<GraphPortOutputAttribute>())
                {
                    IGraphPort port = new IGraphPort(node, field.Name, field.FieldType, 
                        ENUM_PortCapacity.ePC_Multiple, ENUM_PortDirection.ePD_Output);
                    node.AddPort(port);
                }
            }
        }

        public static void Save(IGraph graph)
        {
            List<IGraphNode> nodes = graph.Nodes;
            Dictionary<string, object> graphData = new Dictionary<string, object>();
            List<UnityEngine.Object> objectReferences = new List<UnityEngine.Object>();
            Dictionary<string, object>[] nodeData = new Dictionary<string, object>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                nodeData[i] = SerializeNode(nodes[i], ref objectReferences);
            }
            graphData.Add("Nodes", nodeData);
            graph.SerializationData = MiniJSON.Serialize(graphData);
            graph.SerializedObjects = objectReferences;
            //Debug.Log(graph.serializationData);
        }

        private static Dictionary<string, object> SerializeNode(IGraphNode node, 
            ref List<UnityEngine.Object> objectReferences)
        {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "Type", node.GetType () },
            };
            SerializeFields(node, ref data, ref objectReferences);
            return data;
        }

        private static void SerializeFields(object obj, ref Dictionary<string, object> dic, 
            ref List<UnityEngine.Object> objectReferences)
        {
            if (obj == null)
            {
                return;
            }
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetAllSerializedFields();

            for (int j = 0; j < fields.Length; j++)
            {
                FieldInfo field = fields[j];
                object value = field.GetValue(obj);
                SerializeValue(field.Name, value, ref dic, ref objectReferences);
            }
        }


        private static void SerializeValue(string key, object value, ref Dictionary<string, object> dic, 
            ref List<UnityEngine.Object> objectReferences)
        {
            if (value != null && !dic.ContainsKey(key))
            {
                Type type = value.GetType();
                if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    UnityEngine.Object unityObject = value as UnityEngine.Object;
                    if (!objectReferences.Contains(unityObject))
                    {
                        objectReferences.Add(unityObject);
                    }
                    dic.Add(key, objectReferences.IndexOf(unityObject));
                }
                else if (typeof(LayerMask).IsAssignableFrom(type))
                {
                    dic.Add(key, ((LayerMask)value).value);
                }
                else if (typeof(Enum).IsAssignableFrom(type))
                {
                    dic.Add(key, (Enum)value);
                }
                else if (type.IsPrimitive ||
                         type == typeof(string) ||
                         type == typeof(Vector2) ||
                         type == typeof(Vector3) ||
                         type == typeof(Vector4) ||
                         type == typeof(Color))
                {
                    dic.Add(key, value);
                }
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    IList list = (IList)value;
                    Dictionary<string, object> s = new Dictionary<string, object>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        SerializeValue(i.ToString(), list[i], ref s, ref objectReferences);
                    }
                    dic.Add(key, s);
                }
                else
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    SerializeFields(value, ref data, ref objectReferences);
                    dic.Add(key, data);
                }
            }
        }

        public static void Load(IGraph graph)
        {
            if (string.IsNullOrEmpty(graph.SerializationData))
            {
                return;
            }
            Dictionary<string, object> data = MiniJSON.Deserialize(graph.SerializationData) as Dictionary<string, object>;
            graph.Nodes.Clear();
            object obj;
            if (data.TryGetValue("Nodes", out obj))
            {
                List<object> list = obj as List<object>;
                for (int i = 0; i < list.Count; i++)
                {
                    IGraphNode node = DeserializeNode(list[i] as Dictionary<string, object>, graph.SerializedObjects);
                    node.Graph = graph;
                    graph.Nodes.Add(node);
                }
                for (int i = 0; i < graph.Nodes.Count; i++)
                {
                    graph.Nodes[i].OnAfterDeserialize();
                }
            }
        }


        private static IGraphNode DeserializeNode(Dictionary<string, object> data, 
            List<UnityEngine.Object> objectReferences)
        {
            string typeString = (string)data["Type"];
            Type type = Utility.GetType(typeString);
            if (type == null && !string.IsNullOrEmpty(typeString))
            {
                type = Utility.GetType(typeString);
            }
            IGraphNode node = (IGraphNode)System.Activator.CreateInstance(type);
            DeserializeFields(node, data, objectReferences);
            return node;
        }

        private static void DeserializeFields(object source, Dictionary<string, object> data, 
            List<UnityEngine.Object> objectReferences)
        {
            if (source == null) { return; }
            Type type = source.GetType();
            FieldInfo[] fields = type.GetAllSerializedFields();

            for (int j = 0; j < fields.Length; j++)
            {
                FieldInfo field = fields[j];
                object value = DeserializeValue(field.Name, source, field, field.FieldType, data, objectReferences);
                if (value != null)
                {
                    field.SetValue(source, value);
                }
            }
        }

        private static object DeserializeValue(string key, object source, FieldInfo field, 
            Type type, Dictionary<string, object> data, List<UnityEngine.Object> objectReferences)
        {
            object value;
            if (data.TryGetValue(key, out value))
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    int index = System.Convert.ToInt32(value);
                    if (index >= 0 && index < objectReferences.Count)
                    {
                        return objectReferences[index];
                    }
                }
                else if (typeof(LayerMask) == type)
                {
                    LayerMask mask = new LayerMask();
                    mask.value = (int)value;
                    return mask;
                }
                else if (typeof(Enum).IsAssignableFrom(type))
                {
                    return Enum.Parse(type, (string)value);
                }
                else if (type.IsPrimitive ||
                         type == typeof(string) ||
                         type == typeof(Vector2) ||
                         type == typeof(Vector3) ||
                         type == typeof(Vector4) ||
                         type == typeof(Quaternion) ||
                         type == typeof(Color))
                {
                    return value;
                }
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    Dictionary<string, object> dic = value as Dictionary<string, object>;

                    Type targetType = typeof(List<>).MakeGenericType(Utility.GetElementType(type));
                    IList result = (IList)Activator.CreateInstance(targetType);
                    int count = dic.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Type elementType = Utility.GetElementType(type);

                        result.Add(DeserializeValue(i.ToString(), source, field, elementType, dic, objectReferences));
                    }

                    if (type.IsArray)
                    {
                        Array array = Array.CreateInstance(Utility.GetElementType(type), count);
                        result.CopyTo(array, 0);
                        return array;
                    }
                    return result;
                }
                else
                {
                    Dictionary<string, object> dic = value as Dictionary<string, object>;
                    if (dic.ContainsKey("m_Type"))
                    {
                        type = Utility.GetType((string)dic["m_Type"]);
                    }
                    object instance = Activator.CreateInstance(type);

                    DeserializeFields(instance, value as Dictionary<string, object>, objectReferences);
                    return instance;
                }
            }
            return null;
        }

        public static object ConvertToArray(this IList collection)
        {
            Type type;
            if (collection.GetType().IsGenericType && collection.GetType().GetGenericArguments().Length == 0)
                type = collection.GetType().GetGenericArguments()[0];
            else if (collection.Count > 0)
                type = collection[0].GetType();
            else
                throw new NotSupportedException("Failed to identify collection type for: " + collection.GetType());

            var array = (object[])Array.CreateInstance(type, collection.Count);
            for (int i = 0; i < array.Length; ++i)
                array[i] = collection[i];
            return array;
        }

        public static object ConvertToArray(this IList collection, Type arrayType)
        {
            var array = (object[])Array.CreateInstance(arrayType, collection.Count);
            for (int i = 0; i < array.Length; ++i)
            {
                var obj = collection[i];
                if (!arrayType.IsInstanceOfType(obj))
                    obj = Convert.ChangeType(obj, arrayType);

                array[i] = obj;
            }
            return array;
        }
    }
}
