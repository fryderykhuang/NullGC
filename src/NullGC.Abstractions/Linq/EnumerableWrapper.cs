using System.Collections;
using System.Runtime.CompilerServices;

namespace NullGC.Linq;

internal class EnumerableWrapper<T, TEnumerator> : IEnumerable<T> where TEnumerator : struct, ILinqEnumerator<T>
{
    private readonly TEnumerator _enumerator;

    public EnumerableWrapper(TEnumerator enumerator)
    {
        _enumerator = enumerator;
    }

    public LinqEnumerator<T, TEnumerator> GetEnumerator()
    {
        return new LinqEnumerator<T, TEnumerator>(_enumerator);
    }

    [Obsolete("This will box struct enumerator chain to make a IEnumerator, only use this when absolutely necessary.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    [Obsolete("This will box struct enumerator chain to make a IEnumerator, only use this when absolutely necessary.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}