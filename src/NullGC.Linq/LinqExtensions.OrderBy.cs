using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region OrderBy

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqRefEnumerable<T, TPrevious> src, FuncT1In<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T,
            Comparer<T>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious>(this LinqRefEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqRefEnumerator<T> where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(), null, Comparer<T>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqFixedRefEnumerable<T, TPrevious> src, FuncT1In<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T,
            Comparer<T>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious>(this LinqFixedRefEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(), null, Comparer<T>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqRefEnumerable<T, TPrevious> src, Func<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqFixedRefEnumerable<T, TPrevious> src, Func<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1TRRefInvoker<T, TKey>, TKey,
    //         Comparer<TKey>, DummyEnumerableSorter<T>>>
    //     OrderBy<T, TPrevious, TKey>(this LinqFixedRefEnumerable<T, TPrevious> src, FuncT1TRRef<T, TKey> keySelector)
    //     where TPrevious : struct, ILinqRefEnumerator<T>, IObjectAddressFixed where TKey : unmanaged where T : unmanaged
    // {
    //     return new LinqFixedRefEnumerable<T,
    //         OrderByFixedRefToFixedRef<T, TPrevious, FuncT1TRRefInvoker<T, TKey>, TKey, Comparer<TKey>,
    //             DummyEnumerableSorter<T>>>(
    //         new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1TRRefInvoker<T, TKey>, TKey, Comparer<TKey>,
    //             DummyEnumerableSorter<T>>(
    //             src.GetEnumerator(),
    //             new FuncT1TRRefInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqValueEnumerable<T, TPrevious> src, FuncT1In<T, TKey> keySelector)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T,
            Comparer<T>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious>(this LinqValueEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqValueEnumerator<T> where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(), null, Comparer<T>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqValueEnumerable<T, TPrevious> src, Func<T, TKey> keySelector)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
            DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(this LinqPtrEnumerable<T, TPrevious> src, FuncT1Ptr<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqPtrEnumerable<T,
            OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1PtrInvoker<T, TKey>(keySelector), Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, T>, T, Comparer<T>,
            DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious>(this LinqPtrEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where T : unmanaged
    {
        return new LinqPtrEnumerable<T,
            OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(), null, Comparer<T>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(
            this LinqRefEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static LinqFixedRefEnumerable<T, OrderByPtr<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>>>
    //     OrderBy<T, TPrevious, TKey>(
    //         this LinqFixedRefEnumerable<T, TPrevious> src,
    //         FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
    //     where TPrevious : struct, ILinqRefEnumerator<T>, IObjectAddressFixed where TKey : unmanaged where T : unmanaged
    // {
    //     return new LinqFixedRefEnumerable<T,
    //         OrderByPtr<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>>>(
    //         new OrderByPtr<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>>(
    //             src.GetEnumerator(),
    //             new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(
            this LinqRefEnumerable<T, TPrevious> src,
            Func<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(
            this LinqFixedRefEnumerable<T, TPrevious> src,
            Func<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(
            this LinqFixedRefEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(
            this LinqValueEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderBy<T, TPrevious, TKey>(
            this LinqValueEnumerable<T, TPrevious> src,
            Func<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer)));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderBy<T, TPrevious, TKey, TArg>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious,
        FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderBy<T, TPrevious, TKey, TArg>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious,
            FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderBy<T, TPrevious, TKey, TArg>(
        this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncWithArgInvoker<T, TArg, TKey>,
        TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderBy<T, TPrevious, TKey, TArg>(
        this LinqValueEnumerable<T, TPrevious> src,
        Func<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncWithArgInvoker<T, TArg, TKey>,
            TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>> OrderBy<T, TPrevious, TKey, TArg>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector,
        ComparisonWithArg<TKey, TArg> comparer, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
                StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                new StructComparerWithArg<TKey, TArg>(comparer, arg)));
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static LinqFixedRefEnumerable<T, OrderByPtr<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
    //     StructComparerWithArg<TKey, TArg>>> OrderBy<T, TPrevious, TKey, TArg>(
    //     this LinqFixedRefEnumerable<T, TPrevious> src,
    //     FuncT1In<T, TArg, TKey> keySelector,
    //     ComparisonWithArg<TKey, TArg> comparer, TArg arg)
    //     where TPrevious : struct, ILinqRefEnumerator<T>, IObjectAddressFixed
    //     where TKey : unmanaged
    //     where T : unmanaged
    //     where TArg : unmanaged
    // {
    //     return new LinqFixedRefEnumerable<T, OrderByPtr<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
    //         StructComparerWithArg<TKey, TArg>>>(
    //         new OrderByPtr<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
    //             StructComparerWithArg<TKey, TArg>>(src.GetEnumerator(),
    //             new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
    //             new StructComparerWithArg<TKey, TArg>(comparer, arg)));
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>> OrderBy<T, TPrevious, TKey, TArg>(
        this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector,
        ComparisonWithArg<TKey, TArg> comparer, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
                StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                new StructComparerWithArg<TKey, TArg>(comparer, arg)));
    }

    #endregion

    #region OrderByDescending

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqRefEnumerable<T, TPrevious> src, FuncT1In<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T,
            Comparer<T>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious>(this LinqRefEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqRefEnumerator<T> where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(src.GetEnumerator(), null, Comparer<T>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqFixedRefEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T,
            Comparer<T>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious>(this LinqFixedRefEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(src.GetEnumerator(), null, Comparer<T>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqRefEnumerable<T, TPrevious> src, Func<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqFixedRefEnumerable<T, TPrevious> src, Func<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqValueEnumerable<T, TPrevious> src, FuncT1In<T, TKey> keySelector)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T,
            Comparer<T>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious>(this LinqValueEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqValueEnumerator<T> where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(), null, Comparer<T>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqValueEnumerable<T, TPrevious> src, Func<T, TKey> keySelector)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
            DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(this LinqPtrEnumerable<T, TPrevious> src, FuncT1Ptr<T, TKey> keySelector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqPtrEnumerable<T,
            OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1PtrInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, T>, T, Comparer<T>,
            DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious>(this LinqPtrEnumerable<T, TPrevious> src)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where T : unmanaged
    {
        return new LinqPtrEnumerable<T,
            OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>>(
            new OrderByPtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, T>, T, Comparer<T>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(), null, Comparer<T>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(
            this LinqRefEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer), true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(
            this LinqFixedRefEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer), true));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(
            this LinqRefEnumerable<T, TPrevious> src,
            Func<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer), true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(
            this LinqFixedRefEnumerable<T, TPrevious> src,
            Func<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer), true));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(
            this LinqValueEnumerable<T, TPrevious> src,
            FuncT1In<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer), true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey,
            StructComparer<TKey>, DummyEnumerableSorter<T>>>
        OrderByDescending<T, TPrevious, TKey>(
            this LinqValueEnumerable<T, TPrevious> src,
            Func<T, TKey> keySelector, Comparison<TKey> comparer)
        where TPrevious : struct, ILinqValueEnumerator<T> where TKey : unmanaged where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T,
            OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncInvoker<T, TKey>, TKey, StructComparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncInvoker<T, TKey>(keySelector), new StructComparer<TKey>(comparer), true));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious,
        FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious,
            FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default, true));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncWithArgInvoker<T, TArg, TKey>,
        TKey,
        Comparer<TKey>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqValueEnumerable<T, TPrevious> src,
        Func<T, TArg, TKey> keySelector, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncWithArgInvoker<T, TArg, TKey>,
            TKey,
            Comparer<TKey>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncWithArgInvoker<T, TArg, TKey>, TKey, Comparer<TKey>,
                DummyEnumerableSorter<T>>(
                src.GetEnumerator(),
                new FuncWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                Comparer<TKey>.Default, true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector,
        ComparisonWithArg<TKey, TArg> comparer, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>>(
            new OrderByRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
                StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                new StructComparerWithArg<TKey, TArg>(comparer, arg), true));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious,
        FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
        StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector,
        ComparisonWithArg<TKey, TArg> comparer, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious,
            FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
            StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>>(
            new OrderByFixedRefToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
                StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                new StructComparerWithArg<TKey, TArg>(comparer, arg), true));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
        TKey,
        StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>> OrderByDescending<T, TPrevious, TKey, TArg>(
        this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TKey> keySelector,
        ComparisonWithArg<TKey, TArg> comparer, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T>
        where TKey : unmanaged
        where T : unmanaged
        where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>,
            TKey,
            StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>>(
            new OrderByValueToFixedRef<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TKey>, TKey,
                StructComparerWithArg<TKey, TArg>, DummyEnumerableSorter<T>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TKey>(keySelector, arg),
                new StructComparerWithArg<TKey, TArg>(comparer, arg), true));
    }

    #endregion
}