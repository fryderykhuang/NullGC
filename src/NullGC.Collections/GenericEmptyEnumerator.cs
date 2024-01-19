using System.Collections;

namespace NullGC.Collections;

internal sealed class GenericEmptyEnumerator<T> : IEnumerator<T>
{
    public static readonly GenericEmptyEnumerator<T> Instance = new();

    private GenericEmptyEnumerator()
    {
    }

    public bool MoveNext()
    {
        return false;
    }

    public void Reset()
    {
    }

    object? IEnumerator.Current => Current;

    public void Dispose()
    {
    }

    public T Current
    {
        get
        {
            ThrowHelper.ThrowNoMoreElements();
            return default!;
        }
    }
}