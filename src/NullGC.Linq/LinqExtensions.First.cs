﻿using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region First

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T First<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        ref var ret = ref FirstOrNullRef(src);
        if (Unsafe.IsNullRef(ref ret)) ThrowHelper.SequenceContainsNoElement();
        return ref ret!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T First<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        ref var ret = ref FirstOrNullRef(src);
        if (Unsafe.IsNullRef(ref ret)) ThrowHelper.SequenceContainsNoElement();
        return ref ret!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T First<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        foreach (var item in src)
            return item;

        return ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        foreach (var item in src)
            return item;

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T First<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        ref var ret = ref FirstOrNullRef(src, predicate);
        if (Unsafe.IsNullRef(ref ret)) ThrowHelper.SequenceContainsNoElement();
        return ref ret!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T First<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        ref var ret = ref FirstOrNullRef(src, predicate);
        if (Unsafe.IsNullRef(ref ret)) ThrowHelper.SequenceContainsNoElement();
        return ref ret!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T First<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, bool> predicate)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        foreach (var item in src)
            if (predicate(item))
                return item;

        return ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T FirstOrNullRef<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        foreach (ref var item in src)
            return ref item;

        return ref Unsafe.NullRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T FirstOrNullRef<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        foreach (ref var item in src)
            return ref item;

        return ref Unsafe.NullRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        foreach (ref var item in src)
            return item;

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T FirstOrNullRef<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        foreach (ref var item in src)
            if (predicate(in item))
                return ref item;

        return ref Unsafe.NullRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T FirstOrNullRef<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        foreach (ref var item in src)
            if (predicate(in item))
                return ref item;

        return ref Unsafe.NullRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, bool> predicate)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        foreach (var item in src)
            if (predicate(item))
                return item;

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        foreach (ref var item in src)
            if (predicate(in item))
                return item;

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        foreach (ref var item in src)
            if (predicate(in item))
                return item;

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T First<T, TEnumerator, TArg>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        ref var ret = ref FirstOrNullRef<T, TEnumerator, TArg>(src, predicate, arg);
        if (Unsafe.IsNullRef(ref ret)) ThrowHelper.SequenceContainsNoElement();
        return ref ret!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T First<T, TEnumerator, TArg>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        ref var ret = ref FirstOrNullRef<T, TEnumerator, TArg>(src, predicate, arg);
        if (Unsafe.IsNullRef(ref ret)) ThrowHelper.SequenceContainsNoElement();
        return ref ret!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator, TArg>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        ref var ret = ref FirstOrNullRef<T, TEnumerator, TArg>(src, predicate, arg);
        if (Unsafe.IsNullRef(ref ret)) return default;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator, TArg>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        ref var ret = ref FirstOrNullRef<T, TEnumerator, TArg>(src, predicate, arg);
        if (Unsafe.IsNullRef(ref ret)) return default;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T First<T, TEnumerator, TArg>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        foreach (var item in src)
            if (predicate(item, arg))
                return item;

        return ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T FirstOrNullRef<T, TEnumerator, TArg>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        foreach (ref var item in src)
            if (predicate(in item, arg))
                return ref item;

        return ref Unsafe.NullRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T FirstOrNullRef<T, TEnumerator, TArg>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        foreach (ref var item in src)
            if (predicate(in item, arg))
                return ref item;

        return ref Unsafe.NullRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T, TEnumerator, TArg>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        foreach (var item in src)
            if (predicate(item, arg))
                return item;

        return default;
    }

    #endregion
}