using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public enum ENUM_PortDirection { 
        ePD_Input = 0, 
        ePD_Output = 1 
    }
    public enum ENUM_PortCapacity { 
        ePC_Single = 0,
        ePC_Multiple = 1 
    }


    [Serializable]
    public class IGraphPort
    {
        [SerializeField]
        public IFlowGraphNode Node;
        [SerializeField]
        public string FieldTypeName;

        public string FieldName;
        public bool IsDrawPort = true;
        public bool IsLabel = true;
        public ENUM_PortDirection Direction = ENUM_PortDirection.ePD_Input;
        public ENUM_PortCapacity Capacity = ENUM_PortCapacity.ePC_Single;

        [SerializeField]
        private List<IGraphEdge> _connections;
        public List<IGraphEdge> Connections
        {
            get { return  _connections; }
        }

        private Type _fieldType;
        public Type FieldType
        {
            get
            {
                if (this._fieldType == null)
                    this._fieldType = Utility.GetType(this.FieldTypeName);
                return this._fieldType;
            }
        }


        public IGraphPort()
        {
            _connections = new List<IGraphEdge>();
        }

        public IGraphPort(IFlowGraphNode node, string fieldName, Type fieldType, 
            ENUM_PortCapacity capacity, ENUM_PortDirection direction)
        {
            _connections = new List<IGraphEdge>();
            this.Node = node;
            this.FieldName = fieldName;
            this.Capacity = capacity;
            this.Direction = direction;
            this.FieldTypeName = fieldType.FullName;
        }

        public virtual T GetValue<T>(T defaultValue = default)
        {
            if (Direction == ENUM_PortDirection.ePD_Input)
            {
                if (Connections.Count > 0)
                {
                    return Connections[0].Port.GetValue<T>();
                }
                return defaultValue;
            }
            object value = Node.OnRequestValue(this);
            if (value == null && typeof(T).IsValueType)
            {
                throw new InvalidCastException(
                    $"Cannot cast null to value type `{typeof(T).FullName}`"
                );
            }
            if (value == null || typeof(T).IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception e)
            {
                throw new InvalidCastException(
                    $"Cannot cast `{value.GetType()}` to `{typeof(T)}`. Error: {e}."
                );
            }
        }

        public virtual IEnumerable<T> GetValues<T>()
        {
            if (Direction == ENUM_PortDirection.ePD_Input)
            {
                if (Connections.Count > 0)
                {
                    for (var i = 0; i < Connections.Count; i++)
                    {
                        yield return Connections[i].Port.GetValue<T>();
                    }
                }
            }
            var values = Node.OnRequestValue(this) as IEnumerable<T>;
            foreach (var value in values)
            {
                yield return value;
            }
        }

        public void Connect(IGraphPort port)
        {
            _connections.Add(new IGraphEdge()
            {
                NodeId = port.Node.Id,
                Port = port,
                FieldName = port.FieldName
            });
            port._connections.Add(new IGraphEdge()
            {
                Port = this,
                NodeId = Node.Id,
                FieldName = FieldName
            });
        }

        public void Disconnect(IGraphPort port)
        {
            this._connections.RemoveAll(x => x.NodeId == port.Node.Id && x.FieldName == port.FieldName);
            port.Connections.RemoveAll(x => x.NodeId == Node.Id && x.FieldName == FieldName);
        }

        public void DisconnectAll()
        {
            for (var i = 0; i < this._connections.Count; i++)
            {
                var port = this._connections[i].Port;
                port._connections.RemoveAll(x => x.NodeId == Node.Id && x.FieldName == FieldName);
            }
            this._connections.Clear();
        }
    }
}
