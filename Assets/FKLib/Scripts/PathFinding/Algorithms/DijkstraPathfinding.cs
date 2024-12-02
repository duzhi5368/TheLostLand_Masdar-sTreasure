using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class DijkstraPathfinding : IPathfinding
    {
        public override Dictionary<T, IList<T>> FindAllPaths<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode)
        {
            IPriorityQueue<T> frontier = new HeapPriorityQueue<T>(edges.Count);
            frontier.Enqueue(originNode, 0);

            Dictionary<T, T> cameFrom = new Dictionary<T, T>(edges.Count);
            cameFrom.Add(originNode, default(T));
            Dictionary<T, float> costSoFar = new Dictionary<T, float>(edges.Count);
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbors(edges, current);
                var currentCost = costSoFar[current];
                var currentEdges = edges[current];

                foreach (var neighbour in neighbours)
                {
                    var newCost = currentCost + currentEdges[neighbour];
                    if (!costSoFar.TryGetValue(neighbour, out var neighbourCost) || newCost < neighbourCost)
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
            }

            Dictionary<T, IList<T>> paths = new Dictionary<T, IList<T>>();
            foreach (T destination in cameFrom.Keys)
            {
                List<T> path = new List<T>();
                var current = destination;
                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.RemoveAt(path.Count - 1);
                paths.Add(destination, path);
            }
            return paths;
        }

        public override IList<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode)
        {
            IPriorityQueue<T> frontier = new SortedListPriorityQueue<T>(edges.Count);
            frontier.Enqueue(originNode, 0);

            Dictionary<T, T> cameFrom = new Dictionary<T, T>(edges.Count);
            cameFrom.Add(originNode, default(T));
            Dictionary<T, float> costSoFar = new Dictionary<T, float>(edges.Count);
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbors(edges, current);
                var currentCost = costSoFar[current];
                var currentEdges = edges[current];

                foreach (var neighbour in neighbours)
                {
                    var newCost = currentCost + currentEdges[neighbour];
                    if (!costSoFar.TryGetValue(neighbour, out var neighbourCost) || newCost < neighbourCost)
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
                if (current.Equals(destinationNode)) break;
            }
            List<T> path = new List<T>();
            if (!cameFrom.ContainsKey(destinationNode))
                return path;

            path.Add(destinationNode);
            var temp = destinationNode;

            while (!cameFrom[temp].Equals(originNode))
            {
                var currentPathElement = cameFrom[temp];
                path.Add(currentPathElement);

                temp = currentPathElement;
            }

            return path;
        }
    }
}
