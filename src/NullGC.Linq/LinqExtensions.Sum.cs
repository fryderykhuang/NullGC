using System.Numerics;
using System.Runtime.CompilerServices;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Sum

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum Sum<T, TEnumerator, TNum>(this LinqFixedRefEnumerable<T, TEnumerator> src,
        FuncT1In<T, TNum> selector)
        where TNum : INumber<TNum>
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var enumerator = src.GetEnumerator();
        var sum = TNum.Zero;
        while (enumerator.MoveNext()) sum += selector(in enumerator.Current);

        return sum;
    }

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
    public static T Sum<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src) where T : INumber<T>
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var enumerator = src.GetEnumerator();
        var sum = T.Zero;
        while (enumerator.MoveNext()) sum += enumerator.Current;

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
    public static T Sum<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src) where T : INumber<T>
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        var sum = T.Zero;
        while (enumerator.MoveNext()) sum += enumerator.Current;

        return sum;
    }

    #endregion
}