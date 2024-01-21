using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Linq;

namespace NullGC.Collections.Extensions;

public static class IUnsafeArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ValueArray<T> src) where T : unmanaged
    {
        return src.AsSpan<T, ValueArray<T>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ValueArray<T> src, int index) where T : unmanaged
    {
        return src.AsSpan<T, ValueArray<T>>(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ValueArray<T> src, int index, int count) where T : unmanaged
    {
        return src.AsSpan<T, ValueArray<T>>(index, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ValueArray<T> src) where T : unmanaged
    {
        return src.AsReadOnlySpan<T, ValueArray<T>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ValueArray<T> src, int index) where T : unmanaged
    {
        return src.AsReadOnlySpan<T, ValueArray<T>>(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ValueArray<T> src, int index, int count) where T : unmanaged
    {
        return src.AsReadOnlySpan<T, ValueArray<T>>(index, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ValueList<T> src) where T : unmanaged
    {
        return src.AsSpan<T, ValueList<T>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ValueList<T> src, int index) where T : unmanaged
    {
        return src.AsSpan<T, ValueList<T>>(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ValueList<T> src, int index, int count) where T : unmanaged
    {
        return src.AsSpan<T, ValueList<T>>(index, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ValueList<T> src) where T : unmanaged
    {
        return src.AsReadOnlySpan<T, ValueList<T>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ValueList<T> src, int index) where T : unmanaged
    {
        return src.AsReadOnlySpan<T, ValueList<T>>(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ValueList<T> src, int index, int count) where T : unmanaged
    {
        return src.AsReadOnlySpan<T, ValueList<T>>(index, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T, TCollection>(this TCollection src)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        unsafe
        {
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(src.Items), src.Length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T, TCollection>(this TCollection src)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        unsafe
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(src.Items), src.Length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T, TCollection>(this TCollection src, int index)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsLessThan(index, src.Length, nameof(index));
        unsafe
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(src.Items + index), src.Length - index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T, TCollection>(this TCollection src, int index, int count)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsLessThanOrEqualTo(count - index, src.Length, nameof(count));
        unsafe
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(src.Items + index), count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T, TCollection>(this TCollection src, int index)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsLessThan(index, src.Length, nameof(index));
        unsafe
        {
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(src.Items + index), src.Length - index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T, TCollection>(this TCollection src, int index, int count)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsLessThanOrEqualTo(count - index, src.Length, nameof(count));
        unsafe
        {
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(src.Items + index), count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this ValueArray<T> src, ValueArray<T> dest) where T : unmanaged
    {
        CopyTo<T, ValueArray<T>, ValueArray<T>>(src, dest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this ValueArray<T> src, ValueArray<T> dest, int srcIndex,
        int destIndex, int srcCountToCopy) where T : unmanaged
    {
        CopyTo<T, ValueArray<T>, ValueArray<T>>(src, dest, srcIndex, destIndex, srcCountToCopy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this ValueArray<T> src, ValueArray<T> dest, int destIndex) where T : unmanaged
    {
        CopyTo<T, ValueArray<T>, ValueArray<T>>(src, dest, 0, destIndex, src.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this ValueArray<T> src, T[] dest) where T : unmanaged
    {
        CopyTo<T, ValueArray<T>>(src, dest, 0, 0, src.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this ValueArray<T> src, T[] dest, int srcIndex,
        int destIndex, int srcCountToCopy) where T : unmanaged
    {
        CopyTo<T, ValueArray<T>>(src, dest, srcIndex, destIndex, srcCountToCopy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this ValueArray<T> src, T[] dest, int destIndex) where T : unmanaged
    {
        CopyTo<T, ValueArray<T>>(src, dest, 0, destIndex, src.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this ValueArray<T> src, T key) where T : unmanaged
    {
        for (int i = 0; i < src.Length; i++)
            if (EqualityComparer<T>.Default.Equals(src.GetUnchecked(i), key)) return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T, TCollection1, TCollection2>(this TCollection1 src, TCollection2 dest, int srcIndex,
        int destIndex, int srcCountToCopy)
        where TCollection1 : IUnmanagedArray<T> where TCollection2 : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsLessThanOrEqualTo(srcCountToCopy, dest.Length - destIndex);
        unsafe
        {
            Unsafe.CopyBlockUnaligned(dest.Items + destIndex, src.Items + srcIndex,
                checked((uint) (sizeof(T) * srcCountToCopy)));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T, TCollection1>(this TCollection1 src, IList<T> dest, int srcIndex,
        int destIndex, int srcCountToCopy)
        where TCollection1 : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsLessThanOrEqualTo(srcCountToCopy, dest.Count - destIndex);

        unsafe
        {
            for (int i = srcIndex; i < srcIndex + srcCountToCopy; i++)
            {
                dest[destIndex++] = src.Items[i];
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T, TCollection1, TCollection2>(this TCollection1 src, TCollection2 dest)
        where TCollection1 : IUnmanagedArray<T> where TCollection2 : IUnmanagedArray<T> where T : unmanaged
    {
        Guard.IsGreaterThanOrEqualTo(dest.Length, src.Length, nameof(dest));
        unsafe
        {
            Unsafe.CopyBlock(dest.Items, src.Items, checked((uint) (sizeof(T) * src.Length)));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T, TCollection>(this TCollection src)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        unsafe
        {
            Debug.Assert((nuint)src.Items % (nuint)UIntPtr.Size == 0);
            Unsafe.InitBlock(src.Items, 0, checked((uint) (src.Length * sizeof(T))));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ValueList<T> src) where T : unmanaged
    {
        Clear<T, ValueList<T>>(src);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ValueArray<T> src) where T : unmanaged
    {
        Clear<T, ValueArray<T>>(src);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ValueList<T> src, int index, int count) where T : unmanaged
    {
        Clear<T, ValueList<T>>(src, index, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ValueArray<T> src, int index, int count) where T : unmanaged
    {
        Clear<T, ValueArray<T>>(src, index, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T, TCollection>(this TCollection src, int index, int count)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        unsafe
        {
            Debug.Assert((nuint)src.Items % (nuint)UIntPtr.Size == 0);
            Unsafe.InitBlockUnaligned(src.Items + index, 0, checked((uint) (count * sizeof(T))));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T, TCollection>(this TCollection src,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        var ret = new ValueList<T>(src.Length, allocatorProviderId);
        src.CopyTo<T, TCollection, ValueList<T>>(ret);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this ValueList<T> src,
        int allocatorProviderId = (int) AllocatorTypes.Default) where T : unmanaged
    {
        return ToValueList<T, ValueList<T>>(src, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T>(this ValueArray<T> src,
        int allocatorProviderId = (int) AllocatorTypes.Default) where T : unmanaged
    {
        return ToValueList<T, ValueArray<T>>(src, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T, TCollection>(this TCollection src,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where TCollection : IUnmanagedArray<T> where T : unmanaged
    {
        var ret = new ValueArray<T>(src.Length, allocatorProviderId);
        src.CopyTo<T, TCollection, ValueArray<T>>(ret);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T>(this ValueArray<T> src,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged
    {
        return ToValueArray<T, ValueArray<T>>(src, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T>(this ValueList<T> src,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged
    {
        return ToValueArray<T, ValueList<T>>(src, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T>(this IEnumerable<T> src,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged
    {
        if (src.GetType().IsAssignableTo(typeof(ValueArray<T>)))
        {
            return ToValueArray<T, ValueArray<T>>((ValueArray<T>) src, allocatorProviderId);
        }
        else if (src.GetType().IsAssignableTo(typeof(ValueList<T>)))
        {
            return ToValueArray<T, ValueList<T>>((ValueList<T>) src, allocatorProviderId);
        }
        else if (src.GetType().IsAssignableTo(typeof(ValueStack<T>)))
        {
            return ToValueArray<T, ValueStack<T>>((ValueStack<T>) src, allocatorProviderId);
        }

        var count = 0;

        if (src.GetType().IsAssignableTo(typeof(IReadOnlyCollection<T>)))
        {
            count = ((IReadOnlyCollection<T>) src).Count;
        }
        else if (src.GetType().IsAssignableTo(typeof(ICollection<T>)))
        {
            count = ((ICollection<T>) src).Count;
        }
        else if (src.GetType().IsAssignableTo(typeof(IMaybeCountable)))
        {
            var mc = ((IMaybeCountable) src).MaxCount;
            if (mc.HasValue) count = mc.Value;
        }

        var ret = new ValueList<T>(count, allocatorProviderId);
        foreach (var item in src) ret.Add(item);

        if (ret.TryConvertSelfToArray(out var arr)) return arr;
        return arr.ToValueArray();
    }
}