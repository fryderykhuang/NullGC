// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Linq;

namespace NullGC.Collections;

// modified from the code of .NET Core List<>.
[DebuggerDisplay("Count = {Count}, IsAllocated = {IsAllocated}")]
public struct ValueList<T> : ILinqEnumerable<T, UnmanagedArrayEnumerator<T>>, IUnmanagedArray<T>, IList<T>,
    IReadOnlyList<T>,
    IExplicitOwnership<ValueList<T>> where T : unmanaged
{
    private const int DefaultCapacity = 4;
    private ValueArray<T> _items;
    private int _size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueList(ValueArray<T> items, int size)
    {
        _items = items;
        _size = size;
    }

    public ValueList() : this(0, (int) AllocatorTypes.Default)
    {
    }

    public ValueList(AllocatorTypes allocatorProviderId = AllocatorTypes.Default)
    {
        _items = new ValueArray<T>(0, allocatorProviderId);
    }

    public ValueList(int capacity, AllocatorTypes allocatorProviderId) :
        this(capacity, (int) allocatorProviderId)
    {
    }

    public ValueList(int capacity, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        Guard.IsGreaterThanOrEqualTo(capacity, 0, nameof(capacity));

        _items = capacity == 0
            ? ValueArray<T>.Empty.WithAllocationProviderId(allocatorProviderId)
            : new ValueArray<T>(capacity, allocatorProviderId, true);
    }

    public ValueList(IEnumerable<T> collection, AllocatorTypes allocatorProviderId) :
        this(collection, (int) allocatorProviderId)
    {
    }

    public ValueList(IEnumerable<T> collection, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        Guard.IsNotNull(collection, nameof(collection));

        if (collection is ICollection<T> c)
        {
            var count = c.Count;
            if (count == 0)
            {
                _items = ValueArray<T>.Empty.WithAllocationProviderId(allocatorProviderId);
            }
            else
            {
                _items = new ValueArray<T>(count, allocatorProviderId, true);
                c.CopyTo(_items, 0);
                _size = count;
            }
        }
        else
        {
            _items = ValueArray<T>.Empty.WithAllocationProviderId(allocatorProviderId);
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in collection) Add(item);
        }
    }

    public int Capacity
    {
        readonly get => _items.Length;
        set
        {
            if (value == _items.Length) return;
            Guard.IsGreaterThanOrEqualTo(value, _size, nameof(value));
            if (value > _items.Length)
                _items.Grow(value, value, true);
            else
                _items.ResizeByAllocateNew(value, true);
        }
    }

    public readonly int Count
    {
        get
        {
            Debug.Assert(_size <= _items.Length);
            return _size;
        }
    }

    readonly int? IMaybeCountable.MaxCount => Count;

    readonly bool ICollection<T>.IsReadOnly => false;

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0 || index >= _size)
                CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
            return ref _items.GetRefUnchecked(index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref T GetRefUnchecked(int index)
    {
        Debug.Assert(index >= 0 && index < _size);
        return ref _items.GetRefUnchecked(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T GetUnchecked(int index)
    {
        Debug.Assert(index >= 0 && index < _size);
        return _items.GetUnchecked(index);
    }

    readonly T IReadOnlyList<T>.this[int index] => this[index];

    T IList<T>.this[int index]
    {
        readonly get => this[index];
        set => this[index] = value;
    }

    public ref T AddAndReturnsRef()
    {
        if ((uint) _size < (uint) _items.Length)
        {
            return ref _items.GetRefUnchecked(_size++);
        }
        else
        {
            Grow(_size + 1);
            return ref _items.GetRefUnchecked(_size++);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        var size = _size;
        if ((uint) size < (uint) _items.Length)
            unsafe
            {
                _size = size + 1;
                _items.Items[size] = item;
            }
        else
            AddWithResize(item);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        Debug.Assert(_size == _items.Length);
        var size = _size;
        Grow(size + 1);
        _size = size + 1;
        _items.GetRefUnchecked(size) = item;
    }

    public void AddRange(IEnumerable<T> collection)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        Guard.IsNotNull(collection, nameof(collection));

        if (collection is ICollection<T> c)
        {
            var count = c.Count;
            if (count > 0)
            {
                if (_items.Length - _size < count) Grow(checked(_size + count));

                c.CopyTo(_items, _size);
                _size += count;
            }
        }
        else
        {
            // ReSharper disable once PossibleMultipleEnumeration
            using (var en = collection.GetEnumerator())
            {
                while (en.MoveNext()) Add(en.Current);
            }
        }
    }

    public readonly int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
    {
        if (index < 0)
            ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
        if (count < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "ArgumentOutOfRange_NeedNonNegNum");
        if (_size - index < count)
            ThrowHelper.ThrowArgumentException("Argument_InvalidOffLen");

        return _items.AsReadOnlySpan().Slice(index, count).BinarySearch(item, comparer ?? Comparer<T>.Default);
    }

    public readonly int BinarySearch(T item)
    {
        return BinarySearch(0, Count, item, null);
    }

    public readonly int BinarySearch(T item, IComparer<T>? comparer)
    {
        return BinarySearch(0, Count, item, comparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            var size = _size;
            _size = 0;
            if (size > 0) ValueArray.Clear(_items, 0, size);
        }
        else
        {
            _size = 0;
        }
    }

    public readonly bool Contains(T item)
    {
        return _size != 0 && IndexOf(item) >= 0;
    }

    public ValueList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) where TOutput : unmanaged
    {
        if (converter == null) ThrowHelper.ThrowArgumentNullException(nameof(converter));

        var list = new ValueList<TOutput>(_size);
        for (var i = 0; i < _size; i++) list._items.GetRefUnchecked(i) = converter(_items.GetUnchecked(i));

        list._size = _size;
        return list;
    }

    public void CopyTo(T[] array)
    {
        CopyTo(array, 0);
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        if (_size - index < count) ThrowHelper.ThrowArgumentException("Argument_InvalidOffLen");

        _items.AsReadOnlySpan(index).CopyTo(array.AsSpan(arrayIndex, count));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ValueArray.Copy(_items, 0, array, arrayIndex, _size);
    }

    public int EnsureCapacity(int capacity)
    {
        if (capacity < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(capacity), "ArgumentOutOfRange_NeedNonNegNum");

        if (_items.Length < capacity) Grow(capacity);

        return _items.Length;
    }

    private void Grow(int capacity)
    {
        Debug.Assert(_items.Length < capacity);
        var newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint) newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newCapacity < capacity) newCapacity = capacity;

        _items.Grow(capacity + ((newCapacity - capacity) >> 1), newCapacity, true);
    }

    public bool Exists(Predicate<T> match)
    {
        return FindIndex(match) != -1;
    }

    public T? Find(Predicate<T> match)
    {
        if (match == null) ThrowHelper.ThrowArgumentNullException(nameof(match));

        for (var i = 0; i < _size; i++)
            if (match(_items.GetUnchecked(i)))
                return _items.GetUnchecked(i);

        return default;
    }

    public ValueList<T> FindAll(Predicate<T> match)
    {
        if (match == null) ThrowHelper.ThrowArgumentNullException(nameof(match));

        var list = new ValueList<T>();
        for (var i = 0; i < _size; i++)
            if (match(_items.GetUnchecked(i)))
                list.Add(_items.GetUnchecked(i));

        return list;
    }

    public int FindIndex(Predicate<T> match)
    {
        return FindIndex(0, _size, match);
    }

    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return FindIndex(startIndex, _size - startIndex, match);
    }

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        if ((uint) startIndex > (uint) _size)
            ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual();

        if (count < 0 || startIndex > _size - count)
            ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();

        if (match == null) ThrowHelper.ThrowArgumentNullException(nameof(match));

        var endIndex = startIndex + count;
        for (var i = startIndex; i < endIndex; i++)
            if (match(_items.GetUnchecked(i)))
                return i;

        return -1;
    }

    public T? FindLast(Predicate<T> match)
    {
        if (match == null) ThrowHelper.ThrowArgumentNullException(nameof(match));

        for (var i = _size - 1; i >= 0; i--)
            if (match(_items.GetUnchecked(i)))
                return _items.GetUnchecked(i);

        return default;
    }

    public int FindLastIndex(Predicate<T> match)
    {
        return FindLastIndex(_size - 1, _size, match);
    }

    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return FindLastIndex(startIndex, startIndex + 1, match);
    }

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        if (match == null) ThrowHelper.ThrowArgumentNullException(nameof(match));

        if (_size == 0)
            Guard.IsEqualTo(startIndex, -1, nameof(startIndex));
        else
            Guard.IsLessThan((uint) startIndex, (uint) _size, nameof(startIndex));

        Guard.IsFalse(count < 0 || startIndex - count + 1 < 0, nameof(count));

        var endIndex = startIndex - count;
        for (var i = startIndex; i > endIndex; i--)
            if (match(_items.GetUnchecked(i)))
                return i;

        return -1;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Count == 0 ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();
    }

    public readonly UnmanagedArrayEnumerator<T> GetEnumerator()
    {
        unsafe
        {
            return new UnmanagedArrayEnumerator<T>(_items.Items, _size);
        }
    }

    // public readonly ValueList<T> GetRange(int index, int count)
    // {
    //     Guard.IsGreaterThanOrEqualTo(index, 0, nameof(index));
    //     Guard.IsGreaterThanOrEqualTo(count, 0, nameof(count));
    //     Guard.IsLessThanOrEqualTo(count, _size - index, nameof(count));
    //
    //     var list = new ValueList<T>(count, _items.AllocatorProviderId);
    //     ValueArray.Copy(_items, index, list._items, 0, count);
    //     list._size = count;
    //     return list;
    // }

    public readonly int IndexOf(T item)
    {
        return _items.IndexOf(item, 0, _size);
    }

    public readonly int IndexOf(T item, int index)
    {
        Guard.IsLessThan(index, _size);
        return _items.IndexOf(item, index, _size - index);
    }

    public readonly int IndexOf(T item, int index, int count)
    {
        Guard.IsLessThan(index, _size);
        return _items.IndexOf(item, index, count);
    }

    public void Insert(int index, T item)
    {
        Guard.IsLessThanOrEqualTo((uint) index, (uint) _size, nameof(index));

        if (_size == _items.Length) Grow(_size + 1);
        if (index < _size) _items.Copy(index, _items, index + 1, _size - index);

        _items.GetRefUnchecked(index) = item;
        _size++;
    }

    public void InsertRange<TCollection>(int insertIndex, TCollection collection) where TCollection : IEnumerable<T>
    {
        Guard.IsNotNull(collection, nameof(collection));

        Guard.IsLessThanOrEqualTo((uint) insertIndex, (uint) _size, nameof(insertIndex));

        if (typeof(TCollection).IsAssignableTo(typeof(ICollection<T>)))
        {
            ref var c = ref Unsafe.As<TCollection, ICollection<T>>(ref collection);

            var count = c.Count;
            if (count > 0)
                unsafe
                {
                    EnsureCapacity(_size + count);

                    if (insertIndex < _size)
                        ValueArray.Copy(_items, insertIndex, _items, insertIndex + count, _size - insertIndex);

                    // Inserting part of self to self.
                    if (c is IUnmanagedArray<T> uarr && uarr.Items == _items.Items)
                    {
                        // Copy first part of _items to insert location
                        ValueArray.Copy(_items, 0, _items, insertIndex, insertIndex);
                        // Copy last part of _items back to inserted location
                        ValueArray.Copy(_items, insertIndex + count, _items, insertIndex * 2, _size - insertIndex);
                    }
                    else
                    {
                        c.CopyTo(_items, insertIndex);
                    }

                    _size += count;
                }
        }
        else
        {
            foreach (var item in collection) Insert(insertIndex++, item);
        }
    }

    public readonly int LastIndexOf(T item)
    {
        if (_size == 0) return -1;
        return _items.LastIndexOf(item, _size - 1);
    }

    public readonly int LastIndexOf(T item, int startIndex)
    {
        Guard.IsLessThan(startIndex, _size);
        return _items.LastIndexOf(item, startIndex, startIndex + 1);
    }

    public readonly int LastIndexOf(T item, int startIndex, int count)
    {
        if (_size == 0) return -1;
        Guard.IsLessThan(startIndex, _size);
        return _items.LastIndexOf(item, startIndex, count);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public int RemoveAll(Predicate<T> match)
    {
        Guard.IsNotNull(match, nameof(match));

        var freeIndex = 0; // the first free slot in items array

        // Find the first item which needs to be removed.
        while (freeIndex < _size && !match(_items.GetUnchecked(freeIndex))) freeIndex++;
        if (freeIndex >= _size) return 0;

        var current = freeIndex + 1;
        while (current < _size)
        {
            // Find the first item which needs to be kept.
            while (current < _size && match(_items.GetUnchecked(current))) current++;

            if (current < _size)
                // copy item to the free slot.
                _items.GetRefUnchecked(freeIndex++) = _items.GetUnchecked(current++);
        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            _items.Clear(freeIndex,
                _size - freeIndex); // Clear the elements so that the gc can reclaim the references.

        var result = _size - freeIndex;
        _size = freeIndex;
        return result;
    }

    public void RemoveAt(int index)
    {
        Guard.IsLessThan((uint) index, (uint) _size, nameof(index));

        _size--;
        if (index < _size)
            _items.Copy(index + 1, _items, index, _size - index);
    }

    public void RemoveRange(int index, int count)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0, nameof(index));
        Guard.IsGreaterThanOrEqualTo(count, 0, nameof(count));
        Guard.IsLessThanOrEqualTo(count, _size - index, nameof(count));

        if (count > 0)
        {
            _size -= count;
            if (index < _size) ValueArray.Copy(_items, index + count, _items, index, _size - index);

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) ValueArray.Clear(_items, _size, count);
        }
    }

    public void Reverse()
    {
        Reverse(0, Count);
    }

    public void Reverse(int index, int count)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0, nameof(index));
        Guard.IsGreaterThanOrEqualTo(count, 0, nameof(count));
        Guard.IsLessThanOrEqualTo(count, _size - index, nameof(count));

        if (count > 1) ValueArray.Reverse(_items, index, count);
    }

    public void Sort()
    {
        Sort(0, Count, null);
    }

    public void Sort(IComparer<T>? comparer)
    {
        Sort(0, Count, comparer);
    }

    public void Sort(int index, int count, IComparer<T>? comparer)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0, nameof(index));
        Guard.IsGreaterThanOrEqualTo(count, 0, nameof(count));
        Guard.IsLessThanOrEqualTo(count, _size - index, nameof(count));

        if (count > 1) ValueArray.Sort(_items, index, count, comparer);
    }

    public void Sort(Comparison<T> comparison)
    {
        Guard.IsNotNull(comparison, nameof(comparison));

        if (_size > 1)
            _items.AsSpan().Sort(comparison);
    }

    public readonly T[] ToArray()
    {
        if (_size == 0) return Array.Empty<T>();

        var array = new T[_size];
        ValueArray.Copy(_items, array, _size);
        return array;
    }

    public void TrimExcess()
    {
        var threshold = (int) (_items.Length * 0.9);
        if (_size < threshold) Capacity = _size;
    }

    public readonly bool TrueForAll(Predicate<T> match)
    {
        Guard.IsNotNull(match, nameof(match));

        for (var i = 0; i < _size; i++)
            if (!match(_items.GetUnchecked(i)))
                return false;

        return true;
    }

    public ValueList<T> Borrow()
    {
        return new ValueList<T>(_items.Borrow(), _size);
    }

    public ValueList<T> Take()
    {
        return new ValueList<T>(_items.Take(), _size);
    }

    public void Dispose()
    {
        _items.Dispose();
    }

    public readonly ValueArray<T> GetInnerArray()
    {
        return _items;
    }

    readonly IEnumerator IEnumerable.GetEnumerator()
    {
        return Count == 0 ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();
    }

    public readonly unsafe T* Items => _items.Items;
    readonly int IUnmanagedArray<T>.Length => _size;
    public readonly bool IsAllocated => _items.IsAllocated;

    public static readonly ValueList<T> Empty = default;
    readonly int? IMaybeCountable.Count => Count;

    /// <summary>
    ///     NOTICE: Current instance will become <see cref="Empty" /> if successful.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryConvertSelfToArray(out ValueArray<T> result)
    {
        if (_items.Length == _size)
        {
            result = _items;

            _items = default;
            _size = 0;
            return true;
        }

        result = default;
        return false;
    }
}