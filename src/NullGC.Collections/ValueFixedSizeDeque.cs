using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Linq;

namespace NullGC.Collections;

public struct ValueFixedSizeDeque<T> : IExplicitOwnership<ValueFixedSizeDeque<T>>, IList<T> where T : unmanaged
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

    /// <summary>
    /// </summary>
    /// <param name="capacity">Must be greater than 0</param>
    /// <param name="allocatorProviderId"></param>
    public ValueFixedSizeDeque(int capacity, AllocatorTypes allocatorProviderId) :
        this(capacity, (int) allocatorProviderId)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="capacity">Must be greater than 0</param>
    /// <param name="allocatorProviderId"></param>
    public ValueFixedSizeDeque(int capacity, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        Guard.IsGreaterThanOrEqualTo(capacity, 0);
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
    /// </summary>
    /// <param name="nth">0-based index start at last item increase backward.</param>
    /// <returns></returns>
    public readonly ref T GetNthItemRefFromTail(int nth)
    {
        Guard.IsInRange(nth, 0, Count);
        return ref _items.GetRefUnchecked((_afterTail - 1 - nth + _items.Length) % _items.Length);
    }

    /// <summary>
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
    /// </summary>
    /// <param name="item"></param>
    /// <param name="evicted">if queue is full, this is the pushed out item.</param>
    /// <returns>
    ///     <value>true</value>
    ///     indicates the queue is full, and there's one item pushed out.
    /// </returns>
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
    /// </summary>
    /// <param name="item"></param>
    /// <param name="evicted">if queue is full, this is the pushed out item.</param>
    /// <returns>
    ///     <value>true</value>
    ///     indicates the queue is full, and there's one item pushed out.
    /// </returns>
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
            for (var i = _head;; i++)
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

    public struct ForwardEnumerator : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>, IAddressFixed
    {
        private readonly ValueArray<T> _items;
        private int _afterTail;
        private int _count;
        private int _index;

        public ForwardEnumerator(ValueArray<T> items, int head, int afterTail)
        {
            _items = items;
            _afterTail = afterTail;
            _index = head - 1;
            _count = (afterTail - head + items.Length) % items.Length;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            var result = (_index + 1) % _items.Length != _afterTail;
            if (result) _index++;
            return result;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

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

        public unsafe T* CurrentPtr => &_items.Items[_index % _items.Length];

        readonly T IEnumerator<T>.Current => _items.GetUnchecked(_index % _items.Length);

        readonly object IEnumerator.Current => _items.GetUnchecked(_index % _items.Length);
        int? IMaybeCountable.Count => _count;
        int? IMaybeCountable.MaxCount => _count;
        public bool SetSkipCount(int count)
        {
            while (count-- > 0 && MoveNext())
            {
                _count--;
            }

            return true;
        }

        public bool SetTakeCount(int count)
        {
            Guard.IsGreaterThanOrEqualTo(count, 0);
            if (count >= _count) return true;
            _afterTail = (_afterTail + _items.Length - (_count - count)) % _items.Length;
            return true;
        }
    }

    public readonly ForwardEnumerator GetEnumerator()
    {
        return new ForwardEnumerator(_items, _head, _afterTail);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return IsEmpty ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return IsEmpty ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        AddBack(item);
    }

    public void Clear()
    {
        _afterTail = _head;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) != -1;
    }

    public readonly void CopyTo(T[] array, int arrayIndex)
    {
        unsafe
        {
            for (var i = _head;; i++)
            {
                var ii = i % _items.Length;
                if (ii == _afterTail)
                    break;
                array[arrayIndex++] = _items.Items[ii];
            }
        }
    }

    public bool Remove(T item)
    {
        return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
    }

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

    private ValueFixedSizeDeque(ValueArray<T> items, int head, int afterTail)
    {
        _items = items;
        _head = head;
        _afterTail = afterTail;
    }

    public ValueFixedSizeDeque<T> Borrow()
    {
        return new ValueFixedSizeDeque<T>(_items.Borrow(), _head, _afterTail);
    }

    public ValueFixedSizeDeque<T> Take()
    {
        return new ValueFixedSizeDeque<T>(_items.Take(), _head, _afterTail);
    }

    public void Dispose()
    {
        _items.Dispose();
    }
}