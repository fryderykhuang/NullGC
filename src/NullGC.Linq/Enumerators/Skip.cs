using System.Collections;
using System.Runtime.CompilerServices;

namespace NullGC.Linq.Enumerators;
public struct SkipRef<T, TPrevious> : ILinqRefEnumerator<T>,
    ILinqEnumerable<T, SkipRef<T, TPrevious>>
    where TPrevious : struct, ILinqRefEnumerator<T>
{
    private TPrevious _previous;
    private int _skipCount;

    public SkipRef(TPrevious prev, int count)
    {
        _previous = prev;
        _skipCount = count;
        if (_previous.SetSkipCount(count)) _skipCount = int.MinValue;
    }
    public bool MoveNext()
    {
        while (this._previous.MoveNext())
        {
            if (_skipCount <= 0) return true;
            _skipCount--;
        }

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current => ref _previous.Current;

    public readonly int? Count => _skipCount == int.MinValue
        ? _previous.Count
        : !_previous.Count.HasValue ? null : Math.Max(0, _previous.Count.Value - _skipCount);

    public readonly int? MaxCount => _skipCount == int.MinValue
        ? _previous.MaxCount
        : !_previous.MaxCount.HasValue ? null : Math.Max(0, _previous.MaxCount.Value - _skipCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly SkipRef<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count)
    {
        if (_skipCount == int.MinValue) return _previous.SetSkipCount(count);
        _skipCount += count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_skipCount == int.MinValue) return _previous.SetTakeCount(count);
        return _previous.SetTakeCount(_skipCount + count);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SkipFixedRef<T, TPrevious> : ILinqRefEnumerator<T>, IItemAddressFixed,
    ILinqEnumerable<T, SkipFixedRef<T, TPrevious>>
    where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
{
    private TPrevious _previous;
    private int _skipCount;

    public SkipFixedRef(TPrevious prev, int count)
    {
        _previous = prev;
        _skipCount = count;
        if (_previous.SetSkipCount(count)) _skipCount = int.MinValue;
    }
    public bool MoveNext()
    {
        while (this._previous.MoveNext())
        {
            if (_skipCount <= 0) return true;
            _skipCount--;
        }

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current => ref _previous.Current;

    public readonly int? Count => _skipCount == int.MinValue
        ? _previous.Count
        : !_previous.Count.HasValue ? null : Math.Max(0, _previous.Count.Value - _skipCount);

    public readonly int? MaxCount => _skipCount == int.MinValue
        ? _previous.MaxCount
        : !_previous.MaxCount.HasValue ? null : Math.Max(0, _previous.MaxCount.Value - _skipCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly SkipFixedRef<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count)
    {
        if (_skipCount == int.MinValue) return _previous.SetSkipCount(count);
        _skipCount += count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_skipCount == int.MinValue) return _previous.SetTakeCount(count);
        return _previous.SetTakeCount(_skipCount + count);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SkipValue<T, TPrevious> : ILinqValueEnumerator<T>,
    ILinqEnumerable<T, SkipValue<T, TPrevious>>
    where TPrevious : struct, ILinqValueEnumerator<T>
{
    private TPrevious _previous;
    private int _skipCount;

    public SkipValue(TPrevious prev, int count)
    {
        _previous = prev;
        _skipCount = count;
        if (_previous.SetSkipCount(count)) _skipCount = int.MinValue;
    }

    public bool MoveNext()
    {
        while (this._previous.MoveNext())
        {
            if (_skipCount <= 0) return true;
            _skipCount--;
        }

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public readonly T Current => _previous.Current;

    public readonly int? Count => _skipCount == int.MinValue
        ? _previous.Count
        : !_previous.Count.HasValue ? null : Math.Max(0, _previous.Count.Value - _skipCount);

    public readonly int? MaxCount => _skipCount == int.MinValue
        ? _previous.MaxCount
        : !_previous.MaxCount.HasValue ? null : Math.Max(0, _previous.MaxCount.Value - _skipCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly SkipValue<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count)
    {
        if (_skipCount == int.MinValue) return _previous.SetSkipCount(count);
        _skipCount += count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_skipCount == int.MinValue) return _previous.SetTakeCount(count);
        return _previous.SetTakeCount(_skipCount + count);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
