using System.Runtime.CompilerServices;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, TakeValue<T, TPrevious>> Take<T, TPrevious>(
        this LinqValueEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqValueEnumerator<T>
    {
        return new LinqValueEnumerable<T, TakeValue<T, TPrevious>>(
            new TakeValue<T, TPrevious>(src.GetEnumerator(), count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, TakeRef<T, TPrevious>> Take<T, TPrevious>(
        this LinqRefEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqRefEnumerable<T, TakeRef<T, TPrevious>>(
            new TakeRef<T, TPrevious>(src.GetEnumerator(), count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, TakeFixedRef<T, TPrevious>> Take<T, TPrevious>(
        this LinqFixedRefEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqFixedRefEnumerable<T, TakeFixedRef<T, TPrevious>>(
            new TakeFixedRef<T, TPrevious>(src.GetEnumerator(), count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, TakePtr<T, TPrevious>> Take<T, TPrevious>(
        this LinqPtrEnumerable<T, TPrevious> src, int count)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where T : unmanaged
    {
        return new LinqPtrEnumerable<T, TakePtr<T, TPrevious>>(
            new TakePtr<T, TPrevious>(src.GetEnumerator(), count));
    }
}