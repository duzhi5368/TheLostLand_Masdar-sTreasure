using System;
//============================================================
namespace FKLib
{
    [GraphNodeStyle(true)]
    [Serializable]
    public class FormulaOutputFlowGraphNode : IFlowGraphNode
    {
        [GraphPortInput(false, true)]
        public float Result;

        public override object OnRequestValue(IGraphPort port)
        {
            return GetInputValue("result", Result);
        }
    }
}
