using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace NullGC.Linq;

internal struct LinqEnumerator<T, TEnumerator> : IEnumerator<T>, IEnumerable<T> where TEnumerator : struct, ILinqEnumerator<T>
{
    private TEnumerator _enumerator;

    public LinqEnumerator(TEnumerator enumerator)
    {
        _enumerator = enumerator;
    }

    public bool MoveNext()
    {
        return _enumerator.MoveNext();
    }

    public void Reset()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    public void Dispose()
    {
        _enumerator.Dispose();
    }

    public T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (typeof(TEnumerator).IsAssignableTo(typeof(ILinqValueEnumerator<T>)))
            {
                return ((ILinqValueEnumerator<T>)_enumerator).Current;
            }
            else if (typeof(TEnumerator).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
            {
                return ((ILinqRefEnumerator<T>)_enumerator).Current;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

    object IEnumerator.Current => Current!;

    public IEnumerator<T> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => this;
}