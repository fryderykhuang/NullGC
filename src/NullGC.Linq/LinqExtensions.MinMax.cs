using System.Numerics;
using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region MinMax
    
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

    #endregion
}