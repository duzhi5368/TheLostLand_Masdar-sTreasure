using System;
//============================================================
namespace FKLib
{
    // A priority queue.
    public interface IPriorityQueue<T>
    {
        // Number of items in the queue
        int Count { get; }

        // Add item to the queue.
        void Enqueue(T item, float priority);
        // Return the LOWEST priority item.
        T Dequeue();
    }

    // A node in a priority queue.
    readonly struct PriorityQueueNode<T> : IComparable<PriorityQueueNode<T>>
    {
        public readonly T Item;
        public readonly float Priority;

        public PriorityQueueNode(T item, float priority)
        {
            this.Item = item;
            this.Priority = priority;
        }
        public readonly int CompareTo(PriorityQueueNode<T> other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}
