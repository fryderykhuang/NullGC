using System.Runtime.CompilerServices;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, SkipValue<T, TPrevious>> Skip<T, TPrevious>(
        this LinqValueEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqValueEnumerator<T>
    {
        return new LinqValueEnumerable<T, SkipValue<T, TPrevious>>(
            new SkipValue<T, TPrevious>(src.GetEnumerator(), count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, SkipRef<T, TPrevious>> Skip<T, TPrevious>(
        this LinqRefEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqRefEnumerable<T, SkipRef<T, TPrevious>>(
            new SkipRef<T, TPrevious>(src.GetEnumerator(), count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, SkipFixedRef<T, TPrevious>> Skip<T, TPrevious>(
        this LinqFixedRefEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqFixedRefEnumerable<T, SkipFixedRef<T, TPrevious>>(
            new SkipFixedRef<T, TPrevious>(src.GetEnumerator(), count));
    }
}