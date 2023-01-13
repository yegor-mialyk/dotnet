//
// Heap Array Based Implementation
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.Collections;
using System.Runtime.CompilerServices;

namespace My.Common.Collections;

[Serializable]
public class Heap<T> : ICollection<T>
{
    private const int DefaultCapacity = 8;
    private const uint MaxArrayLength = 0x7feffff;

    // ReSharper disable InconsistentNaming for serialization
    internal T[] _items;
    internal int _size;

    internal readonly IComparer<T> _comparer;
    // ReSharper enable InconsistentNaming

    public Heap(IComparer<T>? comparer = null)
    {
        _comparer = comparer ?? Comparer<T>.Default;
        _items = Array.Empty<T>();
    }

    public Heap(int capacity, IComparer<T>? comparer = null)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _comparer = comparer ?? Comparer<T>.Default;
        _items = capacity == 0 ? Array.Empty<T>() : new T[capacity];
    }

    public Heap(IEnumerable<T> enumerable, IComparer<T>? comparer = null)
    {
        _comparer = comparer ?? Comparer<T>.Default;

        switch (enumerable)
        {
            case null:
                throw new ArgumentNullException(nameof(enumerable));

            case ICollection<T> collection:
                var count = collection.Count;

                if (count == 0)
                {
                    _items = Array.Empty<T>();
                    return;
                }

                _items = new T[count];
                collection.CopyTo(_items, 0);
                _size = count;

                Heapify();

                return;
        }

        _items = Array.Empty<T>();

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

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _items[index];
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

    public bool Remove(T item)
    {
        var index = IndexOf(item);

        if (index < 0)
            return false;

        _size--;

        if (index < _size)
            Array.Copy(_items, index + 1, _items, index, _size - index);

        ClearLastElement();

        if (index < _size)
            Heapify();

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearLastElement()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            _items[_size] = default!;
    }

    public void AddRange(IEnumerable<T> enumerable)
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

                Heapify();

                break;
            default:
                foreach (var item in enumerable)
                    Add(item);
                break;
        }
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

    public T[] ToArray()
    {
        if (_size == 0)
            return Array.Empty<T>();

        var array = new T[_size];
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

    public T Replace(T item)
    {
        if (_size == 0)
            return item;

        var top = Peek();

        _items[0] = item;

        ShiftDown(0);

        return top;
    }

    protected void Heapify()
    {
        for (var i = _size / 2 - 1; i >= 0; i--)
            ShiftDown(i);
    }

    protected void ShiftUp(int index)
    {
        while (true)
        {
            if (index == 0)
                return;

            var parent = (index - 1) / 2;

            if (Compare(parent, index) <= 0)
                return;

            (_items[parent], _items[index]) = (_items[index], _items[parent]);

            index = parent;
        }
    }

    protected void ShiftDown(int index)
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

            (_items[parent], _items[index]) = (_items[index], _items[parent]);
        }
    }

    protected int Compare(int index1, int index2)
    {
        return _comparer.Compare(_items[index1], _items[index2]);
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
