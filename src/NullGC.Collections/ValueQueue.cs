// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Linq;

namespace NullGC.Collections;

// A simple Queue of generic objects.  Internally it is implemented as a
// circular buffer, so Enqueue can be O(n).  Dequeue is O(1).
[DebuggerDisplay("Count = {Count}, IsAllocated = {IsAllocated}")]
public struct ValueQueue<T> : IHasUnmanagedResource, ISingleDisposable<ValueQueue<T>>,
    ILinqEnumerable<T, ValueQueue<T>.Enumerator> where T : unmanaged
{
    private ValueArray<T> _array;
    private int _head; // The index from which to dequeue if the queue isn't empty.
    private int _tail; // The index at which to enqueue if the queue isn't full.
    private int _size; // Number of elements.

    private ValueQueue(ValueArray<T> array, int head, int tail, int size)
    {
        _array = array;
        _head = head;
        _tail = tail;
        _size = size;
    }

    // Creates a queue with room for capacity objects. The default initial
    // capacity and grow factor are used.
    public ValueQueue()
    {
        _array = new ValueArray<T>(0, (int) AllocatorTypes.Default);
    }

    public ValueQueue(AllocatorTypes allocatorProviderId)
    {
        _array = new ValueArray<T>(0, (int) allocatorProviderId);
    }

    public ValueQueue(int capacity, AllocatorTypes allocatorProviderId) : this(capacity, (int) allocatorProviderId)
    {
    }

    // Creates a queue with room for capacity objects. The default grow factor
    // is used.
    public ValueQueue(int capacity, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "ArgumentOutOfRange_NeedNonNegNum");
        _array = new ValueArray<T>(capacity, allocatorProviderId);
    }

    // Fills a Queue with the elements of an ICollection.  Uses the enumerator
    // to get each of the elements.
    public ValueQueue(IEnumerable<T> collection, int allocatorProviderId)
    {
        ArgumentNullException.ThrowIfNull(collection);

        _array = collection.ToValueArray(allocatorProviderId);
        if (_size != _array.Length) _tail = _size;
    }

    int? IMaybeCountable.Count => _size;
    int? IMaybeCountable.MaxCount => _size;

    public int Count => _size;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    // Removes all Objects from the queue.
    public void Clear()
    {
        if (_size != 0)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                if (_head < _tail)
                {
                    _array.Clear(_head, _size);
                }
                else
                {
                    _array.Clear(_head, _array.Length - _head);
                    _array.Clear(0, _tail);
                }
            }

            _size = 0;
        }

        _head = 0;
        _tail = 0;
    }

    // CopyTo copies a collection into an Array, starting at a particular
    // index into the array.
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex,
                "ArgumentOutOfRange_IndexMustBeLessOrEqual");

        if (array.Length - arrayIndex < _size) throw new ArgumentException("Argument_InvalidOffLen");

        var numToCopy = _size;
        if (numToCopy == 0) return;

        var firstPart = Math.Min(_array.Length - _head, numToCopy);
        _array.Copy(_head, array, arrayIndex, firstPart);
        numToCopy -= firstPart;
        if (numToCopy > 0) _array.Copy(0, array, arrayIndex + _array.Length - _head, numToCopy);
    }

    // public void CopyTo(Array array, int index)
    // {
    //     ArgumentNullException.ThrowIfNull(array);
    //
    //     if (array.Rank != 1) throw new ArgumentException("Arg_RankMultiDimNotSupported", nameof(array));
    //
    //     if (array.GetLowerBound(0) != 0) throw new ArgumentException("Arg_NonZeroLowerBound", nameof(array));
    //
    //     var arrayLen = array.Length;
    //     if (index < 0 || index > arrayLen)
    //         throw new ArgumentOutOfRangeException(nameof(index), index, "ArgumentOutOfRange_IndexMustBeLessOrEqual");
    //
    //     if (arrayLen - index < _size) throw new ArgumentException("Argument_InvalidOffLen");
    //
    //     var numToCopy = _size;
    //     if (numToCopy == 0) return;
    //
    //     try
    //     {
    //         var firstPart = _array.Length - _head < numToCopy ? _array.Length - _head : numToCopy;
    //         Array.Copy(_array, _head, array, index, firstPart);
    //         numToCopy -= firstPart;
    //
    //         if (numToCopy > 0) Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
    //     }
    //     catch (ArrayTypeMismatchException)
    //     {
    //         throw new ArgumentException("Argument_InvalidArrayType", nameof(array));
    //     }
    // }

    // Adds item to the tail of the queue.
    public void Enqueue(T item)
    {
        if (_size == _array.Length) Grow(_size + 1);

        _array[_tail] = item;
        MoveNext(ref _tail);
        _size++;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _size == 0 ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _size == 0 ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();
    }

    // GetEnumerator returns an IEnumerator over this Queue.  This
    // Enumerator will support removing.
    public Enumerator GetEnumerator()
    {
        return new Enumerator(_array, _size, _head);
    }

    // Removes the object at the head of the queue and returns it. If the queue
    // is empty, this method throws an
    // InvalidOperationException.
    public T Dequeue()
    {
        var head = _head;
        var array = _array;

        if (_size == 0) ThrowForEmptyQueue();

        var removed = array[head];
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) array[head] = default!;
        MoveNext(ref _head);
        _size--;
        return removed;
    }

    public bool TryDequeue([MaybeNullWhen(false)] out T result)
    {
        var head = _head;
        var array = _array;

        if (_size == 0)
        {
            result = default;
            return false;
        }

        result = array[head];
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) array[head] = default!;
        MoveNext(ref _head);
        _size--;
        return true;
    }

    // Returns the object at the head of the queue. The object remains in the
    // queue. If the queue is empty, this method throws an
    // InvalidOperationException.
    public T Peek()
    {
        if (_size == 0) ThrowForEmptyQueue();

        return _array[_head];
    }

    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        if (_size == 0)
        {
            result = default;
            return false;
        }

        result = _array[_head];
        return true;
    }

    // Returns true if the queue contains at least one object equal to item.
    // Equality is determined using EqualityComparer<T>.Default.Equals().
    public bool Contains(T item)
    {
        if (_size == 0) return false;

        if (_head < _tail) return _array.IndexOf(item, _head, _size) >= 0;

        // We've wrapped around. Check both partitions, the least recently enqueued first.
        return
            _array.IndexOf(item, _head, _array.Length - _head) >= 0 ||
            _array.IndexOf(item, 0, _tail) >= 0;
    }

    // Iterates over the objects in the queue, returning an array of the
    // objects in the Queue, or an empty array if the queue is empty.
    // The order of elements in the array is first in to last in, the same
    // order produced by successive calls to Dequeue.
    public T[] ToArray()
    {
        if (_size == 0) return Array.Empty<T>();

        var arr = new T[_size];

        if (_head < _tail)
        {
            _array.Copy(_head, arr, 0, _size);
        }
        else
        {
            _array.Copy(_head, arr, 0, _array.Length - _head);
            _array.Copy(0, arr, _array.Length - _head, _tail);
        }

        return arr;
    }

    // PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
    // must be >= _size.
    private void SetCapacity(int capacity)
    {
        var newArr = new ValueArray<T>(capacity, _array.AllocatorProviderId);
        if (_size > 0)
        {
            if (_head < _tail)
            {
                _array.Copy(_head, newArr, 0, _size);
            }
            else
            {
                _array.Copy(_head, newArr, 0, _array.Length - _head);
                _array.Copy(0, newArr, _array.Length - _head, _tail);
            }
        }

        _array.Dispose();
        _array = newArr;
        _head = 0;
        _tail = _size == capacity ? 0 : _size;
    }

    // Increments the index wrapping it if necessary.
    private void MoveNext(ref int index)
    {
        // It is tempting to use the remainder operator here but it is actually much slower
        // than a simple comparison and a rarely taken branch.
        // JIT produces better code than with ternary operator ?:
        var tmp = index + 1;
        if (tmp == _array.Length) tmp = 0;
        index = tmp;
    }

    private void ThrowForEmptyQueue()
    {
        Debug.Assert(_size == 0);
        throw new InvalidOperationException("InvalidOperation_EmptyQueue");
    }

    public void TrimExcess()
    {
        var threshold = (int) (_array.Length * 0.9);
        if (_size < threshold) SetCapacity(_size);
    }

    /// <summary>
    ///     Ensures that the capacity of this Queue is at least the specified <paramref name="capacity" />.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
    /// <returns>The new capacity of this queue.</returns>
    public int EnsureCapacity(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "ArgumentOutOfRange_NeedNonNegNum");

        if (_array.Length < capacity) Grow(capacity);

        return _array.Length;
    }

    private void Grow(int capacity)
    {
        Debug.Assert(_array.Length < capacity);

        const int GrowFactor = 2;
        const int MinimumGrow = 4;

        var newcapacity = GrowFactor * _array.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint) newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

        // Ensure minimum growth is respected.
        newcapacity = Math.Max(newcapacity, _array.Length + MinimumGrow);

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newcapacity < capacity) newcapacity = capacity;

        SetCapacity(newcapacity);
    }

    // Implements an enumerator for a Queue.  The enumerator uses the
    // internal version number of the list to ensure that no modifications are
    // made to the list while an enumeration is in progress.
    public struct Enumerator : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>, IAddressFixed
    {
        private readonly ValueArray<T> _array;
        private readonly int _head;
        private int _size;
        private int _index; // -1 = not started, -2 = ended/disposed
        private int _arrayIndex = -1;

        internal Enumerator(ValueArray<T> array, int size, int head)
        {
            _array = array;
            _size = size;
            _head = head;
            _index = -1;
        }

        public void Dispose()
        {
            _index = -2;
        }

        public bool MoveNext()
        {
            if (_index == -2)
            {
                _arrayIndex = -1;
                return false;
            }

            _index++;

            if (_index == _size)
            {
                // We've run past the last element
                _index = -2;
                _arrayIndex = -1;
                return false;
            }

            var capacity = _array.Length;

            // _index represents the 0-based index into the queue, however the queue
            // doesn't have to start from 0 and it may not even be stored contiguously in memory.

            var arrayIndex = _head + _index; // this is the actual index into the queue's backing array
            if (arrayIndex >= capacity)
                // NOTE: Originally we were using the modulo operator here, however
                // on Intel processors it has a very high instruction latency which
                // was slowing down the loop quite a bit.
                // Replacing it with simple comparison/subtraction operations sped up
                // the average foreach loop by 2x.
                arrayIndex -= capacity; // wrap around if needed

            _arrayIndex = arrayIndex;
            return true;
        }

        public ref T Current
        {
            get
            {
                if (_index < 0) ThrowEnumerationNotStartedOrEnded();
                return ref _array[_arrayIndex];
            }
        }

        public unsafe T* CurrentPtr
        {
            get
            {
                if (_index < 0) ThrowEnumerationNotStartedOrEnded();
                return (T*) Unsafe.AsPointer(ref _array[_arrayIndex]);
            }
        }

        [DoesNotReturn]
        private void ThrowEnumerationNotStartedOrEnded()
        {
            Debug.Assert(_index == -1 || _index == -2);
            throw new InvalidOperationException(_index == -1
                ? "InvalidOperation_EnumNotStarted"
                : "InvalidOperation_EnumEnded");
        }

        public void Reset()
        {
            _index = -1;
            _arrayIndex = -1;
        }

        T IEnumerator<T>.Current
        {
            get
            {
                if (_index < 0) ThrowEnumerationNotStartedOrEnded();
                return _array[_arrayIndex];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (_index < 0) ThrowEnumerationNotStartedOrEnded();
                return _array[_arrayIndex];
            }
        }

        int? IMaybeCountable.Count => _size;
        int? IMaybeCountable.MaxCount => _size;

        public bool SetSkipCount(int count)
        {
            while (count-- > 0 && MoveNext())
            {
            }

            return true;
        }

        public bool SetTakeCount(int count)
        {
            _size = count;
            return true;
        }
    }

    public bool IsAllocated => _array.IsAllocated;

    public ValueQueue<T> Borrow()
    {
        return new ValueQueue<T>(_array.Borrow(), _head, _tail, _size);
    }

    public void Dispose()
    {
        _array.Dispose();
    }
}