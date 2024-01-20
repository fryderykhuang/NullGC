using System.Runtime.CompilerServices;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Where

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, WhereRefToRefIn<T, TPrevious, FuncT1InInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, bool> predicate)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqRefEnumerable<T, WhereRefToRefIn<T, TPrevious, FuncT1InInvoker<T, bool>>>(
            new WhereRefToRefIn<T, TPrevious, FuncT1InInvoker<T, bool>>(src.GetEnumerator(),
                new FuncT1InInvoker<T, bool>(predicate)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, WhereFixedRefToFixedRefIn<T, TPrevious, FuncT1InInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, bool> predicate)
        where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        return new LinqFixedRefEnumerable<T, WhereFixedRefToFixedRefIn<T, TPrevious, FuncT1InInvoker<T, bool>>>(
            new WhereFixedRefToFixedRefIn<T, TPrevious, FuncT1InInvoker<T, bool>>(src.GetEnumerator(),
                new FuncT1InInvoker<T, bool>(predicate)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, WhereRefToRef<T, TPrevious, FuncInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqRefEnumerable<T, TPrevious> src,
        Func<T, bool> predicate)
        where TPrevious : struct, ILinqRefEnumerator<T>
    {
        return new LinqRefEnumerable<T, WhereRefToRef<T, TPrevious, FuncInvoker<T, bool>>>(
            new WhereRefToRef<T, TPrevious, FuncInvoker<T, bool>>(src.GetEnumerator(),
                new FuncInvoker<T, bool>(predicate)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, WhereFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqFixedRefEnumerable<T, TPrevious> src,
        Func<T, bool> predicate)
        where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        return new LinqFixedRefEnumerable<T, WhereFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, bool>>>(
            new WhereFixedRefToFixedRef<T, TPrevious, FuncInvoker<T, bool>>(src.GetEnumerator(),
                new FuncInvoker<T, bool>(predicate)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, WhereValueToValueIn<T, TPrevious, FuncT1InInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, bool> predicate)
        where TPrevious : struct, ILinqValueEnumerator<T>
    {
        return new LinqValueEnumerable<T, WhereValueToValueIn<T, TPrevious, FuncT1InInvoker<T, bool>>>(
            new WhereValueToValueIn<T, TPrevious, FuncT1InInvoker<T, bool>>(src.GetEnumerator(),
                new FuncT1InInvoker<T, bool>(predicate)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, WhereValueToValue<T, TPrevious, FuncInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqValueEnumerable<T, TPrevious> src,
        Func<T, bool> predicate)
        where TPrevious : struct, ILinqValueEnumerator<T>
    {
        return new LinqValueEnumerable<T, WhereValueToValue<T, TPrevious, FuncInvoker<T, bool>>>(
            new WhereValueToValue<T, TPrevious, FuncInvoker<T, bool>>(src.GetEnumerator(),
                new FuncInvoker<T, bool>(predicate)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqPtrEnumerable<T, WherePtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, bool>>> Where<T, TPrevious>(
        this LinqPtrEnumerable<T, TPrevious> src,
        FuncT1Ptr<T, bool> predicate)
        where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed where T : unmanaged
    {
        return new LinqPtrEnumerable<T, WherePtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, bool>>>(
            new WherePtrToPtr<T, TPrevious, FuncT1PtrInvoker<T, bool>>(src.GetEnumerator(),
                new FuncT1PtrInvoker<T, bool>(predicate)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqRefEnumerable<T, WhereRefToRefIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>> Where<
        T, TPrevious,
        TArg>(this LinqRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T> where TArg : unmanaged
    {
        return new LinqRefEnumerable<T, WhereRefToRefIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>>(
            new WhereRefToRefIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, bool>(predicate, arg)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqFixedRefEnumerable<T, WhereFixedRefToFixedRefIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>> Where<
        T, TPrevious,
        TArg>(this LinqFixedRefEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TPrevious : struct, ILinqRefEnumerator<T>, IAddressFixed where TArg : unmanaged
    {
        return new LinqFixedRefEnumerable<T, WhereFixedRefToFixedRefIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>>(
            new WhereFixedRefToFixedRefIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, bool>(predicate, arg)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinqValueEnumerable<T, WhereValueToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>> Where<
        T, TPrevious,
        TArg>(this LinqValueEnumerable<T, TPrevious> src,
        FuncT1In<T, TArg, bool> predicate, TArg arg)
        where TPrevious : struct, ILinqValueEnumerator<T> where TArg : unmanaged
    {
        return new LinqValueEnumerable<T, WhereValueToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>>(
            new WhereValueToValueIn<T, TPrevious, FuncT1InWithArgInvoker<T, TArg, bool>>(src.GetEnumerator(),
                new FuncT1InWithArgInvoker<T, TArg, bool>(predicate, arg)));
    }

    #endregion
}