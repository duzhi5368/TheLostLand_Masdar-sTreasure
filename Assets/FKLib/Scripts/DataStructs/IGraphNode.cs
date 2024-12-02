//============================================================
namespace FKLib
{
    public interface IGraphNode
    {
        // Returns distance to a IGraphNode that is given as parameter. 
        int GetDistance(IGraphNode other);
    }
}
