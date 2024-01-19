using System.Collections;
using System.Runtime.CompilerServices;

namespace NullGC.Linq.Enumerators;

public struct IReadOnlyListLinqValueEnumerator<T> : ILinqValueEnumerator<T>
{
    private readonly IReadOnlyList<T> _list;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IReadOnlyListLinqValueEnumerator(IReadOnlyList<T> list)
    {
        _list = list;
        _index = -1;
    }
    
    public void Dispose()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_index + 1 >= _list.Count)
            return false;

        _index++;
        return true;
    }

    public void Reset()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    object? IEnumerator.Current => Current;

    public readonly int? Count => _list.Count;
    public readonly int? MaxCount => _list.Count;
    public readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>  _list[_index];
    }

    public bool SetSkipCount(int count)
    {
        _index = count - 1;
        return true;
    }

    public bool SetTakeCount(int count) => false;
}