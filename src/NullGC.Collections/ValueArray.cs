﻿using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Linq;
using UIntPtr = System.UIntPtr;

namespace NullGC.Collections;

[DebuggerDisplay("Count = {Length}, IsAllocated = {IsAllocated}")]
public struct ValueArray<T> : IUnmanagedArray<T>, ILinqEnumerable<T, UnmanagedArrayEnumerator<T>>, IList<T>,
    IExplicitOwnership<ValueArray<T>> where T : unmanaged
{
    public readonly ValueArray<T> WithAllocationProviderId(int id)
    {
        unsafe
        {
            return new ValueArray<T>(_items, _length, id);
        }
    }

    public ValueArray(int length, AllocatorTypes allocatorProviderId = AllocatorTypes.Default, bool noClear = false) :
        this(length, (int) allocatorProviderId, noClear)
    {
    }

    public ValueArray(int length, int allocatorProviderId, bool noClear = false)
    {
        unsafe
        {
            Debug.Assert(length == 0 || allocatorProviderId != (int) AllocatorTypes.Invalid);
            _allocatorProviderId = allocatorProviderId;
            if (length == 0)
                return;
            var size = (nuint) sizeof(T) * (nuint) length;
            if (size > 0)
            {
                var ptr = AllocatorContext.GetAllocator(allocatorProviderId).Allocate(size);
                Debug.Assert(ptr % (nuint) UIntPtr.Size == 0);
                if (!noClear) UnsafeHelper.InitBlock(ptr.ToPointer(), 0, size);
                _items = (T*) ptr.ToPointer();
            }

            _length = length;
        }
    }

    public unsafe ValueArray(T* buf, int length)
    {
        if (!(length == 0 || buf != (void*) 0))
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(
                $"Either {nameof(buf)} must be a valid pointer or {nameof(length)} must be zero.");
        if ((UIntPtr) buf % (nuint) UIntPtr.Size != 0)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(nameof(buf),
                "Address must align to pointer size.");
        
        _items = buf;
        _length = length;
        _allocatorProviderId = (int) AllocatorTypes.Invalid;
    }

    private unsafe ValueArray(T* buf, int length, int allocatorProviderId)
    {
        Debug.Assert(!(length <= 0 && buf != (void*) 0));
        Debug.Assert((UIntPtr) buf % (nuint) UIntPtr.Size == 0);
        _items = buf;
        _length = length;
        _allocatorProviderId = allocatorProviderId;
    }

    private unsafe T* _items;
    private int _length;
    public int AllocatorProviderId => _allocatorProviderId;
    public int _allocatorProviderId;

    public readonly unsafe T* Items
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items;
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    readonly int IList<T>.IndexOf(T item) => this.IndexOf(item);

    void IList<T>.Insert(int index, T item)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    void IList<T>.RemoveAt(int index)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0 || index >= _length) ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
            unsafe
            {
                return ref _items[index];
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref T GetRefUnchecked(int index)
    {
        Debug.Assert(index >= 0 && index < _length);
        unsafe
        {
            return ref _items[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T GetUnchecked(int index)
    {
        Debug.Assert(index >= 0 && index < _length);
        unsafe
        {
            return _items[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly unsafe T* GetPtr(int index)
    {
        Guard.IsInRange(index, 0, _length);
        return &_items[index];
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal readonly unsafe T* GetPtrUnchecked(int index)
    // {
    //     Debug.Assert(index >= 0 && index < _length);
    //     return &_items[index];
    // }

    T IList<T>.this[int index]
    {
        readonly get => this[index];
        set => this[index] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ValueArray<T> Borrow()
    {
        unsafe
        {
            return new ValueArray<T>(_items, _length);
        }
    }

    public ValueArray<T> Take()
    {
        unsafe
        {
            var allocId = _allocatorProviderId;
            _allocatorProviderId = (int) AllocatorTypes.Invalid;
            return new ValueArray<T>(_items, _length, allocId);
        }
    }

    public void Dispose()
    {
        unsafe
        {
            if (_allocatorProviderId != (int) AllocatorTypes.Invalid)
            {
                _length = 0;
                if (_items == (void*) 0) return;
                AllocatorContext.GetAllocator(_allocatorProviderId).Free((UIntPtr) _items);
                _items = (T*) 0;
            }
        }
    }

    public readonly T[] ToArray()
    {
        if (_length == 0)
            return Array.Empty<T>();
        return this.AsReadOnlySpan().ToArray();
    }

    public static implicit operator ReadOnlySpan<T>(ValueArray<T> src)
    {
        return src.AsReadOnlySpan();
    }

    public static implicit operator Span<T>(ValueArray<T> src)
    {
        return src.AsSpan();
    }

    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        _length == 0 ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly UnmanagedArrayEnumerator<T> GetEnumerator()
    {
        unsafe
        {
            return new UnmanagedArrayEnumerator<T>(_items, _length);
        }
    }

    readonly IEnumerator IEnumerable.GetEnumerator() =>
        _length == 0 ? GenericEmptyEnumerator<T>.Instance : GetEnumerator();

    void ICollection<T>.Add(T item)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    void ICollection<T>.Clear()
    {
        unsafe
        {
            UnsafeHelper.InitBlockUnaligned(_items, 0, (nuint) _length * (nuint) sizeof(T));
        }
    }

    readonly bool ICollection<T>.Contains(T item) => this.Contains(item);

    readonly void ICollection<T>.CopyTo(T[] array, int arrayIndex) => this.CopyTo(array, arrayIndex);

    bool ICollection<T>.Remove(T item) => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();

    readonly int ICollection<T>.Count => _length;

    readonly bool ICollection<T>.IsReadOnly => false;

    readonly int? IMaybeCountable.MaxCount => _length;

    readonly unsafe T* IUnmanagedArray<T>.Items => _items;
    readonly int IUnmanagedArray<T>.Length => _length;

    public readonly bool IsAllocated
    {
        get
        {
            unsafe
            {
                return _items != (void*) 0;
            }
        }
    }

    public static readonly ValueArray<T> Empty = default;

    int? IMaybeCountable.Count => Length;

    internal int TryGrowCheaply(int minLength, int maxLength, bool noClear)
    {
        if (minLength <= _length)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(minLength));
        if (_allocatorProviderId == (int) AllocatorTypes.Invalid)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid allocator provider.");

        unsafe
        {
            var allocator = AllocatorContext.GetAllocator(_allocatorProviderId);
            if (minLength > 0)
            {
#if DEBUG
                var bak = this.AsReadOnlySpan().ToArray();
#endif
                var reallocResult = allocator.TryRealloc((UIntPtr) _items, (nuint) minLength * (nuint) sizeof(T),
                    (nuint) maxLength * (nuint) sizeof(T));
                if (!reallocResult.Success) return -1;
#if DEBUG
                Debug.Assert(MemoryMarshal.CreateReadOnlySpan(in Unsafe.AsRef<T>((T*) reallocResult.Ptr), _length)
                    .SequenceEqual(bak));
#endif

                Debug.Assert((reallocResult.Ptr % (nuint) MemoryConstants.DefaultAlignment) == 0);
                minLength = (int) (reallocResult.ActualSize / (nuint) sizeof(T));
                if (!noClear && minLength > _length)
                    UnsafeHelper.InitBlockUnaligned(((T*) reallocResult.Ptr) + _length, 0,
                        (nuint)(minLength - _length) * (nuint)sizeof(T));
                _items = (T*) reallocResult.Ptr;
            }
            else
            {
                minLength = 0;
                if (_items != (T*) 0)
                {
                    allocator.Free((UIntPtr) _items);
                    _items = (T*) 0;
                }
            }

            _length = minLength;
            return minLength;
        }
    }

    internal void ResizeByAllocateNew(int newLength, bool noClear = false)
    {
        if (_allocatorProviderId == (int) AllocatorTypes.Invalid)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid allocator provider.");
        var allocator = AllocatorContext.GetAllocator(_allocatorProviderId);
        unsafe
        {
            var size = (nuint) newLength * (nuint) sizeof(T);
            var newPtr = allocator.Allocate(size).ToPointer();
            var items = _items;
            if (items != (T*) 0)
            {
                Debug.Assert((UIntPtr) items % (nuint) UIntPtr.Size == 0);
                Debug.Assert((UIntPtr) newPtr % (nuint) UIntPtr.Size == 0);
                UnsafeHelper.CopyBlock(newPtr, items, (nuint) Math.Min(_length, newLength) * (nuint) sizeof(T));
                allocator.Free((UIntPtr) items);
            }

            if (!noClear && newLength > _length)
                UnsafeHelper.InitBlockUnaligned(((T*) newPtr) + _length, 0,
                    (nuint) (newLength - _length) * (nuint) sizeof(T));
            _items = (T*) newPtr;
            _length = newLength;
        }
    }

    internal int Grow(int minLength, int maxLength, bool noClear = false)
    {
        if (_allocatorProviderId == (int) AllocatorTypes.Invalid)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid allocator provider ID.");
        if (minLength <= _length)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(minLength));

        unsafe
        {
            var minSize = (nuint) minLength * (nuint) sizeof(T);
            var maxSize = (nuint) maxLength * (nuint) sizeof(T);
            var allocator = AllocatorContext.GetAllocator(_allocatorProviderId);
            var items = _items;
            var reallocResult = allocator.TryRealloc((UIntPtr) items, minSize, maxSize);
            if (reallocResult.Success)
            {
                Debug.Assert(reallocResult.Ptr % (nuint) UIntPtr.Size == 0);
                _items = (T*) reallocResult.Ptr;
#if DEBUG
                var v = checked((int) (reallocResult.ActualSize / (nuint) sizeof(T)));
                Debug.Assert(minLength != maxLength || v == minLength);
                minLength = v;
#else
                minLength = checked((int) (reallocResult.ActualSize / (nuint) sizeof(T)));
#endif
            }
            else
            {
                var newPtr = allocator.Allocate(maxSize);
                if (items != (T*) 0)
                {
                    Debug.Assert((UIntPtr) items % (nuint) UIntPtr.Size == 0);
                    Debug.Assert(newPtr % (nuint) UIntPtr.Size == 0);
                    UnsafeHelper.CopyBlock(newPtr.ToPointer(), items, (nuint) _length * (nuint) sizeof(T));
                    allocator.Free((UIntPtr) items);
                }

                _items = (T*) newPtr;
#if DEBUG
                var v = (int) (maxSize / (nuint) sizeof(T));
                Debug.Assert(minLength != maxLength || v == minLength);
                minLength = v;
#else
                minLength = (int) (maxSize / (nuint) sizeof(T));
#endif
            }

            if (!noClear && minLength > _length)
                UnsafeHelper.InitBlockUnaligned(((T*) _items) + _length, 0,
                    (nuint) (minLength - _length) * (nuint) sizeof(T));
        }

        _length = minLength;
        return minLength;
    }

    public override string ToString()
    {
        unsafe
        {
            return
                $"{nameof(_items)}: {(UIntPtr) _items:X}, {nameof(_length)}: {_length}, {nameof(AllocatorProviderId)}: {AllocatorProviderId}";
        }
    }
}

public static class ValueArray
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(ValueArray<T> src, ValueArray<T> dest, int count) where T : unmanaged
    {
        Guard.IsGreaterThanOrEqualTo(dest.Length, count);
        src.AsReadOnlySpan(0, count).CopyTo(dest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(this ValueArray<T> src, int srcIndex, ValueArray<T> dest, int destIndex, int count)
        where T : unmanaged
    {
        src.AsReadOnlySpan(srcIndex, count).CopyTo(dest.AsSpan(destIndex));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(this ValueArray<T> src, int srcIndex, T[] dest, int destIndex, int count)
        where T : unmanaged
    {
        src.AsReadOnlySpan(srcIndex, count).CopyTo(dest.AsSpan(destIndex));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this ValueArray<T> arr, T value, int startIndex, int count) where T : unmanaged
    {
        Guard.IsInRange(startIndex, 0, arr.Length);
        for (int i = startIndex; i < startIndex + count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(arr.GetUnchecked(i), value))
                return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(ValueArray<T> arr) where T : unmanaged
    {
        unsafe
        {
            UnsafeHelper.InitBlock(arr.Items, 0, (nuint)arr.Length * (nuint)sizeof(T));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ValueArray<T> arr, int startIndex, int count) where T : unmanaged
    {
        unsafe
        {
            UnsafeHelper.InitBlockUnaligned(arr.Items + startIndex, 0, (nuint) count * (nuint) sizeof(T));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse<T>(ValueArray<T> arr, int startIndex, int count) where T : unmanaged
    {
        arr.AsSpan(startIndex, count).Reverse();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Sort<T>(ValueArray<T> arr, int startIndex, int count, IComparer<T>? comparer = null)
        where T : unmanaged
    {
        arr.AsSpan(startIndex, count).Sort(comparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(ValueArray<T> src, T[] dest, int count) where T : unmanaged
    {
        Copy(src, 0, dest, 0, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this ValueArray<T> arr, T item, int startIndex, int count) where T : unmanaged
    {
        Guard.IsInRange(startIndex, 0, arr.Length);
        unsafe
        {
            for (var i = startIndex; i >= startIndex - count + 1; i--)
                if (EqualityComparer<T>.Default.Equals(arr.Items[i], item))
                    return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this ValueArray<T> arr, T item, int startIndex) where T : unmanaged
    {
        Guard.IsInRange(startIndex, 0, arr.Length);
        unsafe
        {
            for (var i = startIndex; i >= 0; i--)
                if (EqualityComparer<T>.Default.Equals(arr.Items[i], item))
                    return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this ValueArray<T> arr, T item) where T : unmanaged
    {
        unsafe
        {
            for (var i = arr.Length - 1; i >= 0; i--)
                if (EqualityComparer<T>.Default.Equals(arr.Items[i], item))
                    return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this ValueArray<T> arr, T value) where T : unmanaged
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(arr.GetUnchecked(i), value))
                return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this ValueArray<T> arr, T value, int startIndex) where T : unmanaged
    {
        Guard.IsInRange(startIndex, 0, arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(arr.GetUnchecked(i), value))
                return i;
        }

        return -1;
    }
}