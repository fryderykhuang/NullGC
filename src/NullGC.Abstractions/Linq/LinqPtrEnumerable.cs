using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace NullGC.Linq;

public readonly struct LinqPtrEnumerable<T, TEnumerator> : ILinqEnumerable<T, TEnumerator>
    where TEnumerator : struct, ILinqRefEnumerator<T>
{
    public readonly TEnumerator Enumerator;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinqPtrEnumerable(TEnumerator enumerator)
    {
        Enumerator = enumerator;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TEnumerator GetEnumerator()
    {
        return Enumerator;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public IEnumerable<T> ToEnumerable()
    // {
    //     return new EnumerableWrapper<T, TEnumerator>(Enumerator);
    // }
    //
    public int? Count => Enumerator.Count;

    public int? MaxCount => Enumerator.Count;
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}