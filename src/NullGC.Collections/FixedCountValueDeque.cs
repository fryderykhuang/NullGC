using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;

namespace NullGC.Collections;

public struct FixedCountValueDeque<T> : IDisposable, IList<T> where T : unmanaged
{
    private ValueArray<T> _items;
    private int _head = 0;
    private int _afterTail = 0;

    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _head == _afterTail;
    }

    public readonly bool IsFull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items.Length == 0 || (_afterTail + 1) % _items.Length == _head;
    }

    public FixedCountValueDeque(int capacity, int allocatorProviderId = (int) AllocatorProviderIds.Default)
    {
        _items = new ValueArray<T>(capacity + 1, allocatorProviderId);
    }

    public readonly ref T TailRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (IsEmpty) ThrowHelper.CollectionIsEmpty();
            unsafe
            {
                return ref _items.Items[(_afterTail - 1 + _items.Length) % _items.Length];
            }
        }
    }

    public readonly ref T TailRefOrNullRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (IsEmpty) return ref Unsafe.NullRef<T>();
            unsafe
            {
                return ref _items.Items[(_afterTail - 1 + _items.Length) % _items.Length];
            }
        }
    }

    public readonly ref T HeadRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_head == _afterTail) ThrowHelper.CollectionIsEmpty();
            return ref _items.GetRefUnchecked(_head);
        }
    }

    // public readonly T Head
    // {
    //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //     get
    //     {
    //         if (_head == _afterTail) ThrowHelper.CollectionIsEmpty();
    //         return _items.GetUnchecked(_head);
    //     }
    // }
    //


    public readonly ref T HeadRefOrNullRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (IsEmpty) return ref Unsafe.NullRef<T>();
            unsafe
            {
                return ref _items.Items[_head];
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="nth">0-based index start at last item increase backward.</param>
    /// <returns></returns>
    public readonly ref T GetNthItemRefFromTail(int nth)
    {
        Guard.IsInRange(nth, 0, Count);
        return ref _items.GetRefUnchecked((_afterTail - 1 - nth + _items.Length) % _items.Length);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="nth">0-based index start at first item increase forward.</param>
    /// <returns></returns>
    public readonly ref T GetNthItemRefFromHead(int nth)
    {
        Guard.IsInRange(nth, 0, Count);
        return ref _items.GetRefUnchecked((_head + nth) % _items.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFront(T item)
    {
        if (IsFull) ThrowHelper.CollectionIsFull();

        _head = (_head - 1 + _items.Length) % _items.Length;
        _items.GetRefUnchecked(_head) = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddBack(T item)
    {
        if (IsFull) ThrowHelper.CollectionIsFull();
        _items.GetRefUnchecked(_afterTail) = item;
        _afterTail = (_afterTail + 1) % _items.Length;
    }

    public void RemoveBack(out T removed)
    {
        if (IsEmpty) ThrowHelper.CollectionIsEmpty();
        var ii = (_afterTail - 1 + _items.Length) % _items.Length;
        var ret = _items.GetUnchecked(ii);
        _afterTail = ii;
        removed = ret;
    }

    public void RemoveFront(out T removed)
    {
        if (IsEmpty) ThrowHelper.CollectionIsEmpty();
        var ret = _items.GetUnchecked(_head);
        _head = (_head + 1) % _items.Length;
        removed = ret;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="evicted">if queue is full, this is the pushed out item.</param>
    /// <returns><value>true</value> indicates the queue is full, and there's one item pushed out.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PushBack(T item, out T evicted)
    {
        if (IsFull)
        {
            var ret = _items.GetUnchecked(_head);
            _head = (_head + 1) % _items.Length;
            _items.GetRefUnchecked(_afterTail) = item;
            _afterTail = (_afterTail + 1) % _items.Length;
            evicted = ret;
            return true;
        }

        _items.GetRefUnchecked(_afterTail) = item;
        _afterTail = (_afterTail + 1) % _items.Length;
        evicted = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="evicted">if queue is full, this is the pushed out item.</param>
    /// <returns><value>true</value> indicates the queue is full, and there's one item pushed out.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PushFront(T item, out T evicted)
    {
        if (IsFull)
        {
            var ii = (_afterTail - 1 + _items.Length) % _items.Length;
            var ret = _items.GetUnchecked(ii);
            _afterTail = ii;
            _head = (_head - 1 + _items.Length) % _items.Length;
            _items.GetRefUnchecked(_head) = item;
            evicted = ret;
            return true;
        }

        _head = (_head - 1 + _items.Length) % _items.Length;
        _items.GetRefUnchecked(_head) = item;
        evicted = default;
        return false;
    }

    public readonly int IndexOf(T item)
    {
        unsafe
        {
            for (int i = _head;; i++)
            {
                var ii = i % _items.Length;
                if (ii == _afterTail) break;
                if (EqualityComparer<T>.Default.Equals(_items.Items[ii], item)) return i;
            }

            return -1;
        }
    }

    public void Insert(int index, T item)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    public void RemoveAt(int index)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    T IList<T>.this[int index]
    {
        readonly get => this[index];
        set => this[index] = value;
    }

    public readonly ref T this[int index]
    {
        get
        {
            unsafe
            {
                Guard.IsInRange(index, 0, Count);
                return ref _items.Items[(_head + index) % _items.Length];
            }
        }
    }

    public struct ForwardEnumerator : IEnumerator<T>
    {
        private readonly ValueArray<T> _items;
        private readonly int _afterTail;
        private int _index;

        public ForwardEnumerator(ValueArray<T> items, int head, int afterTail)
        {
            _items = items;
            _afterTail = afterTail;
            _index = head - 1;
        }

        public void Dispose()
        {
        }

        public bool MoveNext() => ++_index % _items.Length != _afterTail;

        public void Reset() => throw new NotSupportedException();

        public readonly ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                unsafe
                {
                    return ref _items.Items[_index % _items.Length];
                }
            }
        }

        readonly T IEnumerator<T>.Current => Current;

        readonly object IEnumerator.Current => Current;
    }

    public readonly ForwardEnumerator GetEnumerator() => new(_items, _head, _afterTail);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => IsEmpty ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => IsEmpty ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item) => AddBack(item);

    public void Clear() => _afterTail = _head;

    public bool Contains(T item) => IndexOf(item) != -1;

    public readonly void CopyTo(T[] array, int arrayIndex)
    {
        unsafe
        {
            for (int i = _head; ; i++)
            {
                var ii = i % _items.Length;
                if (ii == _afterTail)
                    break;
                array[arrayIndex++] = _items.Items[ii];
            }
        }
    }

    public bool Remove(T item) => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();

    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Math.Max(0, _items.Length - 1);
    }

    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_afterTail - _head + _items.Length) % _items.Length;
    }

    public readonly bool IsReadOnly => false;

    public void Dispose() => _items.Dispose();
}