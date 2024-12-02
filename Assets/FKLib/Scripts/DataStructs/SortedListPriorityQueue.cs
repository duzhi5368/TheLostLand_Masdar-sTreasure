using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    public class SortedListPriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly SortedList<float, Queue<T>> _queue;

        // Number of items in the queue
        public int Count { get { return _queue.Count; } }

        public SortedListPriorityQueue(int initialCapacity = 0)
        {
            _queue = new SortedList<float, Queue<T>>(initialCapacity);
        }

        // Add item to the queue.
        public void Enqueue(T item, float priority)
        {
            if (!_queue.TryGetValue(priority, out Queue<T> items))
            {
                items = new Queue<T>();
                _queue.Add(priority, items);
            }
            items.Enqueue(item);
        }

        // Return the LOWEST priority item.
        public T Dequeue()
        {
            var pair = _queue.First();
            var item = pair.Value.Dequeue();
            if (pair.Value.Count == 0)
            {
                _queue.RemoveAt(0);
            }
            return item;
        }
    }
}
