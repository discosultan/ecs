using System;
using System.Collections.Generic;

namespace ECS.Utilities
{
    class Pool<T> where T : class
    {
        const int DefaultInitialCapacity = 4;

        int _capacity;
        readonly Queue<T> _queue;
        readonly Func<T> _factory;

        internal Pool(Func<T> factory, int initialCapacity = DefaultInitialCapacity)
        {
            _factory = factory;
            _capacity = initialCapacity;
            _queue = new Queue<T>(initialCapacity);
            EnsureCapacity();
        }

        internal T Fetch()
        {
            if (_queue.Count <= 0)
                EnsureCapacity();
            return _queue.Dequeue();
        }

        internal void Release(T item)
        {
            _queue.Enqueue(item);
        }

        void EnsureCapacity()
        {
            for (int i = 0; i < _capacity; ++i)
                _queue.Enqueue(_factory());
            _capacity *= 2;
        }
    }
}
