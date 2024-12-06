using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public abstract class IFlowGraphNode : IGraphNode
    {
        [SerializeField]
        private List<IGraphPort> _ports = new List<IGraphPort>();

        public List<IGraphPort> Ports { get { return _ports; } }
        public List<IGraphPort> InputPorts { get { return _ports.Where(x => x.Direction == ENUM_PortDirection.ePD_Input).ToList(); } }
        public List<IGraphPort> OutputPorts { get { return _ports.Where(x => x.Direction == ENUM_PortDirection.ePD_Output).ToList(); } }

        public IFlowGraphNode() { }
        public abstract object OnRequestValue(IGraphPort port);

        public T GetInputValue<T>(string portName, T defaultValue = default)
        {
            IGraphPort port = GetPort(portName);
            if (port == null || port.Direction == ENUM_PortDirection.ePD_Output)
            {
                throw new ArgumentException(
                    $"[{this.Name}] No input port named `{portName}`"
                );
            }
            return port.GetValue(defaultValue);
        }

        public T GetOutputValue<T>(string portName)
        {
            IGraphPort port = GetPort(portName);
            if (port == null || port.Direction == ENUM_PortDirection.ePD_Input)
            {
                throw new ArgumentException(
                    $"<b>[{this.Name}]</b> No output port named `{portName}`"
                );
            }
            return port.GetValue(default(T));
        }

        public IGraphPort GetPort(string fieldName)
        {
            return _ports.Find((port) => port.FieldName == fieldName);
        }

        public IGraphPort GetPort(int index)
        {
            return _ports[index];
        }

        public void AddPort(IGraphPort port)
        {
            _ports.Add(port);
            port.Node = this;
        }

        public void DisconnectAllPorts()
        {
            foreach (var port in this._ports)
            {
                port.DisconnectAll();
            }
        }

        public override void OnAfterDeserialize()
        {
            for (int i = 0; i < this._ports.Count; i++)
            {
                if (this._ports[i].FieldTypeName == "String")
                    this._ports[i].FieldTypeName = "System.String";

                this._ports[i].Node = this;
                for (var j = 0; j < this._ports[i].Connections.Count; j++)
                {
                    IGraphEdge edge = this._ports[i].Connections[j];
                    IFlowGraphNode connected = this.Graph.Nodes.Find(x => x.Id == edge.NodeId) as IFlowGraphNode;
                    edge.Port = connected.Ports.Find(x => x.FieldName == edge.FieldName);
                    this._ports[i].Connections[j] = edge;
                }
            }
        }
    }
}
