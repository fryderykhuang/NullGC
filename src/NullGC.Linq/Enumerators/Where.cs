using System.Collections;
using System.Runtime.CompilerServices;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace NullGC.Linq.Enumerators;

// public struct Where<T, TPrevious, TDelegate> : ILinqRefEnumerator<T>, ILinqValueEnumerator<T>,
//     ILinqEnumerable<T, Where<T, TPrevious, TDelegate>>
//     where TPrevious : struct, ILinqEnumerator<T>
// {
//     private TPrevious _previous;
//     private readonly TDelegate _predicate = default!;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public Where(TPrevious prev, TDelegate predicate)
//     {
//         if (!(typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)) ||
//               typeof(TPrevious).IsAssignableTo(typeof(ILinqValueEnumerator<T>))))
//         {
//             ThrowHelper.PreviousEnumeratorTypeNotSupported(nameof(prev));
//             return;
//         }
//
//         if (!(typeof(TDelegate).IsAssignableTo(typeof(IFuncT1InInvoker<T, bool>)) ||
//               typeof(TDelegate).IsAssignableTo(typeof(IFuncInvoker<T, bool>))))
//         {
//             ThrowHelper.DelegateTypeNotSupported(nameof(predicate));
//             return;
//         }
//
//         _previous = prev;
//         _predicate = predicate;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         while (_previous.MoveNext())
//             if (typeof(TDelegate).IsAssignableTo(typeof(IFuncT1InInvoker<T, bool>)))
//             {
//                 if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//                 {
//                     if (((IFuncT1InInvoker<T, bool>) _predicate).Invoke(in ((ILinqRefEnumerator<T>) _previous).Current))
//                         return true;
//                 }
//                 else
//                 {
//                     if (((IFuncT1InInvoker<T, bool>) _predicate).Invoke(((ILinqValueEnumerator<T>) _previous).Current))
//                         return true;
//                 }
//             }
//             else if (typeof(TDelegate).IsAssignableTo(typeof(IFuncInvoker<T, bool>)))
//             {
//                 if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//                 {
//                     if (((IFuncInvoker<T, bool>) _predicate).Invoke(((ILinqRefEnumerator<T>) _previous).Current))
//                         return true;
//                 }
//                 else
//                 {
//                     if (((IFuncInvoker<T, bool>) _predicate).Invoke(((ILinqValueEnumerator<T>) _previous).Current))
//                         return true;
//                 }
//             }
//
//         return false;
//     }
//
//     public ref T Current
//     {
//         get
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqValueEnumerator<T>)))
//                 throw new NotSupportedException();
//             else
//                 return ref ((ILinqRefEnumerator<T>) _previous).Current;
//         }
//     }
//
//     public int? Count => null;
//     public int? MaxCount => _previous.MaxCount;
//
//     public Where<T, TPrevious, TDelegate> GetEnumerator()
//     {
//         return this;
//     }
//
//     public void Dispose()
//     {
//         _previous.Dispose();
//     }
//
//     T ILinqValueEnumerator<T>.Current
//     {
//         get
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//                 return ((ILinqRefEnumerator<T>) _previous).Current;
//             else
//                 return ((ILinqValueEnumerator<T>) _previous).Current;
//         }
//     }
// }

public struct WhereRefToRef<T, TPrevious, TPredicate> : ILinqRefEnumerator<T>,
    ILinqEnumerable<T, WhereRefToRef<T, TPrevious, TPredicate>>
    where TPrevious : struct, ILinqRefEnumerator<T>
    where TPredicate : struct, IFuncInvoker<T, bool>
{
    private TPrevious _previous;
    private TPredicate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WhereRefToRef(TPrevious prev, TPredicate predicate)
    {
        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (_predicate.Invoke(_previous.Current))
                return true;

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current => ref _previous.Current;

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WhereRefToRef<T, TPrevious, TPredicate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();
    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct WhereRefToRefIn<T, TPrevious, TPredicate> : ILinqRefEnumerator<T>,
    ILinqEnumerable<T, WhereRefToRefIn<T, TPrevious, TPredicate>>
    where TPrevious : struct, ILinqRefEnumerator<T>
    where TPredicate : struct, IFuncT1InInvoker<T, bool>
{
    private TPrevious _previous;
    private TPredicate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WhereRefToRefIn(TPrevious prev, TPredicate predicate)
    {
        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (_predicate.Invoke(in _previous.Current))
                return true;

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current => ref _previous.Current;

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WhereRefToRefIn<T, TPrevious, TPredicate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();
    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct WhereFixedRefToFixedRef<T, TPrevious, TPredicate> : ILinqRefEnumerator<T>, IItemAddressFixed,
    ILinqEnumerable<T, WhereFixedRefToFixedRef<T, TPrevious, TPredicate>>
    where TPrevious : ILinqRefEnumerator<T>, IItemAddressFixed
    where TPredicate : struct, IFuncInvoker<T, bool>
{
    private TPrevious _previous = default!;
    private TPredicate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WhereFixedRefToFixedRef(TPrevious prev, TPredicate predicate)
    {
        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (_predicate.Invoke(_previous.Current))
                return true;

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current => ref _previous.Current;

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WhereFixedRefToFixedRef<T, TPrevious, TPredicate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();
    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct WhereFixedRefToFixedRefIn<T, TPrevious, TPredicate> : ILinqRefEnumerator<T>, IItemAddressFixed,
    ILinqEnumerable<T, WhereFixedRefToFixedRefIn<T, TPrevious, TPredicate>>
    where TPrevious : ILinqRefEnumerator<T>, IItemAddressFixed
    where TPredicate : struct, IFuncT1InInvoker<T, bool>
{
    private TPrevious _previous = default!;
    private TPredicate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WhereFixedRefToFixedRefIn(TPrevious prev, TPredicate predicate)
    {
        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (_predicate.Invoke(in _previous.Current))
                return true;

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    T IEnumerator<T>.Current => Current;
    public unsafe T* CurrentPtr => _previous.CurrentPtr;

    object? IEnumerator.Current => Current;

    public ref T Current => ref _previous.Current;

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WhereFixedRefToFixedRefIn<T, TPrevious, TPredicate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();
    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct WhereValueToValue<T, TPrevious, TDelegate> : ILinqValueEnumerator<T>,
    ILinqEnumerable<T, WhereValueToValue<T, TPrevious, TDelegate>>
    where TPrevious : ILinqValueEnumerator<T>
    where TDelegate : struct, IFuncInvoker<T, bool>
{
    private TPrevious _previous = default!;
    private TDelegate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WhereValueToValue(TPrevious prev, TDelegate predicate)
    {
        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (_predicate.Invoke(_previous.Current))
                return true;

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WhereValueToValue<T, TPrevious, TDelegate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public T Current => _previous.Current;
    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct WhereValueToValueIn<T, TPrevious, TDelegate> : ILinqValueEnumerator<T>,
    ILinqEnumerable<T, WhereValueToValueIn<T, TPrevious, TDelegate>>
    where TPrevious : ILinqValueEnumerator<T>
    where TDelegate : struct, IFuncT1InInvoker<T, bool>
{
    private TPrevious _previous = default!;
    private TDelegate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WhereValueToValueIn(TPrevious prev, TDelegate predicate)
    {
        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (_predicate.Invoke(_previous.Current))
                return true;

        return false;
    }

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WhereValueToValueIn<T, TPrevious, TDelegate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public T Current => _previous.Current;
    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// public struct WherePtr<T, TPrevious, TDelegate> : ILinqRefEnumerator<T>, ILinqPtrEnumerator<T>,
//     ILinqEnumerable<T, WherePtr<T, TPrevious, TDelegate>>
//     where TPrevious : struct, ILinqEnumerator<T>, IObjectAddressFixed
// {
//     private TPrevious _previous;
//     private readonly TDelegate _predicate = default!;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public WherePtr(TPrevious prev, TDelegate predicate)
//     {
//         if (!typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)) &&
//             !typeof(TPrevious).IsAssignableTo(typeof(ILinqPtrEnumerator<T>)))
//         {
//             ThrowHelper.PreviousEnumeratorTypeNotSupported(nameof(prev));
//             return;
//         }
//
//         if (!typeof(TDelegate).IsAssignableTo(typeof(IFuncT1InInvoker<T, bool>)) &&
//             !typeof(TDelegate).IsAssignableTo(typeof(IFuncInvoker<T, bool>)) &&
//             !typeof(TDelegate).IsAssignableTo(typeof(IFuncT1PtrInvoker<T, bool>)))
//         {
//             ThrowHelper.DelegateTypeNotSupported(nameof(predicate));
//             return;
//         }
//
//         _previous = prev;
//         _predicate = predicate;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         while (_previous.MoveNext())
//             if (typeof(TDelegate).IsAssignableTo(typeof(IFuncT1InInvoker<T, bool>)))
//             {
//                 if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//                 {
//                     if (((IFuncT1InInvoker<T, bool>) _predicate).Invoke(in ((ILinqRefEnumerator<T>) _previous).Current))
//                         return true;
//                 }
//                 else
//                 {
//                     unsafe
//                     {
//                         if (((IFuncT1InInvoker<T, bool>) _predicate).Invoke(
//                                 in Unsafe.AsRef<T>(((ILinqPtrEnumerator<T>) _previous).Current))) return true;
//                     }
//                 }
//             }
//             else if (typeof(TDelegate).IsAssignableTo(typeof(IFuncInvoker<T, bool>)))
//             {
//                 if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//                 {
//                     if (((IFuncInvoker<T, bool>) _predicate).Invoke(((ILinqRefEnumerator<T>) _previous).Current))
//                         return true;
//                 }
//                 else
//                 {
//                     unsafe
//                     {
//                         if (((IFuncInvoker<T, bool>) _predicate).Invoke(
//                                 Unsafe.AsRef<T>(((ILinqPtrEnumerator<T>) _previous).Current))) return true;
//                     }
//                 }
//             }
//             else if (typeof(TDelegate).IsAssignableTo(typeof(IFuncT1PtrInvoker<T, bool>)))
//             {
//                 if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//                     unsafe
//                     {
//                         if (((IFuncT1PtrInvoker<T, bool>) _predicate).Invoke(
//                                 (T*) Unsafe.AsPointer(ref ((ILinqRefEnumerator<T>) _previous).Current))) return true;
//                     }
//                 else
//                     unsafe
//                     {
//                         if (((IFuncT1PtrInvoker<T, bool>) _predicate).Invoke(
//                                 ((ILinqPtrEnumerator<T>) _previous).Current)) return true;
//                     }
//             }
//
//         return false;
//     }
//
//     public ref T Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get
//         {
//             unsafe
//             {
//                 if (typeof(TPrevious).IsAssignableTo(typeof(ILinqPtrEnumerator<T>)))
//                     return ref Unsafe.AsRef<T>(((ILinqPtrEnumerator<T>) _previous).Current);
//                 else
//                     return ref ((ILinqRefEnumerator<T>) _previous).Current;
//             }
//         }
//     }
//
//     public int? Count => null;
//     public int? MaxCount => _previous.MaxCount;
//
//     public WherePtr<T, TPrevious, TDelegate> GetEnumerator()
//     {
//         return this;
//     }
//
//     public void Dispose()
//     {
//         _previous.Dispose();
//     }
//
//     unsafe T* ILinqPtrEnumerator<T>.Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqPtrEnumerator<T>)))
//                 return ((ILinqPtrEnumerator<T>) _previous).Current;
//             else
//                 return (T*) Unsafe.AsPointer<T>(ref ((ILinqRefEnumerator<T>) _previous).Current);
//         }
//     }
// }
public struct WherePtrToPtr<T, TPrevious, TPredicate> : ILinqRefEnumerator<T>, IItemAddressFixed,
    ILinqEnumerable<T, WherePtrToPtr<T, TPrevious, TPredicate>>
    where TPrevious : ILinqRefEnumerator<T>, IItemAddressFixed
    where TPredicate : struct
    where T : unmanaged
{
    private TPrevious _previous = default!;
    private readonly TPredicate _predicate = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WherePtrToPtr(TPrevious prev, TPredicate predicate)
    {
        if (!typeof(TPredicate).IsAssignableTo(typeof(IFuncT1InInvoker<T, bool>)) &&
            !typeof(TPredicate).IsAssignableTo(typeof(IFuncInvoker<T, bool>)) &&
            !typeof(TPredicate).IsAssignableTo(typeof(IFuncT1PtrInvoker<T, bool>)))
        {
            ThrowHelper.DelegateTypeNotSupported(nameof(predicate));
            return;
        }

        _previous = prev;
        _predicate = predicate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        while (_previous.MoveNext())
            if (typeof(TPredicate).IsAssignableTo(typeof(IFuncT1InInvoker<T, bool>)))
            {
                if (((IFuncT1InInvoker<T, bool>) _predicate).Invoke(
                        in _previous.Current)) return true;
            }
            else if (typeof(TPredicate).IsAssignableTo(typeof(IFuncInvoker<T, bool>)))
            {
                if (((IFuncInvoker<T, bool>) _predicate).Invoke(
                        _previous.Current)) return true;
            }
            else if (typeof(TPredicate).IsAssignableTo(typeof(IFuncT1PtrInvoker<T, bool>)))
            {
                unsafe
                {
                    if (((IFuncT1PtrInvoker<T, bool>) _predicate).Invoke(
                            _previous.CurrentPtr)) return true;
                }
            }

        return false;
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

    public int? Count => null;
    public int? MaxCount => _previous.MaxCount;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public WherePtrToPtr<T, TPrevious, TPredicate> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public unsafe T* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _previous.CurrentPtr;
    }

    public bool SetSkipCount(int count) => false;
    public bool SetTakeCount(int count) => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}