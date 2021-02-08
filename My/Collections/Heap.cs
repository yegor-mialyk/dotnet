//
// Min/MaxHeap Array Based Implementation
//
// Copyright (C) 1995-2021, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace My.Collections
{
    [Serializable]
    public class Heap<T> : IList<T>, IReadOnlyList<T>
    {
        private const int DefaultCapacity = 8;
        private const uint MaxArrayLength = 0x7feffff;

        private static readonly T[] emptyArray = new T[0];

        // ReSharper disable InconsistentNaming for serialization
        internal T[] _items;
        internal int _size;
        internal readonly HeapType _heapType;

        internal readonly IComparer<T> _comparer;
        // ReSharper enable InconsistentNaming

        public Heap([AllowNull] IComparer<T>? comparer = null, HeapType heapType = HeapType.Min)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _heapType = heapType;
            _items = emptyArray;
        }

        public Heap(int capacity, [AllowNull] IComparer<T>? comparer = null, HeapType heapType = HeapType.Min)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _comparer = comparer ?? Comparer<T>.Default;
            _heapType = heapType;
            _items = capacity == 0 ? emptyArray : new T[capacity];
        }

        public Heap([NotNull] IEnumerable<T> enumerable, [AllowNull] IComparer<T>? comparer = null,
            HeapType heapType = HeapType.Min)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _heapType = heapType;

            switch (enumerable)
            {
                case null:
                    throw new ArgumentNullException(nameof(enumerable));

                case ICollection<T> collection:
                    var count = collection.Count;

                    if (count == 0)
                    {
                        _items = emptyArray;
                        return;
                    }

                    _items = new T[count];
                    collection.CopyTo(_items, 0);
                    _size = count;

                    Heapify();

                    return;
            }

            _items = emptyArray;

            foreach (var item in enumerable)
                Add(item);
        }

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException(nameof(Capacity));

                if (value == _items.Length)
                    return;

                var newItems = new T[value];

                if (_size > 0)
                    Array.Copy(_items, newItems, _size);

                _items = newItems;
            }
        }

        public int Count => _size;

        bool ICollection<T>.IsReadOnly => false;

        public HeapType HeapType => _heapType;

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_size)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _items[index];
            }

            set
            {
                if ((uint)index >= (uint)_size)
                    throw new ArgumentOutOfRangeException(nameof(index));

                _items[index] = value;

                Heapify();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            var size = _size;

            if (size == _items.Length)
                EnsureCapacity(size + 1);

            _items[size] = item;
            _size++;

            ShiftUp(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (_size > 0 && RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                Array.Clear(_items, 0, _size);

            _size = 0;
        }

        public bool Contains(T item)
        {
            return _size != 0 && IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex = 0)
        {
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new HeapEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new HeapEnumerator(this);
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }

        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_size == _items.Length)
                EnsureCapacity(_size + 1);

            if (index < _size)
                Array.Copy(_items, index, _items, index + 1, _size - index);

            _items[index] = item;
            _size++;

            Heapify();
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (index < 0)
                return false;

            RemoveAt(index);

            return true;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            _size--;

            if (index < _size)
                Array.Copy(_items, index + 1, _items, index, _size - index);

            ClearLastElement();

            if (index < _size)
                Heapify();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearLastElement()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                _items[_size] = default!;
        }

        public void AddRange([NotNull] IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case null:
                    throw new ArgumentNullException(nameof(enumerable));
                case ICollection<T> collection:
                    var count = collection.Count;

                    if (count <= 0)
                        return;

                    EnsureCapacity(_size + count);

                    collection.CopyTo(_items, _size);

                    _size += count;

                    break;
                default:
                    foreach (var item in enumerable)
                        Add(item);
                    break;
            }

            Heapify();
        }

        public int BinarySearch(T item)
        {
            return Array.BinarySearch(_items, 0, _size, item, _comparer);
        }

        private void EnsureCapacity(int capacity)
        {
            if (_items.Length >= capacity)
                return;

            var newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;

            if ((uint)newCapacity > MaxArrayLength)
                newCapacity = (int)MaxArrayLength;

            if (newCapacity < capacity)
                newCapacity = capacity;

            Capacity = newCapacity;
        }

        public void Sort()
        {
            if (_heapType == HeapType.Max)
                throw new InvalidOperationException("Max Heap cannot be sorted.");

            if (_size > 1)
                Array.Sort(_items, 0, _size, _comparer);
        }

        public T[] ToArray()
        {
            if (_size == 0)
                return emptyArray;

            T[] array = new T[_size];
            Array.Copy(_items, array, _size);
            return array;
        }

        public void TrimExcess()
        {
            var threshold = (int)(_items.Length * 0.9);

            if (_size < threshold)
                Capacity = _size;
        }

        public Span<T> AsSpan()
        {
            return _items.AsSpan();
        }

        public void Heapify()
        {
            for (var i = _size / 2 - 1; i >= 0; i--)
                ShiftDown(i);
        }

        public T Peek()
        {
            if (_size == 0)
                throw new InvalidOperationException("Heap is empty.");

            return _items[0];
        }

        public T Extract()
        {
            var top = Peek();

            _items[0] = _items[_size];

            ClearLastElement();

            _size--;

            ShiftDown(0);

            return top;
        }

        private void ShiftUp(int index)
        {
            while (true)
            {
                if (index == 0)
                    return;

                var parent = (index - 1) / 2;

                if (Compare(parent, index) <= 0)
                    return;

                var n = _items[parent];
                _items[parent] = _items[index];
                _items[index] = n;

                index = parent;
            }
        }

        private void ShiftDown(int index)
        {
            while (true)
            {
                var parent = index;

                var left = parent * 2 + 1;
                var right = parent * 2 + 2;

                if (left < _size && Compare(left, index) > 0)
                    index = left;

                if (right < _size && Compare(right, index) > 0)
                    index = right;

                if (index == parent)
                    break;

                var n = _items[parent];
                _items[parent] = _items[index];
                _items[index] = n;
            }
        }

        private int Compare(int index1, int index2)
        {
            return _comparer.Compare(_items[index1], _items[index2]) * (int)_heapType;
        }

        public struct HeapEnumerator : IEnumerator<T>
        {
            private readonly Heap<T> _heap;
            private int _index;

            public HeapEnumerator(Heap<T> heap)
            {
                _heap = heap;
                _index = 0;
                Current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if ((uint)_index >= (uint)_heap._size)
                {
                    Current = default!;
                    return false;
                }

                Current = _heap._items[_index];
                _index++;

                return true;
            }

            public T Current { get; private set; }

            object? IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                _index = 0;
                Current = default!;
            }
        }
    }

    public enum HeapType
    {
        Min = 1,
        Max = -1
    }
}
