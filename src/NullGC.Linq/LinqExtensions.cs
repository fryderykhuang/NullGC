using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    // Every extension methods with argument RefEnumerable<> are marked as AggressiveInlining because copying is unnecessary
    // and possibly expensive (long operator chain). While ref can be used, the extra indirection is also unnecessary. 

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToList<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed where T : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueList<T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToList<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T> where T : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueList<T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueList<T> ToList<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T> where T : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueList<T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, T> ToDictionary<TKey, T, TEnumerator>(
        this LinqFixedRefEnumerable<T, TEnumerator> src, FuncT1In<T, TKey> keySelector)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed where T : unmanaged where TKey : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(keySelector(in enumerator.Current), enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, T> ToDictionary<TKey, T, TEnumerator>(
        this LinqRefEnumerable<T, TEnumerator> src, FuncT1In<T, TKey> keySelector)
        where TEnumerator : struct, ILinqRefEnumerator<T> where T : unmanaged where TKey : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(keySelector(in enumerator.Current), enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, T> ToDictionary<TKey, T, TEnumerator>(
        this LinqValueEnumerable<T, TEnumerator> src, Func<T, TKey> keySelector)
        where TEnumerator : struct, ILinqValueEnumerator<T> where T : unmanaged where TKey : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            ret.Add(keySelector(current), current);
        }

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, T> ToDictionary<TKey, T, TEnumerator, TArg>(
        this LinqValueEnumerable<T, TEnumerator> src, Func<T, TArg, TKey> keySelector, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T> where T : unmanaged where TKey : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            ret.Add(keySelector(current, arg), current);
        }

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, T> ToDictionary<TKey, T, TEnumerator, TArg>(
        this LinqFixedRefEnumerable<T, TEnumerator> src, FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed where T : unmanaged where TKey : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(keySelector(in enumerator.Current, arg), enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, T> ToDictionary<TKey, T, TEnumerator, TArg>(
        this LinqRefEnumerable<T, TEnumerator> src, FuncT1In<T, TArg, TKey> keySelector, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T> where T : unmanaged where TKey : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, T>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext()) ret.Add(keySelector(in enumerator.Current, arg), enumerator.Current);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToDictionary<TKey, T, TValue, TEnumerator, TArg>(
        this LinqValueEnumerable<T, TEnumerator> src, Func<T, TArg, TKey> keySelector,
        Func<T, TArg, TValue> valueSelector, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T>
        where T : unmanaged
        where TKey : unmanaged
        where TValue : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, TValue>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            ret.Add(keySelector(current, arg), valueSelector(current, arg));
        }

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToDictionary<TKey, T, TValue, TEnumerator, TArg>(
        this LinqFixedRefEnumerable<T, TEnumerator> src, FuncT1In<T, TArg, TKey> keySelector,
        FuncT1In<T, TArg, TValue> valueSelector, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
        where T : unmanaged
        where TKey : unmanaged
        where TValue : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, TValue>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext())
            ret.Add(keySelector(in enumerator.Current, arg), valueSelector(in enumerator.Current, arg));
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueDictionary<TKey, TValue> ToDictionary<TKey, T, TValue, TEnumerator, TArg>(
        this LinqRefEnumerable<T, TEnumerator> src, FuncT1In<T, TArg, TKey> keySelector,
        FuncT1In<T, TArg, TValue> valueSelector, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T>
        where T : unmanaged
        where TKey : unmanaged
        where TValue : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, TValue>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext())
            ret.Add(keySelector(in enumerator.Current, arg), valueSelector(in enumerator.Current, arg));
        return ret;
    }


    #region Value collection types as RefEnumerable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, ValueFixedSizeDeque<T>.ForwardEnumerator> LinqRef<T>(
        [ReadOnly] this ValueFixedSizeDeque<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, ValueFixedSizeDeque<T>.ForwardEnumerator>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, ValueQueue<T>.Enumerator> LinqRef<T>([ReadOnly] this ValueQueue<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, ValueQueue<T>.Enumerator>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, ValueStack<T>.Enumerator> LinqRef<T>([ReadOnly] this ValueStack<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, ValueStack<T>.Enumerator>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>> LinqRef<T>([ReadOnly] this ValueList<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ValueFixedSizeDeque<T>.ForwardEnumerator> LinqValue<T>(
        [ReadOnly] this ValueFixedSizeDeque<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ValueFixedSizeDeque<T>.ForwardEnumerator>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ValueQueue<T>.Enumerator> LinqValue<T>([ReadOnly] this ValueQueue<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ValueQueue<T>.Enumerator>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ValueStack<T>.Enumerator> LinqValue<T>([ReadOnly] this ValueStack<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ValueStack<T>.Enumerator>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>> LinqValue<T>([ReadOnly] this ValueList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>> LinqPtr<T>([ReadOnly] this ValueList<T> src)
        where T : unmanaged
    {
        return new LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>> LinqRef<T>([ReadOnly] this ValueArray<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator> LinqRef<T>(
        [ReadOnly] this ValueLinkedList<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator>(
            src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>> LinqPtr<T>([ReadOnly] this ValueArray<T> src)
        where T : unmanaged
    {
        return new LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, ArrayLinqRefEnumerator<T>> LinqRef<T>([ReadOnly] this T[] src)
        where T : unmanaged
    {
        return new LinqRefEnumerable<T, ArrayLinqRefEnumerator<T>>(new ArrayLinqRefEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ArrayLinqRefEnumerator<T>> LinqValue<T>([ReadOnly] this T[] src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ArrayLinqRefEnumerator<T>>(new ArrayLinqRefEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IListLinqValueEnumerator<T>> LinqValue<T>([ReadOnly] this IList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IListLinqValueEnumerator<T>>(new IListLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IReadOnlyListLinqValueEnumerator<T>> LinqValue<T>(
        [ReadOnly] this IReadOnlyList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IReadOnlyListLinqValueEnumerator<T>>(
            new IReadOnlyListLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ICollectionLinqValueEnumerator<T>> LinqValue<T>(
        [ReadOnly] this ICollection<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ICollectionLinqValueEnumerator<T>>(
            new ICollectionLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IReadOnlyCollectionLinqValueEnumerator<T>> LinqValue<T>(
        [ReadOnly] this IReadOnlyCollection<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IReadOnlyCollectionLinqValueEnumerator<T>>(
            new IReadOnlyCollectionLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IEnumerableLinqValueEnumerator<T>> LinqValue<T>(
        [ReadOnly] this IEnumerable<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IEnumerableLinqValueEnumerator<T>>(
            new IEnumerableLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>> LinqValue<T>([ReadOnly] this ValueArray<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator> LinqValue<T>(
        [ReadOnly] this ValueLinkedList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator>(
            src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<ValueDictionary<TKey, TValue>.Entry, ValueDictionary<TKey, TValue>.Enumerator>
        LinqRef<TKey, TValue>([ReadOnly] this ValueDictionary<TKey, TValue> src)
        where TValue : unmanaged where TKey : unmanaged
    {
        return new
            LinqFixedRefEnumerable<ValueDictionary<TKey, TValue>.Entry, ValueDictionary<TKey, TValue>.Enumerator>(
                src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<TKey, ValueDictionary<TKey, TValue>.KeyCollection.Enumerator>
        LinqRef<TKey, TValue>([ReadOnly] this ValueDictionary<TKey, TValue>.KeyCollection src)
        where TValue : unmanaged where TKey : unmanaged
    {
        return new LinqFixedRefEnumerable<TKey, ValueDictionary<TKey, TValue>.KeyCollection.Enumerator>(
            src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<TValue, ValueDictionary<TKey, TValue>.ValueCollection.Enumerator>
        LinqRef<TKey, TValue>([ReadOnly] this ValueDictionary<TKey, TValue>.ValueCollection src)
        where TValue : unmanaged where TKey : unmanaged
    {
        return new LinqFixedRefEnumerable<TValue, ValueDictionary<TKey, TValue>.ValueCollection.Enumerator>(
            src.GetEnumerator());
    }

    #endregion
}