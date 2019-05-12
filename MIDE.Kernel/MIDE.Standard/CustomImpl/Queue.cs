using System;

namespace MIDE.CustomImpl
{
    /// <summary>
    /// Simple queue implementation based on linked list. Enqueue and dequeue operations take O(1), clear operation takes O(n)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Queue<T>
    {
        private Node<T> first;
        private Node<T> last;

        /// <summary>
        /// Length of the queue
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Append new item to queue
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value)
        {
            Length++;
            if (first == null)
            {
                first = new Node<T>(value);
                last = first;
                return;
            }
            last.Next = new Node<T>(value);
            last = last.Next;
        }
        /// <summary>
        /// Clears the queue
        /// </summary>
        public void Clear()
        {
            var node = first;
            while (node != null)
            {
                var next = node.Next;
                node.Next = null;
                node = next;
            }
            first = null;
            last = null;
            Length = 0;
        }
        /// <summary>
        /// Clears the queue by simply removing last and first items' references
        /// </summary>
        public void QuickClear()
        {
            first = null;
            last = null;
            Length = 0;
        }
        /// <summary>
        /// Iterates through queue items and applies the given action to each of them
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            if (Length == 0)
                return;
            var node = first;
            while (node != null)
            {
                action(node.Value);
                var next = node.Next;
                node.Next = null;
                node = next;
                first = node;
                Length--;
            }
            last = null;
        }

        /// <summary>
        /// Returns the value at the beginning of the queue without removing it
        /// Takes the first item in queue but does not deletes it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (first == null)
                throw new InvalidOperationException("Cannot return value from the first item because queue is empty");
            return first.Value;
        }
        /// <summary>
        /// Extract first item from queue
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (first == null)
                throw new InvalidOperationException("Cannot dequeue item from empty queue");
            T value = first.Value;
            var next = first.Next;
            first.Next = null;
            first = next;
            Length--;
            return value;
        }
        /// <summary>
        /// Iterates through queue items and applies the given reduce function
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public T Reduce(Func<T, T, T> func)
        {
            if (Length == 0)
                throw new InvalidOperationException("Cannot apply the reduce function on empty queue");
            var accum = first.Value;
            var node = first.Next;
            first = node;
            while (node != null)
            {
                accum = func(node.Value, accum);
                var next = node.Next;
                node.Next = null;
                node = next;
                first = node;
                Length--;
            }
            last = null;
            return accum;
        }
     
        protected class Node<TValue>
        {
            public TValue Value { get; set; }
            public Node<TValue> Next { get; set; }

            public Node(TValue value)
            {
                Value = value;
            }
        }
    }
}