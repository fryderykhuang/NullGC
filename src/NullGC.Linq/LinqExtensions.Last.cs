using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Last

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (var item in src)
        {
            ret = item;
            hasValue = true;
        }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        T? ret = default;
        Unsafe.SkipInit(out ret);
        foreach (var item in src) ret = item;

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (ref var item in src)
        {
            ret = item;
            hasValue = true;
        }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (ref var item in src)
        {
            ret = item;
            hasValue = true;
        }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        T? ret = default;
        foreach (ref var item in src) ret = item;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        T? ret = default;
        foreach (ref var item in src) ret = item;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, bool> predicate)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (var item in src)
            if (predicate(item))
            {
                ret = item;
                hasValue = true;
            }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, bool> predicate)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        T? ret = default;
        foreach (var item in src)
            if (predicate(item))
                ret = item;

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        T? ret = default;
        foreach (ref var item in src)
            if (predicate(in item))
                ret = item;

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (ref var item in src)
            if (predicate(in item))
            {
                ret = item;
                hasValue = true;
            }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (ref var item in src)
            if (predicate(in item))
            {
                ret = item;
                hasValue = true;
            }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src,
        FuncT1In<T, bool> predicate)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        T? ret = default;
        foreach (var item in src)
            if (predicate(in item))
                ret = item;

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator, TArg>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (ref var item in src)
            if (predicate(in item, arg))
            {
                ret = item;
                hasValue = true;
            }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator, TArg>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (ref var item in src)
            if (predicate(in item, arg))
            {
                ret = item;
                hasValue = true;
            }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator, TArg>(this LinqRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        T? ret = default;
        foreach (ref var item in src)
            if (predicate(in item, arg))
                ret = item;

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator, TArg>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        T? ret = default;
        foreach (ref var item in src)
            if (predicate(in item, arg))
                ret = item;

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T, TEnumerator, TArg>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        bool hasValue = false;
        T ret;
        Unsafe.SkipInit(out ret);
        foreach (var item in src)
            if (predicate(item, arg))
            {
                ret = item;
                hasValue = true;
            }

        return hasValue ? ret : ThrowHelper.SequenceContainsNoElement<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? LastOrDefault<T, TEnumerator, TArg>(this LinqValueEnumerable<T, TEnumerator> src,
        Func<T, TArg, bool> predicate, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        T? ret = default;
        foreach (var item in src)
            if (predicate(item, arg))
                ret = item;

        return ret;
    }
    #endregion
}