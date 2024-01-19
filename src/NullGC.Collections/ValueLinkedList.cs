using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Linq;

namespace NullGC.Collections;

public static class ValueLinkedList
{
    public const int FirstPosition = -1;
    public const int LastPosition = -2;
}

/// <summary>
/// Linked list implemented by array using array index as pointer, the node index is fixed once allocated for an added node. 
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("Count = {Count}")]
public struct ValueLinkedList<T> : IUnsafeArray<ValueLinkedList<T>.Node>, ISingleDisposable<ValueLinkedList<T>>,
    ILinqEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator>, IDisposable where T : unmanaged
{
    public struct ForwardEnumerator : ILinqRefEnumerator<Node>, ILinqValueEnumerator<Node>, IItemAddressFixed
    {
        private readonly ValueArray<Node> _items;
        private int _count;
        private int _index;
        private int _nextIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ForwardEnumerator(ValueArray<Node> items, int count, int head)
        {
            _items = items;
            _nextIndex = head;
            _count = count;
            _index = -1;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_nextIndex < 0 || _count <= 0) return false;
            _index = _nextIndex;
            _nextIndex = _items.GetRefUnchecked(_index).Next;
            _count--;
            return true;
        }

        public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

        public ref Node Current => ref _items.GetRefUnchecked(_index);
        Node IEnumerator<Node>.Current => _items.GetUnchecked(_index);
        public unsafe Node* CurrentPtr => &_items.Items[_index];

        object IEnumerator.Current => _items.GetUnchecked(_index);

        public int? Count => _count;
        public int? MaxCount => _count;

        public bool SetSkipCount(int count)
        {
            for (var i = 0; i < count && MoveNext(); i++)
            {
            }

            return true;
        }

        public bool SetTakeCount(int count)
        {
            _count = Math.Min(_count, count);
            return true;
        }
    }

    public struct Node
    {
        public int Index;
        public int Previous;
        public int Next;
        public T Value;
    }

    private const int FreeListBaseIndex = -3;
    
    private ValueArray<Node> _items;
    private int _head = -1;
    private int _tail = -1;
    private int _endIndex;
    private int _freeHead = -1;
    private int _freeCount;

    private ValueLinkedList(ValueArray<Node> items, int head, int tail, int endIndex, int freeHead, int freeCount)
    {
        _items = items;
        _head = head;
        _tail = tail;
        _endIndex = endIndex;
        _freeHead = freeHead;
        _freeCount = freeCount;
    }

    public ValueLinkedList() => _items = new ValueArray<Node>(0);

    public ValueLinkedList(int capacity, int allocatorProviderId = (int) AllocatorProviderIds.Default) =>
        _items = new ValueArray<Node>(capacity, allocatorProviderId);

    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (_items.Length == value) return;
            Guard.IsGreaterThanOrEqualTo(value, _endIndex, nameof(value));
            _items.Grow(value, value, true);
        }
    }

    IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => Count == 0 ? GenericEmptyEnumerator<Node>.Instance : GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ForwardEnumerator GetEnumerator() => new(_items, Count, _head);

    public int Count => _endIndex - _freeCount;
    int? IMaybeCountable.MaxCount => Count;

    public void MoveToFirst(int index) => Move(index, ValueLinkedList.FirstPosition);
    public void MoveToLast(int index) => Move(index, ValueLinkedList.LastPosition);

    /// <summary>
    ///     Move source node before dest node
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"><see cref="ValueLinkedList.LastPosition" />(-1) means move to tail.</param>
    public void Move(int srcIndex, int destIndex)
    {
        var count = Count;
        Guard.IsFalse(count == 0 && (srcIndex >= 0 || destIndex >= -1), $"{nameof(srcIndex)}/{nameof(destIndex)}",
            "List is empty.");
        Guard.IsInRange(srcIndex, 0, count);
        Guard.IsInRange(destIndex, -2, count);

        if (srcIndex == destIndex)
            return;

        ref var srcNode = ref _items.GetRefUnchecked(srcIndex);
        Debug.Assert(srcNode is {Previous: >= -1, Next: >= -1}, "Free node cannot be moved.");
        int srcPrev = srcNode.Previous, srcNext = srcNode.Next;
        switch (destIndex)
        {
            case ValueLinkedList.LastPosition when srcIndex == _tail:
                return;
            case ValueLinkedList.LastPosition:
                _items.GetRefUnchecked(_tail).Next = srcIndex;
                srcNode.Previous = _tail;
                srcNode.Next = -1;
                _tail = srcIndex;
                break;
            case ValueLinkedList.FirstPosition when srcIndex == _head:
                return;
            case ValueLinkedList.FirstPosition:
                _items.GetRefUnchecked(_head).Previous = srcIndex;
                srcNode.Next = _head;
                srcNode.Previous = -1;
                _head = srcIndex;
                break;
            default:
            {
                ref var destNode = ref _items.GetRefUnchecked(destIndex);
                Debug.Assert(destNode is {Previous: >= -1, Next: >= -1}, nameof(destIndex),
                    "Free node cannot be moved.");
                srcNode.Previous = destNode.Previous;
                srcNode.Next = destIndex;
                if (destNode.Previous == -1)
                    _head = srcIndex;
                else
                    _items.GetRefUnchecked(destNode.Previous).Next = srcIndex;
                destNode.Previous = srcIndex;
                break;
            }
        }


        if (srcPrev >= 0)
        {
            _items.GetRefUnchecked(srcPrev).Next = srcNext;
        }
        else
        {
            Debug.Assert(_head == srcIndex);
            _head = srcNext;
        }

        if (srcNext >= 0)
        {
            _items.GetRefUnchecked(srcNext).Previous = srcPrev;
        }
        else
        {
            Debug.Assert(_tail == srcIndex);
            _tail = srcPrev;
        }
    }

    // public void Swap(int srcIndex, int destIndex)
    // {
    //     var count = Count;
    //     Debug.Assert(count >= 0);
    //     Guard.IsFalse(count == 0 && (srcIndex >= 0 || destIndex >= 0), $"{nameof(srcIndex)}/{nameof(destIndex)}",
    //         "List is empty.");
    //     Guard.IsInRange(srcIndex, 0, count, nameof(srcIndex));
    //     Guard.IsInRange(destIndex, 0, count, nameof(destIndex));
    //
    //     ref var srcNode = ref _items[srcIndex];
    //     Guard.IsFalse(srcNode.Previous < -1 || srcNode.Next < -1, nameof(srcIndex), "Free node cannot be moved.");
    //     ref var destNode = ref _items[destIndex];
    //     Guard.IsFalse(destNode.Previous < -1 || destNode.Next < -1, nameof(destIndex), "Free node cannot be moved.");
    //
    //     var srcPrev = srcNode.Previous;
    //     var srcNext = srcNode.Next;
    //     srcNode.Previous = destNode.Previous;
    //     srcNode.Next = destNode.Next;
    //     destNode.Previous = srcPrev;
    //     destNode.Next = srcNext;
    //     if (srcIndex == _head)
    //         _head = destIndex;
    //     else if (srcIndex == _tail) _tail = destIndex;
    //
    //     if (destIndex == _head)
    //         _head = srcIndex;
    //     else if (destIndex == _tail) _tail = srcIndex;
    // }

    public void AddFront(in T value)
    {
        int index;
        if (_freeCount > 0)
        {
            index = _freeHead;
            _freeHead = FreeListBaseIndex - _items.GetRefUnchecked(_freeHead).Next;
            _freeCount--;
        }
        else
        {
            if (_endIndex == _items.Length) Resize();

            index = _endIndex;
            _endIndex++;
        }

        ref var node = ref _items.GetRefUnchecked(index);
        node.Index = index;
        node.Previous = -1;
        node.Next = _head;
        node.Value = value;
        if (_head >= 0) _items.GetRefUnchecked(_head).Previous = index;
        _head = index;
        if (_tail < 0) _tail = index;
    }

    public void AddBack(in T value)
    {
        int index;
        if (_freeCount > 0)
        {
            index = _freeHead;
            _freeHead = FreeListBaseIndex - _items.GetRefUnchecked(_freeHead).Next;
            _freeCount--;
        }
        else
        {
            if (_endIndex == _items.Length) Resize();

            index = _endIndex;
            _endIndex++;
        }

        ref var node = ref _items.GetRefUnchecked(index);
        node.Index = index;
        node.Previous = _tail;
        node.Next = -1;
        if (_tail >= 0) _items.GetRefUnchecked(_tail).Next = index;
        node.Value = value;
        _tail = index;
        if (_head < 0) _head = index;
    }

    public ref Node HeadRefOrNullRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_head < 0) return ref Unsafe.NullRef<Node>();
            return ref _items.GetRefUnchecked(_head);
        }
    }

    public ref Node HeadRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_head < 0) CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException();
            return ref _items.GetRefUnchecked(_head);
        }
    }

    public ref Node HeadRefUnchecked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _items.GetRefUnchecked(_head);
    }

    public ref Node TailRefOrNullRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_tail < 0) return ref Unsafe.NullRef<Node>();
            return ref _items.GetRefUnchecked(_tail);
        }
    }

    public ref Node TailRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_tail < 0) CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException();
            return ref _items.GetRefUnchecked(_tail);
        }
    }

    public ref Node TailRefUnchecked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _items.GetRefUnchecked(_tail);
    }
    
    public void Remove(int index)
    {
        unsafe
        {
            Guard.IsInRange(index, 0, _endIndex);
            var items = _items.Items;
            ref var node = ref items[index];
            var next = node.Next;
            var prev = node.Previous;
            if (_freeCount > 0)
            {
                items[_freeHead].Previous = FreeListBaseIndex - index;
                node.Next = FreeListBaseIndex - _freeHead;
                _freeHead = index;
            }
            else
            {
                _freeHead = index;
                node.Next = -2;
                node.Previous = -2;
            }

            _freeCount++;
            if (next >= 0) items[next].Previous = prev;
            if (prev >= 0) items[prev].Next = next;
            if (_head == index) _head = next;
            if (_tail == index) _tail = prev;
        }
    }

    public void RemoveFront()
    {
        if (_head < 0)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("List is empty.");
        ref var node = ref _items.GetRefUnchecked(_head);
        var next = node.Next;
        if (_freeCount > 0)
        {
            _items.GetRefUnchecked(_freeHead).Previous = FreeListBaseIndex - _head;
            node.Next = FreeListBaseIndex - _freeHead;
            _freeHead = _head;
        }
        else
        {
            _freeHead = _head;
            node.Next = -2;
            node.Previous = -2;
        }

        _freeCount++;
        if (next >= 0) _items.GetRefUnchecked(next).Previous = -1;
        if (_tail == _head) _tail = next;
        _head = next;
    }

    public void RemoveBack()
    {
        if (_tail < 0)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("List is empty.");
        ref var node = ref _items.GetRefUnchecked(_tail);
        var previous = node.Previous;
        if (_freeCount > 0)
        {
            _items.GetRefUnchecked(_freeHead).Previous = FreeListBaseIndex - _tail;
            node.Next = FreeListBaseIndex - _freeHead;
            _freeHead = _tail;
        }
        else
        {
            _freeHead = _tail;
            node.Next = -2;
            node.Previous = -2;
        }

        _freeCount++;
        if (previous >= 0) _items.GetRefUnchecked(previous).Next = -1;
        if (_head == _tail) _head = previous;
        _tail = previous;
    }


    private void Resize()
    {
        var newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;
        if ((uint) newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;
        _items.Grow(_items.Length + ((newCapacity + 1 - _items.Length) >> 1), newCapacity, true);
    }

    private const int DefaultCapacity = 4; 

    public unsafe Node* Items => _items.Items;
    int IUnsafeArray<Node>.Length => _endIndex;
    public bool IsInitialized => _items.IsInitialized;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueLinkedList<T> Borrow()
    {
        return new ValueLinkedList<T>(_items.Borrow(), _head, _tail, _endIndex, _freeHead, _freeCount);
    }

    public void Dispose() => _items.Dispose();

    /// <summary>
    /// Get the ref to the Node with index <paramref name="index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Node GetNodeRef(int index)
    {
        if (index < 0 || index >= _endIndex)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
        ref var ret = ref _items.GetRefUnchecked(index);
        if (ret is {Previous: < -1, Next: < -1})
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(nameof(index), "Invalid node index.");
        return ref ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Node GetNodeRefUnchecked(int index)
    {
        Debug.Assert(index >= 0 && index < _endIndex);
        return ref _items.GetRefUnchecked(index);
    }

    /// <summary>
    /// </summary>
    /// <param name="nth">Zero based position starts at first node.</param>
    /// <returns></returns>
    public ref Node GetNthNodeRefFromHead(int nth)
    {
        Guard.IsInRange(nth, 0, Count);
        var p = _head;
        for (var i = 0; i < nth; i++) p = _items.GetRefUnchecked(p).Next;

        return ref _items.GetRefUnchecked(p);
    }

    /// <summary>
    /// </summary>
    /// <param name="nth">Zero based position starts at last node.</param>
    /// <returns></returns>
    public ref Node GetNthNodeRefFromTail(int nth)
    {
        Guard.IsInRange(nth, 0, Count);
        var p = _tail;
        for (var i = 0; i < nth; i++) p = _items.GetRefUnchecked(p).Previous;

        return ref _items.GetRefUnchecked(p);
    }

    int? IMaybeCountable.Count => Count;

    IEnumerator IEnumerable.GetEnumerator() => Count == 0 ? GenericEmptyEnumerator<Node>.Instance : GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFirst(int index)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0);
        return _head == index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLast(int index)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0);
        return _tail == index;
    }
}