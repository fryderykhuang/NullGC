using System.Collections;
using System.Runtime.CompilerServices;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace NullGC.Linq.Enumerators;

public struct SelectRefToValue<T, TPrevious, TSelector, TResult> : ILinqValueEnumerator<TResult>,
    ILinqEnumerable<TResult, SelectRefToValue<T, TPrevious, TSelector, TResult>>
    where TPrevious : ILinqRefEnumerator<T>
    where TSelector : struct,IFuncInvoker<T, TResult>
{
    private TPrevious _previous = default!;
    private TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectRefToValue(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    
    object? IEnumerator.Current => Current;

    public TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _selector.Invoke(_previous.Current);
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectRefToValue<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectRefToValueIn<T, TPrevious, TSelector, TResult> : ILinqValueEnumerator<TResult>,
    ILinqEnumerable<TResult, SelectRefToValueIn<T, TPrevious, TSelector, TResult>>
    where TPrevious : ILinqRefEnumerator<T>
    where TSelector : struct, IFuncT1InInvoker<T, TResult>
{
    private TPrevious _previous = default!;
    private TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectRefToValueIn(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    
    object? IEnumerator.Current => Current;

    public TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _selector.Invoke(in _previous.Current);
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectRefToValueIn<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectRefToRef<T, TPrevious, TSelector, TResult> : ILinqRefEnumerator<TResult>,
    ILinqEnumerable<TResult, SelectRefToRef<T, TPrevious, TSelector, TResult>>
    where TPrevious : ILinqRefEnumerator<T>
    where TSelector : struct, IFuncT1TRRefInvoker<T, TResult>
{
    private TPrevious _previous = default!;
    private TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectRefToRef(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    TResult IEnumerator<TResult>.Current => Current;

    public unsafe TResult* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TResult*) Unsafe.AsPointer(ref _selector.Invoke(ref _previous.Current));
    }

    object? IEnumerator.Current => Current;

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectRefToRef<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public ref TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _selector.Invoke(ref _previous.Current);
    }

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectValueToValue<T, TPrevious, TSelector, TResult> : ILinqValueEnumerator<TResult>,
    ILinqEnumerable<TResult, SelectValueToValue<T, TPrevious, TSelector, TResult>>
    where TPrevious : ILinqValueEnumerator<T>
    where TSelector : struct, IFuncInvoker<T, TResult>
{
    private TPrevious _previous = default!;
    private TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectValueToValue(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _selector.Invoke(_previous.Current);
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectValueToValue<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectValueToValueIn<T, TPrevious, TSelector, TResult> : ILinqValueEnumerator<TResult>,
    ILinqEnumerable<TResult, SelectValueToValueIn<T, TPrevious, TSelector, TResult>>
    where TPrevious : ILinqValueEnumerator<T>
    where TSelector : struct, IFuncT1InInvoker<T, TResult>
{
    private TPrevious _previous = default!;
    private TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectValueToValueIn(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _selector.Invoke(_previous.Current);
    }

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectValueToValueIn<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectFixedRefToFixedRef<T, TPrevious, TSelector, TResult> : ILinqRefEnumerator<TResult>,
    IAddressFixed,
    ILinqEnumerable<TResult, SelectFixedRefToFixedRef<T, TPrevious, TSelector, TResult>>
    where TSelector : IFuncT1TRRefInvoker<T, TResult>
    where TPrevious : ILinqRefEnumerator<T>, IAddressFixed
{
    private TPrevious _previous = default!;
    private readonly TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectFixedRefToFixedRef(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    TResult IEnumerator<TResult>.Current => Current;

    public unsafe TResult* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TResult*) Unsafe.AsPointer(ref _selector.Invoke(ref _previous.Current));
    }

    object? IEnumerator.Current => Current;

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectFixedRefToFixedRef<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public ref TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _selector.Invoke(ref _previous.Current);
    }

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectPtrToValue<T, TPrevious, TSelector, TResult> : ILinqValueEnumerator<TResult>,
    ILinqEnumerable<TResult, SelectPtrToValue<T, TPrevious, TSelector, TResult>>
    where TSelector : IFuncT1PtrInvoker<T, TResult>
    where TPrevious : ILinqRefEnumerator<T>
{
    private TPrevious _previous = default!;
    private readonly TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectPtrToValue(TPrevious prev, TSelector selector)
    {
        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    object? IEnumerator.Current => Current;

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectPtrToValue<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return _selector.Invoke(_previous.CurrentPtr);
            }
        }
    }

    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct SelectPtrToPtr<T, TPrevious, TSelector, TResult> : ILinqRefEnumerator<TResult>, IAddressFixed,
    ILinqEnumerable<TResult, SelectPtrToPtr<T, TPrevious, TSelector, TResult>>
    where TPrevious : ILinqRefEnumerator<T>, IAddressFixed
    where TSelector : struct
    where TResult : unmanaged
{
    private TPrevious _previous = default!;
    private readonly TSelector _selector = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SelectPtrToPtr(TPrevious prev, TSelector selector)
    {
        if (!(typeof(TSelector).IsAssignableTo(typeof(IFuncT1PtrInvoker<T, TResult>)) ||
              typeof(TSelector).IsAssignableTo(typeof(IFuncT1TRPtrInvoker<T, TResult>)) ||
              typeof(TSelector).IsAssignableTo(typeof(IFuncT1TRRefInvoker<T, TResult>))
            ))
        {
            ThrowHelper.DelegateTypeNotSupported(nameof(selector));
            return;
        }

        _previous = prev;
        _selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => _previous.MoveNext();

    public void Reset() => CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();

    public ref TResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                if (typeof(TSelector).IsAssignableTo(typeof(IFuncT1TRPtrInvoker<T, TResult>)))
                    return ref Unsafe.AsRef<TResult>(
                        ((IFuncT1TRPtrInvoker<T, TResult>) _selector).Invoke(_previous.CurrentPtr));
                else if (typeof(TSelector).IsAssignableTo(typeof(IFuncT1TRRefInvoker<T, TResult>)))
                    return ref ((IFuncT1TRRefInvoker<T, TResult>) _selector).Invoke(
                        ref _previous.Current);

                CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
                return ref Unsafe.NullRef<TResult>();
            }
        }
    }

    TResult IEnumerator<TResult>.Current
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

    public readonly int? Count => _previous.Count;
    public readonly int? MaxCount => _previous.MaxCount;

    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();

    public readonly SelectPtrToPtr<T, TPrevious, TSelector, TResult> GetEnumerator() => this;

    public void Dispose() => _previous.Dispose();

    public unsafe TResult* CurrentPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (typeof(TSelector).IsAssignableTo(typeof(IFuncT1TRPtrInvoker<T, TResult>)))
                return ((IFuncT1TRPtrInvoker<T, TResult>) _selector).Invoke(_previous.CurrentPtr);
            else if (typeof(TSelector).IsAssignableTo(typeof(IFuncT1TRRefInvoker<T, TResult>)))
                return (TResult*) Unsafe.AsPointer(ref ((IFuncT1TRRefInvoker<T, TResult>) _selector).Invoke(
                    ref _previous.Current));

            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
            return (TResult*) 0;
        }
    }
    
    public bool SetSkipCount(int count) => _previous.SetSkipCount(count);
    public bool SetTakeCount(int count) => _previous.SetTakeCount(count);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
//
//
// public struct SelectRefReturn<T, TPrevious, TDelegate, TResult> : ILinqRefEnumerator<TResult>,
//     ILinqEnumerable<TResult, SelectRefReturn<T, TPrevious, TDelegate, TResult>>
//     where TDelegate : IFuncT1TRRefInvoker<T, TResult>
//     where TPrevious : struct, ILinqRefEnumerator<T>
// {
//     private TPrevious _previous;
//     private readonly TDelegate _selector;
//     // private TResult _current = default!;
//
//     public SelectRefReturn(TPrevious prev, TDelegate selector)
//     {
//         this._previous = prev;
//         this._selector = selector;
//     }
//
//     public void Initialize()
//     {
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         if (!this._previous.MoveNext())
//             return false;
//         
//         return true;
//     }
//
//     public readonly ref TResult Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//             {
//                 return ref _selector.Invoke(ref this._previous.Current);
//             }
//             else
//             {
//                 CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
//             }
//             
//             // return _current;
//             return ref Unsafe.NullRef<TResult>();
//         }
//     }
//
//     public readonly int? Count => _previous.Count;
//     public readonly int? MaxCount => _previous.MaxCount;
//
//     public readonly SelectRefReturn<T, TPrevious, TDelegate, TResult> GetEnumerator() => this;
//
//     public void Dispose()
//     {
//         _previous.Dispose();
//     }
// }
//
//
// public struct SelectCopy<T, TPrevious, TDelegate, TResult> : ILinqValueEnumerator<TResult>,
//     ILinqEnumerable<TResult, SelectCopy<T, TPrevious, TDelegate, TResult>>
//     where TDelegate : IFuncInvoker<T, TResult>
//     where TPrevious : struct, ILinqEnumerator<T>
// {
//     private TPrevious _previous;
//     private readonly TDelegate _selector;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public SelectCopy(TPrevious prev, TDelegate selector)
//     {
//         this._previous = prev;
//         this._selector = selector;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         if (!this._previous.MoveNext())
//             return false;
//
//         return true;
//     }
//
//     public readonly TResult Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get
//         {
//             if (typeof(TPrevious).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
//             {
//                 return _selector.Invoke(((ILinqRefEnumerator<T>)this._previous).Current);
//             }
//             else if(typeof(TPrevious).IsAssignableTo(typeof(ILinqValueEnumerator<T>)))
//             {
//                 return _selector.Invoke(((ILinqValueEnumerator<T>)this._previous).Current);
//             }
//             else
//             {
//                 CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
//             }
//
//             return default;
//         }
//     }
//
//     public readonly int? Count => _previous.Count;
//     public readonly int? MaxCount => _previous.MaxCount;
//
//     public readonly SelectCopy<T, TPrevious, TDelegate, TResult> GetEnumerator() => this;
//
//     public void Dispose()
//     {
//         _previous.Dispose();
//     }
// }
//
//
// public struct SelectPtr<T, TPrevious, TDelegate, TResult> : ILinqValueEnumerator<TResult>,
//     ILinqEnumerable<TResult, SelectPtr<T, TPrevious, TDelegate, TResult>>
//     where TDelegate : IFuncT1PtrInvoker<T, TResult>
//     where TPrevious : struct, ILinqPtrEnumerator<T>
// {
//     private TPrevious _previous;
//     private readonly TDelegate _selector;
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public SelectPtr(TPrevious prev, TDelegate selector)
//     {
//         this._previous = prev;
//         this._selector = selector;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool MoveNext()
//     {
//         if (!this._previous.MoveNext())
//             return false;
//
//         return true;
//     }
//
//     public readonly TResult Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get
//         {
//             unsafe
//             {
//                 return _selector.Invoke(this._previous.Current);
//             }
//         }
//     }
//
//     public readonly int? Count => _previous.Count;
//     public readonly int? MaxCount => _previous.MaxCount;
//
//     public readonly SelectPtr<T, TPrevious, TDelegate, TResult> GetEnumerator() => this;
//
//     public void Dispose()
//     {
//         _previous.Dispose();
//     }
// }