using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Linq;

namespace NullGC.Collections;

public struct UnmanagedArrayEnumerator<T> : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>, IUnmanagedArray<T>, IAddressFixed where T : unmanaged
{
    private readonly unsafe T* _items;
    private readonly int _length;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe UnmanagedArrayEnumerator(T* items, int length)
    {
        _items = items;
        _length = length;
        _index = -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_index + 1 >= _length) return false;
        ++_index;
        return true;
    }

    public void Reset() => _index = -1;

    T IEnumerator<T>.Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return _items[_index];
            }
        }
    }

    public unsafe T* CurrentPtr => &_items[_index];

    object IEnumerator.Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return _items[_index];
            }
        }
    }

    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return ref _items[_index];
            }
        }
    }

    public int? Count => _length;

    public int? MaxCount => _length;

    public void Dispose()
    {
    }

    public unsafe T* Items => _items;
    public int Length => _length;

    public bool IsAllocated
    {
        get
        {
            unsafe
            {
                return _items == (T*) 0;
            }
        }
    }

    public bool SetSkipCount(int count)
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        _index = count - 1;
        return true;
    }

    public bool SetTakeCount(int count) => false;
}