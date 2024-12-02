using System.Collections.Generic;
//============================================================
namespace FKLib
{
    // A priority queue based on binary heap.
    public class HeapPriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly List<(float priority, T item)> _queue;

        public HeapPriorityQueue(int initialCapacity = 0)
        {
            _queue = new List<(float priority, T item)> (initialCapacity);
        }

        // Number of items in the queue
        public int Count { get { return _queue.Count; } }

        // Return the LOWEST priority item.
        public T Dequeue()
        {
            int lastIndex = _queue.Count - 1;
            var frontItem = _queue[0].item;
            _queue[0] = _queue[lastIndex];
            _queue.RemoveAt(lastIndex);

            --lastIndex;
            int parentIndex = 0;
            while (true)
            {
                int leftChildIndex = parentIndex * 2 + 1;
                if (leftChildIndex > lastIndex) 
                    break;
                int rightChildIndex = leftChildIndex + 1;
                if (rightChildIndex <= lastIndex && _queue[rightChildIndex].priority < _queue[leftChildIndex].priority)
                    leftChildIndex = rightChildIndex;
                if (_queue[parentIndex].priority <= _queue[leftChildIndex].priority) 
                    break;
                (_queue[leftChildIndex], _queue[parentIndex]) = (_queue[parentIndex], _queue[leftChildIndex]);
                parentIndex = leftChildIndex;
            }
            return frontItem;
        }

        // Add item to the queue.
        public void Enqueue(T item, float priority)
        {
            _queue.Add((priority, item));
            int childIndex = _queue.Count - 1;
            while (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / 2;
                if (_queue[childIndex].priority >= _queue[parentIndex].priority)
                    break;
                (_queue[parentIndex], _queue[childIndex]) = (_queue[childIndex], _queue[parentIndex]);
                childIndex = parentIndex;
            }
        }
    }
}
