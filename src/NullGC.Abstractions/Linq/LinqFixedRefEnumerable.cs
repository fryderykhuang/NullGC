using System.Collections;
using System.Runtime.CompilerServices;

namespace NullGC.Linq;

public readonly struct LinqFixedRefEnumerable<T, TEnumerator> : ILinqEnumerable<T, TEnumerator>
    where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
{
    private readonly TEnumerator _enumerator;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinqFixedRefEnumerable(TEnumerator enumerator)
    {
        _enumerator = enumerator;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _enumerator;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TEnumerator GetEnumerator()
    {
        return _enumerator;
    }

    // public IEnumerable<T> ToEnumerable()
    // {
    //     return new EnumerableWrapper<T, TEnumerator>(_enumerator);
    // }

    public int? Count => _enumerator.Count;

    public int? MaxCount => _enumerator.Count;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}