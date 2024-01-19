using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Collections;

namespace NullGC.Linq.Enumerators;

public struct ArrayLinqRefEnumerator<T> : ILinqRefEnumerator<T>, IArray<T>
{
    private readonly T[] _array;
    private readonly int _length;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayLinqRefEnumerator(T[] array, int length)
    {
        Guard.IsNotNull(array);
        Guard.IsLessThanOrEqualTo(length, _array!.Length);
        _array = array;
        _length = length;
        _index = -1;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayLinqRefEnumerator(T[] array)
    {
        Guard.IsNotNull(array);
        _array = array;
        _length = array.Length;
        _index = -1;
    }
    
    public void Dispose()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_index + 1 >= _length)
            return false;

        _index++;
        return true;
    }

    public void Reset()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    T IEnumerator<T>.Current => Current;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    public unsafe T* CurrentPtr
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    {
        get
        {
            CommunityToolkit.Diagnostics.ThrowHelper
                .ThrowNotSupportedException();
            return default;
            //(T*) Unsafe.AsPointer(ref _array[_index]);
        }
    }

    object? IEnumerator.Current => Current;

    public int? Count => _length;
    public int? MaxCount => _length;
    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _array[_index];
    }

    public T[] Items => _array;
    public int Length => _length;
    public bool IsInitialized => _array is not null;
    public bool SetSkipCount(int count)
    {
        _index = count - 1;
        return true;
    }

    public bool SetTakeCount(int count) => false;
}