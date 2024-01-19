using System.Runtime.CompilerServices;

namespace NullGC.Linq;

internal static class LinqHelper
{
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static bool TrySetTakeCount<TCollection>(ref TCollection src, int count) where TCollection : struct
    // {
    //     if (!typeof(TCollection).IsAssignableTo(typeof(ISkipTakeAware)))
    //         return false;
    //
    //     return ((ISkipTakeAware) src).SetTakeCount(count);
    // }
    //
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static bool TrySetSkipCount<TCollection>(ref TCollection src, int count) where TCollection : struct
    // {
    //     if (!typeof(TCollection).IsAssignableTo(typeof(ISkipTakeAware)))
    //         return false;
    //
    //     return ((ISkipTakeAware) src).SetSkipCount(count);
    // }
}