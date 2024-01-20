// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Linq;

namespace NullGC.Collections;

// modified from the code of .NET Core Stack<>.
[DebuggerDisplay("Count = {Count}, IsInitialized = {IsInitialized}")]
public struct ValueStack<T> : ILinqEnumerable<T, ValueStack<T>.Enumerator>, ISingleDisposable<ValueStack<T>>,
    ICollection, IReadOnlyCollection<T>, IUnsafeArray<T> where T : unmanaged
{
    private ValueArray<T> _array; // Storage for stack elements. Do not rename (binary serialization)
    private int _size; // Number of items in the stack. Do not rename (binary serialization)

    private const int DefaultCapacity = 4;

    private ValueStack(ValueArray<T> array, int size)
    {
        _array = array;
        _size = size;
    }

    public ValueStack() : this(0, (int) AllocatorTypes.Default)
    {
    }

    public ValueStack(AllocatorTypes allocatorProviderId) : this(0, (int) allocatorProviderId)
    {
    }

    // Create a stack with a specific initial capacity.  The initial capacity
    // must be a non-negative number.
    public ValueStack(int capacity, AllocatorTypes allocatorProviderId) : this(capacity, (int) allocatorProviderId)
    {
    }

    public ValueStack(int capacity, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        Guard.IsGreaterThanOrEqualTo(capacity, 0);
        _array = new ValueArray<T>(capacity, allocatorProviderId, true);
    }

    // Fills a Stack with the contents of a particular collection.  The items are
    // pushed onto the stack in the same order they are read by the enumerator.
    public ValueStack(IEnumerable<T> collection, AllocatorTypes allocatorProviderId) :
        this(collection, (int) allocatorProviderId)
    {
    }

    public ValueStack(IEnumerable<T> collection, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        _array = collection.ToValueArray(allocatorProviderId);
    }

    public int Count => _size;
    int? IMaybeCountable.MaxCount => Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    public void Clear()
    {
        _size = 0;
    }

    public bool Contains(T item)
    {
        // Compare items using the default equality comparer

        // PERF: Internally Array.LastIndexOf calls
        // EqualityComparer<T>.Default.LastIndexOf, which
        // is specialized for different types. This
        // boosts performance since instead of making a
        // virtual method call each iteration of the loop,
        // via EqualityComparer<T>.Default.Equals, we
        // only make one virtual call to EqualityComparer.LastIndexOf.

        return _size != 0 && _array.LastIndexOf(item, _size - 1) != -1;
    }

    void ICollection.CopyTo(Array array, int arrayIndex)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    // Returns an IEnumerator for this Stack.
    public Enumerator GetEnumerator() => new(this);

    /// <internalonly />
    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        _size == 0 ? GenericEmptyEnumerator<T>.Instance : new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => _size == 0 ? GenericEmptyEnumerator<T>.Instance : new Enumerator(this);

    public void TrimExcess()
    {
        var threshold = (int) (_array.Length * 0.9);
        if (_size >= threshold) return;
        _array.ResizeByAllocateNew(_size, true);
    }

    // Returns the top object on the stack without removing it.  If the stack
    // is empty, Peek throws an InvalidOperationException.
    public T Peek()
    {
        var size = _size - 1;

        if ((uint) size >= (uint) _array.Length) ThrowForEmptyStack();

        return _array.GetUnchecked(size);
    }

    public ref T PeekRef()
    {
        var size = _size - 1;

        if ((uint) size >= (uint) _array.Length) ThrowForEmptyStack();

        return ref _array.GetRefUnchecked(size);
    }

    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        var size = _size - 1;

        if ((uint) size >= (uint) _array.Length)
        {
            result = default!;
            return false;
        }

        result = _array.GetUnchecked(size);
        return true;
    }

    // Pops an item from the top of the stack.  If the stack is empty, Pop
    // throws an InvalidOperationException.
    public T Pop()
    {
        var size = _size - 1;

        // if (_size == 0) is equivalent to if (size == -1), and this case
        // is covered with (uint)size, thus allowing bounds check elimination
        // https://github.com/dotnet/coreclr/pull/9773
        if ((uint) size >= (uint) _array.Length) ThrowForEmptyStack();

        _size = size;
        return _array.GetUnchecked(size);
    }

    public bool TryPop([MaybeNullWhen(false)] out T result)
    {
        var size = _size - 1;

        if ((uint) size >= (uint) _array.Length)
        {
            result = default!;
            return false;
        }

        _size = size;
        result = _array.GetUnchecked(size);
        return true;
    }

    // Pushes an item to the top of the stack.
    public void Push(T item)
    {
        var size = _size;

        if ((uint) size < (uint) _array.Length)
        {
            _array.GetRefUnchecked(size) = item;
            _size = size + 1;
        }
        else
        {
            PushWithResize(item);
        }
    }

    // Non-inline from Stack.Push to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void PushWithResize(T item)
    {
        Debug.Assert(_size == _array.Length);
        Grow(_size + 1);
        _array.GetRefUnchecked(_size) = item;
        _size++;
    }

    /// <summary>
    ///     Ensures that the capacity of this Stack is at least the specified <paramref name="capacity" />.
    ///     If the current capacity of the Stack is less than specified <paramref name="capacity" />,
    ///     the capacity is increased by continuously twice current capacity until it is at least the specified
    ///     <paramref name="capacity" />.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
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

        var newCapacity = _array.Length == 0 ? DefaultCapacity : 2 * _array.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast.
        if ((uint) newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;

        // If computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newCapacity < capacity) newCapacity = capacity;

        _array.Grow(capacity + ((newCapacity - _array.Length) >> 1), newCapacity, true);
    }

    // Copies the Stack to an array, in the same order Pop would return the items.
    public T[] ToArray()
    {
        if (_size == 0)
            return Array.Empty<T>();

        var objArray = new T[_size];
        var i = 0;
        while (i < _size)
        {
            objArray[i] = _array.GetUnchecked(_size - i - 1);
            i++;
        }

        return objArray;
    }

    private void ThrowForEmptyStack()
    {
        Debug.Assert(_size == 0);
        throw new InvalidOperationException("InvalidOperation_EmptyStack");
    }

    public struct Enumerator : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>, IAddressFixed
    {
        private readonly ValueStack<T> _stack;
        private int _index;

        internal Enumerator(ValueStack<T> stack)
        {
            _stack = stack;
            _index = -2;
        }

        public void Dispose()
        {
            _index = -1;
        }

        public bool MoveNext()
        {
            bool retval;
            if (_index == -2)
            {
                // First call to enumerator.
                _index = _stack._size - 1;
                retval = _index >= 0;
                // if (retval)
                //     _currentElement = _stack._array[_index];
                return retval;
            }

            if (_index == -1)
                // End of enumeration.
                return false;

            retval = --_index >= 0;
            return retval;
        }

        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_index < 0) ThrowEnumerationNotStartedOrEnded();
                return ref _stack._array.GetRefUnchecked(_index);
            }
        }

        public unsafe T* CurrentPtr => &_stack._array.Items[_index];

        T IEnumerator<T>.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_index < 0) ThrowEnumerationNotStartedOrEnded();
                return _stack._array.GetUnchecked(_index);
            }
        }

        private void ThrowEnumerationNotStartedOrEnded()
        {
            Debug.Assert(_index == -1 || _index == -2);
            throw new InvalidOperationException(_index == -2
                ? "InvalidOperation_EnumNotStarted"
                : "InvalidOperation_EnumEnded");
        }

        object IEnumerator.Current => Current;

        void IEnumerator.Reset()
        {
            _index = -2;
        }

        public int? Count => _stack.Count;
        public int? MaxCount => _stack.Count;

        public bool SetSkipCount(int count)
        {
            return false;
        }

        public bool SetTakeCount(int count)
        {
            return false;
        }
    }

    int? IMaybeCountable.Count => Count;
    public unsafe T* Items => _array.Items;
    public int Length => _size;
    public bool IsInitialized => _array.IsInitialized;

    public ValueStack<T> Borrow() => new(_array.Borrow(), _size);

    public void Dispose() => _array.Dispose();
}