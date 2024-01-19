using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NullGC.Collections;

namespace NullGC.Linq;

public static class SpanHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> GetSpanOrDefault<T, TCollection>(TCollection src, out bool success) where TCollection : struct
    {
        if (typeof(TCollection).IsAssignableTo(typeof(IUnsafeArray<T>)))
        {
            unsafe
            {
                success = true;
                return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(((IUnsafeArray<T>) src).Items),
                    ((IUnsafeArray<T>) src).Length);
            }
        }
        else if (typeof(TCollection).IsAssignableTo(typeof(IArray<T>)))
        {
            success = true;
            return new ArraySegment<T>(((IArray<T>) src).Items, 0, ((IArray<T>) src).Length);
        }

        success = false;
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> GetReadOnlySpanOrDefault<T, TCollection>(TCollection src, out bool success) where TCollection : struct
    {
        return GetSpanOrDefault<T, TCollection>(src, out success);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T, TCollection>(TCollection src) where TCollection : IUnsafeArray<T>
    {
        unsafe
        {
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(src.Items), src.Length);
        }
    }

}