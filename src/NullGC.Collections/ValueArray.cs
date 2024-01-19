using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Linq;
using UIntPtr = System.UIntPtr;

namespace NullGC.Collections;

[DebuggerDisplay("Count = {Length}, IsInitialized = {IsInitialized}")]
public struct ValueArray<T> : IUnsafeArray<T>, ILinqEnumerable<T, UnsafeArrayEnumerator<T>>, IList<T>,
    ISingleDisposable<ValueArray<T>> where T : unmanaged
{
    public readonly ValueArray<T> WithAllocationProviderId(int id)
    {
        unsafe
        {
            return new ValueArray<T>(_items, _length, id);
        }
    }

    public ValueArray(int length, int allocatorProviderId = (int) AllocatorTypes.Default, bool noClear = false)
    {
        unsafe
        {
            if (length > Array.MaxLength || length * (long) sizeof(T) > uint.MaxValue)
                CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
            Debug.Assert(allocatorProviderId != (int) AllocatorTypes.Invalid);
            AllocatorProviderId = allocatorProviderId;
            var size = checked((uint) (sizeof(T) * length));
            if (size > 0)
            {
                var ptr = AllocatorContext.GetAllocator(allocatorProviderId).Allocate(size);
                Debug.Assert(ptr % (nuint) UIntPtr.Size == 0);
                if (!noClear) Unsafe.InitBlock(ptr.ToPointer(), 0, size);
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
        if (length > Array.MaxLength || length * (long) sizeof(T) > uint.MaxValue)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
        _items = buf;
        _length = length;
        AllocatorProviderId = (int) AllocatorTypes.Invalid;
    }

    private unsafe ValueArray(T* buf, int length, int allocatorProviderId)
    {
        Debug.Assert(!(length <= 0 && buf != (void*) 0));
        Debug.Assert((UIntPtr) buf % (nuint) UIntPtr.Size == 0);
        Debug.Assert(length <= Array.MaxLength && length * (long) sizeof(T) <= uint.MaxValue);
        _items = buf;
        _length = length;
        AllocatorProviderId = allocatorProviderId;
    }

    private unsafe T* _items;
    private int _length;
    public readonly int AllocatorProviderId;

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

    public void Dispose()
    {
        unsafe
        {
            if (AllocatorProviderId != (int) AllocatorTypes.Invalid)
            {
                _length = 0;
                if (_items == (void*) 0) return;
                AllocatorContext.GetAllocator(AllocatorProviderId).Free((UIntPtr) _items);
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
    public readonly UnsafeArrayEnumerator<T> GetEnumerator()
    {
        unsafe
        {
            return new UnsafeArrayEnumerator<T>(_items, _length);
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
            Unsafe.InitBlockUnaligned(_items, 0, (uint) (_length * sizeof(T)));
        }
    }

    readonly bool ICollection<T>.Contains(T item) => this.Contains(item);

    readonly void ICollection<T>.CopyTo(T[] array, int arrayIndex) => this.CopyTo(array, arrayIndex);

    bool ICollection<T>.Remove(T item) => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();

    readonly int ICollection<T>.Count => _length;

    readonly bool ICollection<T>.IsReadOnly => false;

    readonly int? IMaybeCountable.MaxCount => _length;

    readonly unsafe T* IUnsafeArray<T>.Items => _items;
    readonly int IUnsafeArray<T>.Length => _length;

    public readonly bool IsInitialized
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
        if (AllocatorProviderId == (int) AllocatorTypes.Invalid)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid allocator provider.");

        unsafe
        {
            var allocator = AllocatorContext.GetAllocator(AllocatorProviderId);
            if (minLength > 0)
            {
#if DEBUG
                var bak = this.AsReadOnlySpan().ToArray();
#endif
                var reallocResult = allocator.TryRealloc((UIntPtr) _items, (nuint) minLength * (nuint) sizeof(T),
                    (nuint) maxLength * (nuint) sizeof(T));
                if (!reallocResult.Success) return -1;
#if DEBUG
                Debug.Assert(MemoryMarshal.CreateReadOnlySpan(in Unsafe.AsRef<T>((T*) reallocResult.Ptr), _length).SequenceEqual(bak));
#endif
                
                Debug.Assert(reallocResult.Ptr % IMemoryAllocator.DefaultAlignment == 0);
                minLength = (int) (reallocResult.ActualSize / (nuint) sizeof(T));
                if (!noClear && minLength > _length)
                    Unsafe.InitBlockUnaligned(((T*) reallocResult.Ptr) + _length, 0,
                        checked((uint) ((minLength - _length) * sizeof(T))));
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
        if (AllocatorProviderId == (int) AllocatorTypes.Invalid)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid allocator provider.");
        var allocator = AllocatorContext.GetAllocator(AllocatorProviderId);
        unsafe
        {
            var size = checked((uint) ((nuint) newLength * (nuint) sizeof(T)));
            var newPtr = allocator.Allocate(size).ToPointer();
            var items = _items;
            if (items != (T*) 0)
            {
                Debug.Assert((UIntPtr) items % (nuint) UIntPtr.Size == 0);
                Debug.Assert((UIntPtr) newPtr % (nuint) UIntPtr.Size == 0);
                Unsafe.CopyBlock(newPtr, items, (uint) (Math.Min(_length, newLength) * sizeof(T)));
                allocator.Free((UIntPtr) items);
            }

            if (!noClear && newLength > _length)
                Unsafe.InitBlockUnaligned(((T*) newPtr) + _length, 0, (uint) ((newLength - _length) * sizeof(T)));
            _items = (T*) newPtr;
            _length = newLength;
        }
    }

    internal int Grow(int minLength, int maxLength, bool noClear = false)
    {
        if (AllocatorProviderId == (int) AllocatorTypes.Invalid)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid allocator provider ID.");
        if (minLength <= _length)
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentOutOfRangeException(nameof(minLength));

        unsafe
        {
            var minSize = checked((uint) ((nuint) minLength * (nuint) sizeof(T)));
#if DEBUG
            var psz = (uint) Math.Min((nuint) maxLength * (nuint) sizeof(T), uint.MaxValue);
            Debug.Assert(minLength != maxLength || psz == maxLength * sizeof(T));
            var maxSize = psz;
#else
            var maxSize = (uint) Math.Min((nuint) maxLength * (nuint) sizeof(T), uint.MaxValue);
#endif
            var allocator = AllocatorContext.GetAllocator(AllocatorProviderId);
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
                    Unsafe.CopyBlock(newPtr.ToPointer(), items, (uint) _length * (uint) sizeof(T));
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
                Unsafe.InitBlockUnaligned(((T*) _items) + _length, 0, (uint) ((minLength - _length) * sizeof(T)));
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
    public static void Copy<T>(ValueArray<T> src, int srcIndex, T[] dest, int destIndex, int count) where T : unmanaged
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
            Unsafe.InitBlock(arr.Items, 0, checked((uint) (arr.Length * sizeof(T))));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ValueArray<T> arr, int startIndex, int count) where T : unmanaged
    {
        unsafe
        {
            Unsafe.InitBlockUnaligned(arr.Items + startIndex, 0, checked((uint) (count * sizeof(T))));
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