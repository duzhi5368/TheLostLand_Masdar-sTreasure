//============================================================
namespace FKLib
{
    public interface IGraphPathFindingNode
    {
        // Returns distance to a IGraphNode that is given as parameter. 
        int GetDistance(IGraphPathFindingNode other);
    }
}
