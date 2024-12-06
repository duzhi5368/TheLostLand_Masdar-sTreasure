using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    public abstract class IPathfinding
    {
        // 从图中查找原点到目标节点之间的最短路径
        // Params: edges - 图的边结构
        // Params: originNode - 起始点
        // Params: destinationNode - 目标点
        // Return: 从起始点到目标点的最短路径节点列表；若不存在路径，则返回null
        public abstract IList<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode) where T : IGraphPathFindingNode;

        public abstract Dictionary<T, IList<T>> FindAllPaths<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode) where T : IGraphPathFindingNode;
        // 从图中检索指定节点的相邻节点
        // Params: edges - 图的边结构
        // Params: node - 需要进行检索的节点
        // Return: 相邻节点；若无邻接节点，则返回null
        protected IEnumerable<T> GetNeigbors<T>(Dictionary<T, Dictionary<T, float>> edges, T node) where T : IGraphPathFindingNode
        {
            if (edges.TryGetValue(node, out var neighbours))
            {
                return neighbours.Keys;
            }
            return Enumerable.Empty<T>();
        }
    }
}
