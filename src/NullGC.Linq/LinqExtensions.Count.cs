using System.Numerics;
using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Count

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var enumerator = src.GetEnumerator();
        if (enumerator.Count.HasValue)
            return enumerator.Count.Value;

        var c = 0;
        while (enumerator.MoveNext()) c++;
        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        if (enumerator.Count.HasValue)
            return enumerator.Count.Value;

        var c = 0;
        while (enumerator.MoveNext()) c++;
        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var enumerator = src.GetEnumerator();
        if (enumerator.Count.HasValue)
            return enumerator.Count.Value;

        var c = 0;
        while (enumerator.MoveNext()) c++;
        return c;
    }

    #endregion
}