using System.Numerics;
using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    // Every extension methods with argument RefEnumerable<> are marked as AggressiveInlining because copying is unnecessary
    // and possibly expensive (long operator chain). While ref can be used, the extra indirection is also unnecessary. 

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
        this LinqValueEnumerable<T, TEnumerator> src, Func<T, TArg, TKey> keySelector, Func<T, TArg, TValue> valueSelector, TArg arg)
        where TEnumerator : struct, ILinqValueEnumerator<T> where T : unmanaged where TKey : unmanaged where TValue : unmanaged
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
        this LinqRefEnumerable<T, TEnumerator> src, FuncT1In<T, TArg, TKey> keySelector, FuncT1In<T, TArg, TValue> valueSelector, TArg arg)
        where TEnumerator : struct, ILinqRefEnumerator<T> where T : unmanaged where TKey : unmanaged where TValue : unmanaged
    {
        using var enumerator = src.GetEnumerator();
        var ret = new ValueDictionary<TKey, TValue>(enumerator.MaxCount ?? 0);
        while (enumerator.MoveNext())
            ret.Add(keySelector(in enumerator.Current, arg), valueSelector(in enumerator.Current, arg));
        return ret;
    }

    
    #region Value collection types as RefEnumerable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>> LinqRef<T>(this ValueList<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>> LinqValue<T>(this ValueList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>> LinqPtr<T>(this ValueList<T> src)
        where T : unmanaged
    {
        return new LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>> LinqRef<T>(this ValueArray<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator> LinqRef<T>(
        this ValueLinkedList<T> src)
        where T : unmanaged
    {
        return new LinqFixedRefEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator>(
            src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>> LinqPtr<T>(this ValueArray<T> src)
        where T : unmanaged
    {
        return new LinqPtrEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, ArrayLinqRefEnumerator<T>> LinqRef<T>(this T[] src)
        where T : unmanaged
    {
        return new LinqRefEnumerable<T, ArrayLinqRefEnumerator<T>>(new ArrayLinqRefEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ArrayLinqRefEnumerator<T>> LinqValue<T>(this T[] src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ArrayLinqRefEnumerator<T>>(new ArrayLinqRefEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IListLinqValueEnumerator<T>> LinqValue<T>(this IList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IListLinqValueEnumerator<T>>(new IListLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IReadOnlyListLinqValueEnumerator<T>> LinqValue<T>(this IReadOnlyList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IReadOnlyListLinqValueEnumerator<T>>(
            new IReadOnlyListLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, ICollectionLinqValueEnumerator<T>> LinqValue<T>(this ICollection<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, ICollectionLinqValueEnumerator<T>>(
            new ICollectionLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IReadOnlyCollectionLinqValueEnumerator<T>> LinqValue<T>(
        this IReadOnlyCollection<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IReadOnlyCollectionLinqValueEnumerator<T>>(
            new IReadOnlyCollectionLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, IEnumerableLinqValueEnumerator<T>> LinqValue<T>(this IEnumerable<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, IEnumerableLinqValueEnumerator<T>>(
            new IEnumerableLinqValueEnumerator<T>(src));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>> LinqValue<T>(this ValueArray<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<T, UnmanagedArrayEnumerator<T>>(src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator> LinqValue<T>(
        this ValueLinkedList<T> src)
        where T : unmanaged
    {
        return new LinqValueEnumerable<ValueLinkedList<T>.Node, ValueLinkedList<T>.ForwardEnumerator>(
            src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<TKey, ValueDictionary<TKey, TValue>.KeyCollection.Enumerator>
        LinqRef<TKey, TValue>(this ValueDictionary<TKey, TValue>.KeyCollection src)
        where TValue : unmanaged where TKey : unmanaged
    {
        return new LinqFixedRefEnumerable<TKey, ValueDictionary<TKey, TValue>.KeyCollection.Enumerator>(
            src.GetEnumerator());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<TValue, ValueDictionary<TKey, TValue>.ValueCollection.Enumerator>
        LinqRef<TKey, TValue>(this ValueDictionary<TKey, TValue>.ValueCollection src)
        where TValue : unmanaged where TKey : unmanaged
    {
        return new LinqFixedRefEnumerable<TValue, ValueDictionary<TKey, TValue>.ValueCollection.Enumerator>(
            src.GetEnumerator());
    }

    #endregion

    #region Count

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        // ReSharper disable once NotDisposedResource
        var enumerator = src.GetEnumerator();
        if (enumerator.Count.HasValue)
            return enumerator.Count.Value;

        var c = 0;
        while (enumerator.MoveNext()) c++;

        enumerator.Dispose();
        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        // ReSharper disable once NotDisposedResource
        var enumerator = src.GetEnumerator();
        if (enumerator.Count.HasValue)
            return enumerator.Count.Value;

        var c = 0;
        while (enumerator.MoveNext()) c++;

        enumerator.Dispose();
        return c;
    }

    #endregion


    #region Sum

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum Sum<T, TEnumerator, TNum>(this LinqRefEnumerable<T, TEnumerator> src, FuncT1In<T, TNum> selector)
        where TNum : INumber<TNum>
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var sum = TNum.Zero;
        while (enumerator.MoveNext()) sum += selector(in enumerator.Current);

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum Sum<T, TEnumerator, TNum>(this LinqValueEnumerable<T, TEnumerator> src, Func<T, TNum> selector)
        where TNum : INumber<TNum>
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var sum = TNum.Zero;
        while (enumerator.MoveNext()) sum += selector(enumerator.Current);

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src) where T : INumber<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var sum = T.Zero;
        while (enumerator.MoveNext()) sum += enumerator.Current;

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src) where T : INumber<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var enumerator = src.GetEnumerator();
        var sum = T.Zero;
        while (enumerator.MoveNext()) sum += enumerator.Current;

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src) where T : INumber<T>
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var sum = T.Zero;
        while (enumerator.MoveNext()) sum += enumerator.Current;

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TEnumerator>(this LinqPtrEnumerable<T, TEnumerator> src)
        where T : INumber<T>, IMinMaxValue<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var enumerator = src.GetEnumerator();
        var min = T.MaxValue;
        while (enumerator.MoveNext())
            unsafe
            {
                min = T.Min(min, *enumerator.CurrentPtr);
            }

        return min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where T : INumber<T>, IMinMaxValue<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var min = T.MaxValue;
        while (enumerator.MoveNext()) min = T.Min(min, enumerator.Current);

        return min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where T : INumber<T>, IMinMaxValue<T>
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var min = T.MaxValue;
        while (enumerator.MoveNext()) min = T.Min(min, enumerator.Current);

        return min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TEnumerator>(this LinqPtrEnumerable<T, TEnumerator> src)
        where T : INumber<T>, IMinMaxValue<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var enumerator = src.GetEnumerator();
        var max = T.MinValue;
        while (enumerator.MoveNext())
            unsafe
            {
                max = T.Max(max, *enumerator.CurrentPtr);
            }

        return max;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where T : INumber<T>, IMinMaxValue<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var max = T.MinValue;
        while (enumerator.MoveNext()) max = T.Max(max, enumerator.Current);

        return max;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where T : INumber<T>, IMinMaxValue<T>
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var max = T.MinValue;
        while (enumerator.MoveNext()) max = T.Max(max, enumerator.Current);

        return max;
    }


    internal static class DummyLoad<T>
    {
        public static T? Dummy;
    }

    public static void Drain<T>(this IEnumerable<T> src)
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqPtrEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext())
            unsafe
            {
                DummyLoad<T>.Dummy = *e.CurrentPtr;
            }
    }

    #endregion
}