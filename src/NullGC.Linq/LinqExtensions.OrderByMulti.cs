using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region OrderBy_Multi

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqRefEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqRefEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqFixedRefEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqRefEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqRefEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqFixedRefEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqValueEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqValueEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
            TNext>>
        OrderBy<T, TPrevious, TKey, TNext>(this LinqPtrEnumerable<T, TPrevious> src,
            ValueEnumerableSorter<T, TKey, FuncT1PtrInvoker<T, TKey>, Comparer<TKey>, TNext> sorter)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
        where TKey : unmanaged
        where T : unmanaged
        where TNext : struct, IValueEnumerableSorter<T>
    {
        return new LinqPtrEnumerable<T,
            OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>>(
            new OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>, TNext>(
                src.GetEnumerator(), sorter));
    }

    #endregion
}