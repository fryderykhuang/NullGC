using System.Collections;
using System.Runtime.CompilerServices;
using NullGC.Allocators;
using NullGC.Collections;

namespace NullGC.Linq.Enumerators;

public static class OrderBy
{
    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> Ascending<T, TKey, TNext>(
        Func<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false, next);
    }

    public static
        ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>> Ascending<T, TKey, TNext>(
            Func<T, TKey> keySelector, ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false,
            next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> Descending<T, TKey, TNext>(
        Func<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true,
            next);
    }

    public static
        ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>> Descending<T, TKey, TNext>(
            Func<T, TKey> keySelector, ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, TNext>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true,
            next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> Ascending<T, TKey, TNext>(
        FuncT1In<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false, next);
    }

    public static
        ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>> Ascending<T, TKey, TNext>(
            FuncT1In<T, TKey> keySelector,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false, next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext> Descending<T, TKey, TNext>(
        FuncT1In<T, TKey> keySelector, TNext next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T>
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, TNext>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true, next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey2, FuncT1InInvoker<T, TKey2>, Comparer<TKey2>, TNext>>
        Descending<T, TKey, TKey2, TNext>(
            FuncT1In<T, TKey> keySelector,
            ValueEnumerableSorter<T, TKey2, FuncT1InInvoker<T, TKey2>, Comparer<TKey2>, TNext> next)
        where T : unmanaged where TKey : unmanaged where TNext : struct, IValueEnumerableSorter<T> where TKey2 : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>,
            ValueEnumerableSorter<T, TKey2, FuncT1InInvoker<T, TKey2>, Comparer<TKey2>, TNext>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true, next);
    }

    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Ascending<T,
        TKey>(
        Func<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false);
    }

    public static ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Descending<T,
        TKey>(
        Func<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Ascending<T,
        TKey>(
        FuncT1In<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, false);
    }

    public static ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>> Descending<T,
        TKey>(
        FuncT1In<T, TKey> keySelector)
        where T : unmanaged where TKey : unmanaged
    {
        return new ValueEnumerableSorter<T, TKey, FuncT1InInvoker<T, TKey>, Comparer<TKey>, DummyEnumerableSorter<T>>(
            new FuncT1InInvoker<T, TKey>(keySelector), Comparer<TKey>.Default, true);
    }
}


// public struct OrderBy<T, TPrevious, TKeySel, TKey, TComparer> : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>,
//     ILinqPtrEnumerator<T>,
//     ILinqEnumerable<T, OrderBy<T, TPrevious, TKeySel, TKey, TComparer>>,
//     IUnmanagedArray<int>, IUnmanagedArray<Wrapper<T>>
//     where TPrevious : struct, ILinqEnumerator<T>
//     where T : unmanaged
//     where TKey : unmanaged
//     where TComparer : IComparer<TKey>
// {
//     private TPrevious _previous;
//     private ValueArray<int> _indexBuffer;
//     private ValueList<T> _valueBuffer;
//     private EnumerableSorter<T, T, TKey, TKeySel, TComparer> _sorter;
//     private int _index;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public OrderBy(TPrevious prev, TKeySel keySelector, TComparer comparer, bool isDescending = false)
//     {
//         _previous = prev;
//         if (typeof(TPrevious).IsAssignableTo(typeof(ILinqPtrEnumerator<T>)) ||
//             typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)) &&
//             typeof(TPrevious).IsAssignableTo(typeof(IObjectAddressFixed)))
//         {
//             ThrowHelper.PreviousEnumeratorTypeNotSupported(nameof(prev));
//         }
//         else
//         {
//             _sorter = new EnumerableSorter<T, T, TKey, TKeySel, TComparer>(keySelector, comparer, isDescending);
//         }
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         if (!_indexBuffer.IsAllocated) BuildBuffer();
//
//         if (_index++ < _indexBuffer.Length)
//             return true;
//
//         return false;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private void BuildBuffer()
//     {
//         _valueBuffer = new ValueList<T>(_previous.MaxCount ?? 0);
//
//         while (_previous.MoveNext())
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//             {
//                 _valueBuffer.Add(((ILinqRefEnumerator<T>) _previous).Current);
//             }
//             else if (typeof(TPrevious).IsAssignableTo(typeof(ILinqValueEnumerator<T>)))
//             {
//                 _valueBuffer.Add(((ILinqValueEnumerator<T>) _previous).Current);
//             }
//             else
//                 throw new NotSupportedException();
//         }
//
//         _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
//     }
//
//     public ref T Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => ref _valueBuffer[_indexBuffer[_index - 1]];
//     }
//
//     T ILinqValueEnumerator<T>.Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => _valueBuffer[_indexBuffer[_index - 1]];
//     }
//
//     unsafe T* ILinqPtrEnumerator<T>.Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (T*) Unsafe.AsPointer(ref _valueBuffer[_indexBuffer[_index - 1]]);
//     }
//
//     public readonly int? Count => _previous.Count;
//     public readonly int? MaxCount => _previous.Count;
//
//     public readonly OrderBy<T, TPrevious, TKeySel, TKey, TComparer> GetEnumerator() => this;
//
//     public void Dispose()
//     {
//         _sorter.Dispose();
//         _indexBuffer.Dispose();
//         _valueBuffer.Dispose();
//         _previous.Dispose();
//     }
//
//     unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;
//
//     unsafe Wrapper<T>* IUnmanagedArray<Wrapper<T>>.Items
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Wrapper<T>*) ((IUnmanagedArray<T>) _valueBuffer).Items;
//     }
//
//     public int Length => _indexBuffer.Length;
//     public bool IsAllocated => _indexBuffer.IsAllocated;
// }

public struct OrderByRefToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext> : ILinqRefEnumerator<T>,
    ILinqEnumerable<T, OrderByRefToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext>>,
    IUnmanagedArray<int>, IUnmanagedArray<Wrapper<T>>, IAddressFixed
    where TPrevious : struct, ILinqRefEnumerator<T>
    where T : unmanaged
    where TKey : unmanaged
    where TComparer : IComparer<TKey>
    where TNext : struct, IValueEnumerableSorter<T>
    where TKeySel : struct
{
    private TPrevious _previous;
    private ValueArray<int> _indexBuffer;
    private ValueList<T> _valueBuffer;
    private ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> _sorter;
    private int _minIndex = int.MinValue;
    private int _maxIndex = int.MinValue;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByRefToFixedRef(TPrevious prev, TKeySel? keySelector, TComparer comparer, bool isDescending = false,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;
        if (typeof(TNext) != typeof(DummyEnumerableSorter<T>))
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(nameof(TNext));
            return;
        }

        _sorter = new ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext>(keySelector, comparer,
            isDescending, allocatorProviderId);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByRefToFixedRef(TPrevious prev, ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> sorter,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;
        _sorter = sorter;
        _sorter.SetAllocatorProviderId(allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (!_indexBuffer.IsAllocated) BuildBuffer();

        return _index++ <= _maxIndex;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    
    public unsafe T* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => &_valueBuffer.Items[_indexBuffer.Items[_index - 1]];
    }

    object IEnumerator.Current => Current;

    private void BuildBuffer()
    {
        _valueBuffer = new ValueList<T>(_previous.MaxCount ?? 0);

        while (_previous.MoveNext())
        {
            _valueBuffer.Add(_previous.Current);
        }

        if (_minIndex == int.MinValue)
        {
            _maxIndex = _valueBuffer.Count - 1;
            if (_maxIndex < 0) return;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
        }
        else
        {
            _maxIndex = Math.Min(_maxIndex, _valueBuffer.Count - 1);
            if (_maxIndex < 0) return;
            _index = _minIndex;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count, _minIndex, _maxIndex);
        }
    }

    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return ref _valueBuffer.Items[_indexBuffer.Items[_index - 1]];
            }
        }
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.Count;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly OrderByRefToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext> GetEnumerator() => this;

    public void Dispose()
    {
        _sorter.Dispose();
        _indexBuffer.Dispose();
        _valueBuffer.Dispose();
        _previous.Dispose();
    }

    unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;

    unsafe Wrapper<T>* IUnmanagedArray<Wrapper<T>>.Items
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (Wrapper<T>*) ((IUnmanagedArray<T>) _valueBuffer).Items;
    }

    public int Length => _indexBuffer.Length;
    public bool IsAllocated => _indexBuffer.IsAllocated;
    
    public bool SetSkipCount(int count)
    {
        if (_maxIndex == int.MinValue) _maxIndex = int.MaxValue;
        _minIndex = count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_minIndex == int.MinValue) _minIndex = 0;
        _maxIndex = _minIndex + count - 1;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct OrderByValueToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext> : ILinqRefEnumerator<T>, ISkipTakeAware,
    IAddressFixed,
    ILinqEnumerable<T, OrderByValueToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext>>,
    IUnmanagedArray<int>, IUnmanagedArray<Wrapper<T>>
    where TPrevious : struct, ILinqValueEnumerator<T>
    where T : unmanaged
    where TKey : unmanaged
    where TComparer : IComparer<TKey>
    where TNext : struct, IValueEnumerableSorter<T>
    where TKeySel : struct
{
    private TPrevious _previous;
    private ValueArray<int> _indexBuffer;
    private ValueList<T> _valueBuffer;
    private ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> _sorter;
    private int _minIndex = int.MinValue;
    private int _maxIndex = int.MinValue;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByValueToFixedRef(TPrevious prev, TKeySel? keySelector, TComparer comparer, bool isDescending = false,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;

        if (typeof(TNext) != typeof(DummyEnumerableSorter<T>))
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(nameof(TNext));
            return;
        }

        _sorter = new ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext>(keySelector, comparer,
            isDescending, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByValueToFixedRef(TPrevious prev, ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> sorter,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;
        _sorter = sorter;
        _sorter.SetAllocatorProviderId(allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (!_indexBuffer.IsAllocated) BuildBuffer();

        return _index++ <= _maxIndex;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;

    public unsafe T* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => &_valueBuffer.Items[_indexBuffer.Items[_index - 1]];
    } 

    object IEnumerator.Current => Current;

    private void BuildBuffer()
    {
        _valueBuffer = new ValueList<T>(_previous.MaxCount ?? 0);

        while (_previous.MoveNext())
        {
            _valueBuffer.Add(_previous.Current);
        }

        if (_minIndex == int.MinValue)
        {
            _maxIndex = _valueBuffer.Count - 1;
            if (_maxIndex < 0) return;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
        }
        else
        {
            _maxIndex = Math.Min(_maxIndex, _valueBuffer.Count - 1);
            if (_maxIndex < 0) return;
            _index = _minIndex;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count, _minIndex, _maxIndex);
        }
    }

    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _valueBuffer.GetRefUnchecked(_indexBuffer.GetUnchecked(_index - 1));
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.Count;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly OrderByValueToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext> GetEnumerator() => this;

    public void Dispose()
    {
        _sorter.Dispose();
        _indexBuffer.Dispose();
        _valueBuffer.Dispose();
        _previous.Dispose();
    }

    unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;

    unsafe Wrapper<T>* IUnmanagedArray<Wrapper<T>>.Items
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (Wrapper<T>*) ((IUnmanagedArray<T>) _valueBuffer).Items;
    }

    public int Length => _indexBuffer.Length;
    public bool IsAllocated => _indexBuffer.IsAllocated;

    public bool SetSkipCount(int count)
    {
        if (_maxIndex == int.MinValue) _maxIndex = int.MaxValue;
        _minIndex = count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_minIndex == int.MinValue) _minIndex = 0;
        _maxIndex = _minIndex + count - 1;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// public struct OrderByValueToValue<T, TPrevious, TKeySel, TKey, TComparer> : ILinqValueEnumerator<T>,
//     ILinqEnumerable<T, OrderByValueToValue<T, TPrevious, TKeySel, TKey, TComparer>>,
//     IUnmanagedArray<int>, IUnmanagedArray<Wrapper<T>>
//     where TPrevious : struct, ILinqValueEnumerator<T>
//     where T : unmanaged
//     where TKey : unmanaged
//     where TComparer : IComparer<TKey>
// {
//     private TPrevious _previous;
//     private ValueArray<int> _indexBuffer;
//     private ValueList<T> _valueBuffer;
//     private EnumerableSorter<T, T, TKey, TKeySel, TComparer> _sorter;
//     private int _index;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public OrderByValueToValue(TPrevious prev, TKeySel keySelector, TComparer comparer, bool isDescending = false)
//     {
//         _previous = prev;
//         _sorter = new EnumerableSorter<T, T, TKey, TKeySel, TComparer>(keySelector, comparer, isDescending);
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         if (!_indexBuffer.IsAllocated) BuildBuffer();
//
//         if (_index++ < _indexBuffer.Length)
//             return true;
//
//         return false;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private void BuildBuffer()
//     {
//         _valueBuffer = new ValueList<T>(_previous.MaxCount ?? 0);
//
//         while (_previous.MoveNext())
//         {
//             _valueBuffer.Add(_previous.Current);
//         }
//
//         _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
//     }
//
//     T ILinqValueEnumerator<T>.Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => _valueBuffer[_indexBuffer[_index - 1]];
//     }
//
//     public readonly int? Count => _previous.Count;
//     public readonly int? MaxCount => _previous.Count;
//
//     public readonly OrderByValueToValue<T, TPrevious, TKeySel, TKey, TComparer> GetEnumerator() => this;
//
//     public void Dispose()
//     {
//         _sorter.Dispose();
//         _indexBuffer.Dispose();
//         _valueBuffer.Dispose();
//         _previous.Dispose();
//     }
//
//     unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;
//
//     unsafe Wrapper<T>* IUnmanagedArray<Wrapper<T>>.Items
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Wrapper<T>*) ((IUnmanagedArray<T>) _valueBuffer).Items;
//     }
//
//     public int Length => _indexBuffer.Length;
//     public bool IsAllocated => _indexBuffer.IsAllocated;
// }

// public struct OrderByPtr<T, TPrevious, TKeySel, TKey, TComparer> : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>,
//     ILinqPtrEnumerator<T>,
//     ILinqEnumerable<T, OrderByPtr<T, TPrevious, TKeySel, TKey, TComparer>>,
//     IUnmanagedArray<int>, IUnmanagedArray<Ptr<T>>
//     where TPrevious : struct, ILinqEnumerator<T>
//     where T : unmanaged
//     where TKey : unmanaged
//     where TComparer : IComparer<TKey>
// {
//     private TPrevious _previous;
//     private ValueArray<int> _indexBuffer;
//     private ValueList<Ptr<T>> _valueBuffer;
//     private EnumerableSorter<Ptr<T>, T, TKey, TKeySel, TComparer> _sorter;
//
//     private int _index;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public OrderByPtr(TPrevious prev, TKeySel keySelector, TComparer comparer, bool isDescending = false)
//     {
//         _previous = prev;
//         if (typeof(TPrevious).IsAssignableTo(typeof(ILinqPtrEnumerator<T>)) ||
//             typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)) &&
//             typeof(TPrevious).IsAssignableTo(typeof(IObjectAddressFixed)))
//         {
//             _sorter = new EnumerableSorter<Ptr<T>, T, TKey, TKeySel, TComparer>(keySelector, comparer, isDescending);
//         }
//         else
//         {
//             ThrowHelper.PreviousEnumeratorTypeNotSupported(nameof(prev));
//         }
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         if (!_indexBuffer.IsAllocated) BuildBuffer();
//
//         return _index++ < _indexBuffer.Length;
//     }
//
//     [MethodImpl(MethodImplOptions.NoInlining)]
//     private void BuildBuffer()
//     {
//         _valueBuffer = new ValueList<Ptr<T>>(_previous.MaxCount ?? 0);
//
//         while (_previous.MoveNext())
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//             {
//                 _valueBuffer.Add(new Ptr<T>(ref ((ILinqRefEnumerator<T>) _previous).Current));
//             }
//             else if (typeof(TPrevious).IsAssignableTo(typeof(ILinqPtrEnumerator<T>)))
//             {
//                 unsafe
//                 {
//                     _valueBuffer.Add(new Ptr<T>(((ILinqPtrEnumerator<T>) _previous).Current));
//                 }
//             }
//             else
//                 throw new NotSupportedException();
//         }
//
//         _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
//     }
//
//     public ref T Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => ref _valueBuffer[_indexBuffer[_index - 1]].Ref;
//     }
//
//     T ILinqValueEnumerator<T>.Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => _valueBuffer[_indexBuffer[_index - 1]].Ref;
//     }
//
//     unsafe T* ILinqPtrEnumerator<T>.Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => _valueBuffer[_indexBuffer[_index - 1]].Value;
//     }
//
//     public readonly int? Count => _previous.Count;
//     public readonly int? MaxCount => _previous.Count;
//
//     public readonly OrderByPtr<T, TPrevious, TKeySel, TKey, TComparer> GetEnumerator() => this;
//
//     public void Dispose()
//     {
//         _sorter.Dispose();
//         _indexBuffer.Dispose();
//         _valueBuffer.Dispose();
//         _previous.Dispose();
//     }
//
//     unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;
//
//     public unsafe Ptr<T>* Items
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => ((IUnmanagedArray<Ptr<T>>) _valueBuffer).Items;
//     }
//
//     public int Length => _indexBuffer.Length;
//     public bool IsAllocated => _indexBuffer.IsAllocated;
// }

public struct OrderByPtrToPtr<T, TPrevious, TKeySel, TKey, TComparer, TNext> : ILinqRefEnumerator<T>, IAddressFixed,
    ILinqEnumerable<T, OrderByPtrToPtr<T, TPrevious, TKeySel, TKey, TComparer, TNext>>,
    IUnmanagedArray<int>, IUnmanagedArray<Ptr<T>>
    where TPrevious : ILinqRefEnumerator<T>, IAddressFixed
    where T : unmanaged
    where TKey : unmanaged
    where TComparer : IComparer<TKey>
    where TNext : struct, IValueEnumerableSorter<T>
    where TKeySel : struct
{
    private TPrevious _previous;
    private ValueList<Ptr<T>> _valueBuffer;
    private ValueArray<int> _indexBuffer;
    private ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> _sorter;
    private int _minIndex = int.MinValue;
    private int _maxIndex = int.MinValue;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByPtrToPtr(TPrevious prev, TKeySel? keySelector, TComparer comparer, bool isDescending = false,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;
        if (typeof(TNext) != typeof(DummyEnumerableSorter<T>))
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(nameof(TNext));
            return;
        }

        _sorter = new ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext>(keySelector, comparer,
            isDescending, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByPtrToPtr(TPrevious prev, ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> sorter,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;
        _sorter = sorter;
        _sorter.SetAllocatorProviderId(allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (!_indexBuffer.IsAllocated) BuildBuffer();

        return _index++ <= _maxIndex;
    }

    public void Reset()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    ref T ILinqRefEnumerator<T>.Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _valueBuffer.GetRefUnchecked(_indexBuffer.GetRefUnchecked(_index - 1)).Ref;
    }
    
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

    private void BuildBuffer()
    {
        _valueBuffer = new ValueList<Ptr<T>>(_previous.MaxCount ?? 0);

        while (_previous.MoveNext())
        {
            unsafe
            {
                _valueBuffer.Add(new Ptr<T>(_previous.CurrentPtr));
            }
        }

        if (_minIndex == int.MinValue)
        {
            _maxIndex = _valueBuffer.Count - 1;
            if (_maxIndex < 0) return;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
        }
        else
        {
            _maxIndex = Math.Min(_maxIndex, _valueBuffer.Count - 1);
            if (_maxIndex < 0) return;
            _index = _minIndex;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count, _minIndex, _maxIndex);
        }
    }

    public unsafe T* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _valueBuffer.GetRefUnchecked(_indexBuffer.GetUnchecked(_index - 1)).Value;
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.Count;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly OrderByPtrToPtr<T, TPrevious, TKeySel, TKey, TComparer, TNext> GetEnumerator() => this;

    public void Dispose()
    {
        _sorter.Dispose();
        _indexBuffer.Dispose();
        _valueBuffer.Dispose();
        _previous.Dispose();
    }

    unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;

    public unsafe Ptr<T>* Items
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _valueBuffer.Items;
    }

    public int Length => _indexBuffer.Length;
    public bool IsAllocated => _indexBuffer.IsAllocated;
    
    public bool SetSkipCount(int count)
    {
        if (_maxIndex == int.MinValue) _maxIndex = int.MaxValue;
        _minIndex = count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_minIndex == int.MinValue) _minIndex = 0;
        _maxIndex = _minIndex + count - 1;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct OrderByFixedRefToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext> : ILinqRefEnumerator<T>,
    IAddressFixed,
    ILinqEnumerable<T, OrderByFixedRefToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext>>,
    IUnmanagedArray<int>, IUnmanagedArray<Ptr<T>>
    where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed
    where T : unmanaged
    where TKey : unmanaged
    where TComparer : IComparer<TKey>
    where TNext : struct, IValueEnumerableSorter<T>
    where TKeySel : struct
{
    private TPrevious _previous;
    private ValueArray<int> _indexBuffer;
    private ValueList<Ptr<T>> _valueBuffer;
    private ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> _sorter;
    private int _minIndex = int.MinValue;
    private int _maxIndex = int.MinValue;
    private int _index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByFixedRefToFixedRef(TPrevious prev, TKeySel? keySelector, TComparer comparer, bool isDescending = false,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;

        if (typeof(TNext) != typeof(DummyEnumerableSorter<T>))
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowArgumentException(nameof(TNext));
            return;
        }

        _sorter = new ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext>(keySelector, comparer,
            isDescending, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OrderByFixedRefToFixedRef(TPrevious prev, ValueEnumerableSorter<T, TKey, TKeySel, TComparer, TNext> sorter,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _previous = prev;
        _sorter = sorter;
        _sorter.SetAllocatorProviderId(allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (!_indexBuffer.IsAllocated) BuildBuffer();

        return _index++ <= _maxIndex;
    }

    public void Reset()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    T IEnumerator<T>.Current => Current;

    public unsafe T* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _valueBuffer.Items[_indexBuffer.Items[_index - 1]].Value;
    }

    object IEnumerator.Current => Current;

    private void BuildBuffer()
    {
        _valueBuffer = new ValueList<Ptr<T>>(_previous.MaxCount ?? 0);

        while (_previous.MoveNext())
        {
            _valueBuffer.Add(new Ptr<T>(ref _previous.Current));
        }

        if (_minIndex == int.MinValue)
        {
            _maxIndex = _valueBuffer.Count - 1;
            if (_maxIndex < 0) return;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count);
        }
        else
        {
            _maxIndex = Math.Min(_maxIndex, _valueBuffer.Count - 1);
            if (_maxIndex < 0) return;
            _index = _minIndex;
            _indexBuffer = _sorter.Sort(_valueBuffer.GetInnerArray().Borrow(), _valueBuffer.Count, _minIndex, _maxIndex);
        }
    }

    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _valueBuffer.GetRefUnchecked(_indexBuffer.GetUnchecked(_index - 1)).Ref;
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.Count;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public readonly OrderByFixedRefToFixedRef<T, TPrevious, TKeySel, TKey, TComparer, TNext> GetEnumerator() => this;

    public void Dispose()
    {
        _sorter.Dispose();
        _indexBuffer.Dispose();
        _valueBuffer.Dispose();
        _previous.Dispose();
    }

    unsafe int* IUnmanagedArray<int>.Items => ((IUnmanagedArray<int>) _indexBuffer).Items;

    public unsafe Ptr<T>* Items
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ((IUnmanagedArray<Ptr<T>>) _valueBuffer).Items;
    }

    public int Length => _indexBuffer.Length;
    public bool IsAllocated => _indexBuffer.IsAllocated;
    public bool SetSkipCount(int count)
    {
        if (_maxIndex == int.MinValue) _maxIndex = int.MaxValue;
        _minIndex = count;
        return true;
    }

    public bool SetTakeCount(int count)
    {
        if (_minIndex == int.MinValue) _minIndex = 0;
        _maxIndex = _minIndex + count - 1;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
