using System.Runtime.CompilerServices;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Select

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>>(
            new SelectRefToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1InInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<TResult, SelectRefToRef<T, TPrevious, FuncT1TRRefInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1TRRef<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqRefEnumerable<TResult, SelectRefToRef<T, TPrevious, FuncT1TRRefInvoker<T, TResult>, TResult>>(
            new SelectRefToRef<T, TPrevious, FuncT1TRRefInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1TRRefInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<TResult, SelectFixedRefToFixedRef<T, TPrevious, FuncT1TRRefInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1TRRef<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqFixedRefEnumerable<TResult, SelectFixedRefToFixedRef<T, TPrevious, FuncT1TRRefInvoker<T, TResult>, TResult>>(
            new SelectFixedRefToFixedRef<T, TPrevious, FuncT1TRRefInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1TRRefInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqRefEnumerable<T, TPrevious> src,
        Func<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqValueEnumerable<TResult, SelectRefToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>>(
            new SelectRefToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncInvoker<T, TResult>(selector)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>>(
            new SelectRefToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1InInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        Func<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqValueEnumerable<TResult, SelectRefToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>>(
            new SelectRefToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValue<T, TPrevious, FuncInvokerWithArg<T, TArg, TResult>, TResult>> Select<T, TPrevious, TResult, TArg>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        Func<T, TArg, TResult> selector, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqValueEnumerable<TResult, SelectRefToValue<T, TPrevious, FuncInvokerWithArg<T, TArg, TResult>, TResult>>(
            new SelectRefToValue<T, TPrevious, FuncInvokerWithArg<T, TArg, TResult>, TResult>(src.GetEnumerator(),
                new FuncInvokerWithArg<T, TArg, TResult>(selector, arg)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectPtrToValue<T, TPrevious, FuncT1PtrInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqPtrEnumerable<T, TPrevious> src,
        FuncT1Ptr<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed
    {
        return new LinqValueEnumerable<TResult, SelectPtrToValue<T, TPrevious, FuncT1PtrInvoker<T, TResult>, TResult>>(
            new SelectPtrToValue<T, TPrevious, FuncT1PtrInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1PtrInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<TResult, SelectPtrToPtr<T, TPrevious, FuncT1TRPtrInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqPtrEnumerable<T, TPrevious> src,
        FuncT1TRPtr<T, TResult> selector)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TResult : unmanaged
    {
        return new LinqPtrEnumerable<TResult, SelectPtrToPtr<T, TPrevious, FuncT1TRPtrInvoker<T, TResult>, TResult>>(
            new SelectPtrToPtr<T, TPrevious, FuncT1TRPtrInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1TRPtrInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectValueToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqValueEnumerable<T, TPrevious> src,
        Func<T, TResult> selector)
        where TPrevious : struct, ILinqValueEnumerator<T>
    {
        return new LinqValueEnumerable<TResult, SelectValueToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>>(
            new SelectValueToValue<T, TPrevious, FuncInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncInvoker<T, TResult>(selector)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectValueToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>> Select<T, TPrevious, TResult>(
        this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TResult> selector)
        where TPrevious : struct, ILinqValueEnumerator<T>
    {
        return new LinqValueEnumerable<TResult, SelectValueToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>>(
            new SelectValueToValueIn<T, TPrevious, FuncT1InInvoker<T, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1InInvoker<T, TResult>(selector)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>> Select<
        T, TPrevious, TResult,
        TArg>(this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TResult> predicate, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T> where TArg : unmanaged
    {
        return new LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>>(
            new SelectRefToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TResult>(predicate, arg)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>> Select<
        T, TPrevious, TResult,
        TArg>(this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TResult> predicate, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TArg : unmanaged
    {
        return new LinqValueEnumerable<TResult, SelectRefToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>>(
            new SelectRefToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TResult>(predicate, arg)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<TResult, SelectFixedRefToFixedRef<T, TPrevious, FuncT1TRRefWithArgInvoker<T, TArg, TResult>, TResult>> Select<
        T, TPrevious, TResult,
        TArg>(this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1TRRef<T, TArg, TResult> predicate, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IItemAddressFixed where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<TResult, SelectFixedRefToFixedRef<T, TPrevious, FuncT1TRRefWithArgInvoker<T, TArg, TResult>, TResult>>(
            new SelectFixedRefToFixedRef<T, TPrevious, FuncT1TRRefWithArgInvoker<T, TArg, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1TRRefWithArgInvoker<T, TArg, TResult>(predicate, arg)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectValueToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>> Select<
        T, TPrevious, TResult,
        TArg>(this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, TResult> predicate, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T> where TArg : unmanaged
    {
        return new LinqValueEnumerable<TResult, SelectValueToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>>(
            new SelectValueToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, TResult>, TResult>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, TResult>(predicate, arg)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<TResult, SelectValueToValue<T, TPrevious, FuncWithArgInvoker<T, TArg, TResult>, TResult>> Select<
        T, TPrevious, TResult,
        TArg>(this LinqValueEnumerable<T, TPrevious> src,
        Func<T, TArg, TResult> predicate, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T> where TArg : unmanaged
    {
        return new LinqValueEnumerable<TResult, SelectValueToValue<T, TPrevious, FuncWithArgInvoker<T, TArg, TResult>, TResult>>(
            new SelectValueToValue<T, TPrevious, FuncWithArgInvoker<T, TArg, TResult>, TResult>(src.GetEnumerator(),
                new FuncWithArgInvoker<T, TArg, TResult>(predicate, arg)));
    }

    #endregion

}