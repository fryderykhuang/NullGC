using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace NullGC.Linq.Enumerators;

public struct TakeRef<T, TPrevious> : ILinqRefEnumerator<T>, ILinqEnumerable<T, TakeRef<T, TPrevious>>
    where TPrevious : struct, ILinqRefEnumerator<T>
{
    private TPrevious _previous;
    private int _takeCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TakeRef(TPrevious prev, int count)
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        _previous = prev;
        _takeCount = count;
        if (_previous.SetTakeCount(count)) _takeCount = int.MinValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_takeCount == int.MinValue) return _previous.MoveNext();
        if (_takeCount <= 0 || !_previous.MoveNext()) return false;
        _takeCount--;
        return true;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _previous.Current;
    }

    public readonly int? Count => !_previous.Count.HasValue
        ? null
        : _takeCount == int.MinValue
            ? _previous.Count.Value
            : Math.Min(_previous.Count.Value, _takeCount);
    public readonly int? MaxCount => !_previous.MaxCount.HasValue
        ? _takeCount == int.MinValue ? null : _takeCount
        : _takeCount == int.MinValue ? _previous.MaxCount.Value : Math.Min(_previous.MaxCount.Value, _takeCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly TakeRef<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count)
    {
        if (!_previous.SetSkipCount(count)) return false;
        if (_takeCount != int.MinValue) _takeCount = Math.Max(0, _takeCount - count);
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_takeCount == int.MinValue) return _previous.SetTakeCount(count);
        _takeCount = Math.Min(_takeCount, count);
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct TakeFixedRef<T, TPrevious> : ILinqRefEnumerator<T>, IAddressFixed,
    ILinqEnumerable<T, TakeFixedRef<T, TPrevious>>
    where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed
{
    private TPrevious _previous;
    private int _takeCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TakeFixedRef(TPrevious prev, int count)
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        _previous = prev;
        _takeCount = count;
        if (_previous.SetTakeCount(count)) _takeCount = int.MinValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_takeCount == int.MinValue) return _previous.MoveNext();
        if (_takeCount <= 0 || !_previous.MoveNext()) return false;
        _takeCount--;
        return true;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _previous.Current;
    }

    public readonly int? Count => !_previous.Count.HasValue
        ? null
        : _takeCount == int.MinValue
            ? _previous.Count.Value
            : Math.Min(_previous.Count.Value, _takeCount);
    public readonly int? MaxCount => !_previous.MaxCount.HasValue
        ? _takeCount == int.MinValue ? null : _takeCount
        : _takeCount == int.MinValue ? _previous.MaxCount.Value : Math.Min(_previous.MaxCount.Value, _takeCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly TakeFixedRef<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count)
    {
        if (!_previous.SetSkipCount(count)) return false;
        if (_takeCount != int.MinValue) _takeCount = Math.Max(0, _takeCount - count);
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_takeCount == int.MinValue) return _previous.SetTakeCount(count);
        _takeCount = Math.Min(_takeCount, count);
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct TakeValue<T, TPrevious> : ILinqValueEnumerator<T>,
    ILinqEnumerable<T, TakeValue<T, TPrevious>>
    where TPrevious : struct, ILinqValueEnumerator<T>
{
    private TPrevious _previous;
    private int _takeCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TakeValue(TPrevious prev, int count)
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        _previous = prev;
        _takeCount = count;
        if (_previous.SetTakeCount(count))
        {
            _takeCount = int.MinValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_takeCount == int.MinValue) return _previous.MoveNext();
        if (_takeCount <= 0 || !_previous.MoveNext()) return false;
        _takeCount--;
        return true;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public readonly T Current => _previous.Current;

    public readonly int? Count => !_previous.Count.HasValue
        ? null
        : _takeCount == int.MinValue
            ? _previous.Count.Value
            : Math.Min(_previous.Count.Value, _takeCount);
    public readonly int? MaxCount => !_previous.MaxCount.HasValue
        ? _takeCount == int.MinValue ? null : _takeCount
        : _takeCount == int.MinValue ? _previous.MaxCount.Value : Math.Min(_previous.MaxCount.Value, _takeCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly TakeValue<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count)
    {
        if (!_previous.SetSkipCount(count)) return false;
        if (_takeCount != int.MinValue) _takeCount = Math.Max(0, _takeCount - count);
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_takeCount == int.MinValue) return _previous.SetTakeCount(count);
        _takeCount = Math.Min(_takeCount, count);
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct TakePtr<T, TPrevious> : ILinqRefEnumerator<T>, ILinqEnumerable<T, TakePtr<T, TPrevious>>, IAddressFixed
    where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed
    where T : unmanaged
{
    private TPrevious _previous;
    private int _takeCount;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TakePtr(TPrevious prev, int count)
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        _previous = prev;
        _takeCount = count;
        if (_previous.SetTakeCount(count))
        {
            _takeCount = int.MinValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_takeCount == int.MinValue) return _previous.MoveNext();
        if (_takeCount <= 0 || !_previous.MoveNext()) return false;
        _takeCount--;
        return true;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    ref T ILinqRefEnumerator<T>.Current => ref _previous.Current;
    
    T IEnumerator<T>.Current
    {
        get
        {
            unsafe
            {
                return *CurrentPtr;
            }
        }
    }

    object IEnumerator.Current
    {
        get
        {
            unsafe
            {
                return *CurrentPtr;
            }
        }
    }

    public unsafe T* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _previous.CurrentPtr;
    }

    public readonly int? Count => !_previous.Count.HasValue
        ? null
        : _takeCount == int.MinValue
            ? _previous.Count.Value
            : Math.Min(_previous.Count.Value, _takeCount);
    public readonly int? MaxCount => !_previous.MaxCount.HasValue
        ? _takeCount == int.MinValue ? null : _takeCount
        : _takeCount == int.MinValue ? _previous.MaxCount.Value : Math.Min(_previous.MaxCount.Value, _takeCount);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly TakePtr<T, TPrevious> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();
    
    public bool SetSkipCount(int count)
    {
        if (!_previous.SetSkipCount(count)) return false;
        if (_takeCount != int.MinValue) _takeCount = Math.Max(0, _takeCount - count);
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_takeCount == int.MinValue) return _previous.SetTakeCount(count);
        _takeCount = Math.Min(_takeCount, count);
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}