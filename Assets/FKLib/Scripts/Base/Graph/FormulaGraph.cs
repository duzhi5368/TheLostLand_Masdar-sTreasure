using System;
//============================================================
namespace FKLib
{
    [Serializable]
    public class FormulaGraph : IGraph
    {
        public FormulaGraph()
        {
            GraphUtility.AddNode(this, typeof(FormulaOutputFlowGraphNode));
        }

        public static implicit operator float(FormulaGraph graph) 
        {
            FormulaOutputFlowGraphNode output = graph.Nodes.Find(x => x.GetType() == typeof(FormulaOutputFlowGraphNode)) as FormulaOutputFlowGraphNode;
            return output.GetInputValue<float>("result", output.Result);
        }
    }
}
