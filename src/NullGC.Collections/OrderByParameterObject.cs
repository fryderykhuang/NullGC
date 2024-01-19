// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using NullGC.Allocators;

namespace NullGC.Collections;

public static class OrderBy
{
    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> Ascending<T, TKey, TNext>(
        Func<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false, next);
    }

    public static
        ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>> Ascending<T, TKey, TNext>(
            Func<T, TKey> keySelector, ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false,
            next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> Descending<T, TKey, TNext>(
        Func<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true,
            next);
    }

    public static
        ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>> Descending<T, TKey, TNext>(
            Func<T, TKey> keySelector, ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true,
            next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> Ascending<T, TKey, TNext>(
        FuncT1In<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false, next);
    }

    public static
        ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>> Ascending<T, TKey, TNext>(
            FuncT1In<T, TKey> keySelector,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false, next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> Descending<T, TKey, TNext>(
        FuncT1In<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true, next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey2, FuncT1InInvoker<T, TKey2>, Comparer<TKey2>, TNext>>
        Descending<T, TKey, TKey2, TNext>(
            FuncT1In<T, TKey> keySelector,
            ValueEnumerableSorter<T, TKey2, FuncT1InInvoker<T, TKey2>, Comparer<TKey2>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T> where TKey2 : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey2, FuncT1InInvoker<T, TKey2>, Comparer<TKey2>, TNext>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true, next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Ascending<T,
        TKey>(
        Func<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false);
    }

    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Descending<T,
        TKey>(
        Func<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Ascending<T,
        TKey>(
        FuncT1In<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Descending<T,
        TKey>(
        FuncT1In<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true);
    }
}
