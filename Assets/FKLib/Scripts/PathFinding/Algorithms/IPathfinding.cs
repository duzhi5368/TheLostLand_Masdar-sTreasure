using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    public abstract class IPathfinding
    {
        // ��ͼ�в���ԭ�㵽Ŀ��ڵ�֮������·��
        // Params: edges - ͼ�ı߽ṹ
        // Params: originNode - ��ʼ��
        // Params: destinationNode - Ŀ���
        // Return: ����ʼ�㵽Ŀ�������·���ڵ��б���������·�����򷵻�null
        public abstract IList<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode) where T : IGraphPathFindingNode;

        public abstract Dictionary<T, IList<T>> FindAllPaths<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode) where T : IGraphPathFindingNode;
        // ��ͼ�м���ָ���ڵ�����ڽڵ�
        // Params: edges - ͼ�ı߽ṹ
        // Params: node - ��Ҫ���м����Ľڵ�
        // Return: ���ڽڵ㣻�����ڽӽڵ㣬�򷵻�null
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
