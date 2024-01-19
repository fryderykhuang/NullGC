using System.Runtime.CompilerServices;
using NullGC.Allocators;
using NullGC.Linq;

namespace NullGC.Collections.Extensions;

public static class ValueCollectionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T, TCollection>(this TCollection src, ValueArray<T> dest, int destIndex)
        where T : unmanaged where TCollection : IEnumerable<T>
    {
        foreach (var item in src)
        {
            dest.GetRefUnchecked(destIndex++) = item;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<TKey> ToValueList<TKey, TValue>(this ValueDictionary<TKey, TValue>.KeyCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default) where TKey : unmanaged where TValue : unmanaged
    {
        return ToValueList<TKey, ValueDictionary<TKey, TValue>.KeyCollection>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<TValue> ToValueList<TKey, TValue>(
        this ValueDictionary<TKey, TValue>.ValueCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default) where TKey : unmanaged where TValue : unmanaged
    {
        return ToValueList<TValue, ValueDictionary<TKey, TValue>.ValueCollection>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        return ToValueList<T, LinqRefEnumerable<T, TEnumerator>, TEnumerator>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        return ToValueList<T, LinqValueEnumerable<T, TEnumerator>, TEnumerator>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<TKey> ToValueArray<TKey, TValue>(
        this ValueDictionary<TKey, TValue>.KeyCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default) where TKey : unmanaged where TValue : unmanaged
    {
        return ToValueArray<TKey, ValueDictionary<TKey, TValue>.KeyCollection>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<TValue> ToValueArray<TKey, TValue>(
        this ValueDictionary<TKey, TValue>.ValueCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default) where TKey : unmanaged where TValue : unmanaged
    {
        return ToValueArray<TValue, ValueDictionary<TKey, TValue>.ValueCollection>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        return ToValueArray<T, LinqRefEnumerable<T, TEnumerator>, TEnumerator>(collection, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        return ToValueArray<T, LinqValueEnumerable<T, TEnumerator>, TEnumerator>(collection, allocatorProviderId);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T, TCollection, TEnumerator>(this TCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TCollection : ILinqEnumerable<T, TEnumerator> where TEnumerator : struct, ILinqEnumerator<T>
    {
        int count = 0;
        if (typeof(TCollection).IsAssignableTo(typeof(IReadOnlyCollection<T>)))
        {
            count = ((IReadOnlyCollection<T>) collection).Count;
        }
        else if (typeof(TCollection).IsAssignableTo(typeof(IMaybeCountable)))
        {
            var cc = ((IMaybeCountable) collection).Count;
            if (cc.HasValue)
                count = cc.Value;
            else
            {
                var mc = ((IMaybeCountable) collection).MaxCount;
                if (mc.HasValue)
                    count = mc.Value;
            }
        }

        var ret = new ValueList<T>(count, allocatorProviderId);

        using (var e = collection.GetEnumerator())
        {
            if (typeof(TEnumerator).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
            {
                while (e.MoveNext())
                {
                    ret.Add(((ILinqRefEnumerator<T>) e).Current);
                }
            }
            else if (typeof(TEnumerator).IsAssignableTo(typeof(ILinqValueEnumerator<T>)))
            {
                while (e.MoveNext())
                {
                    ret.Add(((ILinqValueEnumerator<T>) e).Current);
                }
            }
        }

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToValueList<T, TCollection>(this TCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TCollection : IEnumerable<T>
    {
        int count = 0;
        if (typeof(TCollection).IsAssignableTo(typeof(IReadOnlyCollection<T>)))
        {
            count = ((IReadOnlyCollection<T>) collection).Count;
        }
        else if (typeof(TCollection).IsAssignableTo(typeof(IMaybeCountable)))
        {
            var cc = ((IMaybeCountable) collection).Count;
            if (cc.HasValue)
                count = cc.Value;
            else
            {
                var mc = ((IMaybeCountable) collection).MaxCount;
                if (mc.HasValue)
                    count = mc.Value;
            }
        }

        var ret = new ValueList<T>(count, allocatorProviderId);

        foreach (var item in collection)
        {
            ret.Add(item);
        }

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T, TCollection, TEnumerator>(this TCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TCollection : ILinqEnumerable<T, TEnumerator> where TEnumerator : struct, ILinqEnumerator<T>
    {
        var lst = ToValueList<T, TCollection, TEnumerator>(collection, allocatorProviderId);
        if (lst.TryConvertSelfToArray(out var ret))
            return ret;
        return lst.ToValueArray();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueArray<T> ToValueArray<T, TCollection>(this TCollection collection,
        int allocatorProviderId = (int) AllocatorTypes.Default)
        where T : unmanaged where TCollection : IEnumerable<T>
    {
        var lst = ToValueList<T, TCollection>(collection, allocatorProviderId);
        if (lst.TryConvertSelfToArray(out var ret))
            return ret;
        return lst.ToValueArray();
    }
}