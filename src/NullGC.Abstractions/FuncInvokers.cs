using System.Runtime.CompilerServices;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace NullGC;

public interface IFuncInvoker<in T1, in T2, out TR>
{
    TR Invoke(T1 t1, T2 t2);
}

public interface IFuncInvoker<in T1, out TR>
{
    TR Invoke(T1 t1);
}

public interface IFuncT1PtrInvoker<T1, out TR>
{
    unsafe TR Invoke(T1* t1);
}

public interface IFuncT1TRPtrInvoker<T1, TR>
{
    unsafe TR* Invoke(T1* t1);
}

public readonly struct FuncT1TRPtrInvoker<T1, TR> : IFuncT1TRPtrInvoker<T1, TR>
{
    private readonly FuncT1TRPtr<T1, TR> _func;

    public FuncT1TRPtrInvoker(FuncT1TRPtr<T1, TR> func)
    {
        _func = func;
    }

    public unsafe TR* Invoke(T1* t1)
    {
        return _func(t1);
    }
}

public readonly struct FuncT1PtrInvoker<T1, TR> : IFuncT1PtrInvoker<T1, TR>
{
    private readonly FuncT1Ptr<T1, TR> _func;

    public FuncT1PtrInvoker(FuncT1Ptr<T1, TR> func) => _func = func;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe TR Invoke(T1* t1) => _func(t1);
}

public interface IFuncT1T1Invoker<in T1, out TR>
{
    TR Invoke(T1 t1, T1 t2);
}

public interface IFuncT1T1ScopedRefReadOnlyInvoker<T1, out TR>
{
    TR Invoke(scoped ref readonly T1 t1, scoped ref readonly T1 t2);
}

public interface IFuncT1T2RefInvoker<T1, T2, out TR>
{
    TR Invoke(ref T1 t1, ref T2 t2);
}

public interface IFuncT1RefInvoker<T1, out TR>
{
    TR Invoke(ref T1 t1);
}

public interface IFuncT1TRScopedRefReadOnlyInvoker<T1, TR>
{
    ref readonly TR Invoke(scoped ref readonly T1 t1);
}

public interface IFuncT1ScopedRefReadOnlyInvoker<T1, TR>
{
    TR Invoke(scoped ref readonly T1 t1);
}

public interface IFuncT1InInvoker<T1, out TR>
{
    TR Invoke(in T1 t1);
}

public interface IFuncT1TRRefInvoker<T1, TR>
{
    ref TR Invoke(ref T1 t1);
}

public readonly struct FuncT1TRScopedRefReadOnlyInvoker<T1, TR> : IFuncT1TRScopedRefReadOnlyInvoker<T1, TR>
{
    private readonly FuncT1TRScopedRefReadOnly<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1TRScopedRefReadOnlyInvoker(FuncT1TRScopedRefReadOnly<T1, TR> inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly TR Invoke(scoped ref readonly T1 t1)
    {
        return ref _inner(in t1);
    }
}

public readonly struct FuncT1ScopedRefReadOnlyWithPtrArgInvoker<T1, TArg, TR> : IFuncT1ScopedRefReadOnlyInvoker<T1, TR>
    where TArg : unmanaged
{
    private readonly FuncT1ScopedRefReadOnlyT2ScopedRef<T1, TArg, TR> _inner;
    private readonly unsafe TArg* _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1ScopedRefReadOnlyWithPtrArgInvoker(FuncT1ScopedRefReadOnlyT2ScopedRef<T1, TArg, TR> inner,
        ref TArg arg)
    {
        _inner = inner;
        unsafe
        {
            _arg = (TArg*) Unsafe.AsPointer(ref arg);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(scoped ref readonly T1 t1)
    {
        unsafe
        {
            return _inner(in t1, ref Unsafe.AsRef<TArg>(_arg));
        }
    }
}

public readonly struct FuncT1T2RefInvoker<T1, T2, TR> : IFuncT1T2RefInvoker<T1, T2, TR>
{
    private readonly FuncT1T2Ref<T1, T2, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1T2RefInvoker(FuncT1T2Ref<T1, T2, TR> inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(ref T1 t1, ref T2 t2)
    {
        return _inner(ref t1, ref t2);
    }
}

public readonly struct FuncInvoker<T1, TR> : IFuncInvoker<T1, TR>
{
    private readonly Func<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncInvoker(Func<T1, TR> inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(T1 t1)
    {
        return _inner(t1);
    }
}

public readonly struct FuncInvokerWithArg<T1, TArg, TR> : IFuncInvoker<T1, TR>
{
    private readonly Func<T1, TArg, TR> _inner;
    private readonly TArg _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncInvokerWithArg(Func<T1, TArg, TR> inner, TArg arg)
    {
        _inner = inner;
        _arg = arg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(T1 t1)
    {
        return _inner(t1, _arg);
    }
}

public readonly struct FuncInvoker<T1, T2, TR> : IFuncInvoker<T1, T2, TR>
{
    private readonly Func<T1, T2, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncInvoker(Func<T1, T2, TR> inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(T1 t1, T2 t2)
    {
        return _inner(t1, t2);
    }
}

public readonly struct FuncWithArgInvoker<T1, TArg, TR> : IFuncInvoker<T1, TR>
{
    private readonly Func<T1, TArg, TR> _inner;
    private readonly TArg _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncWithArgInvoker(Func<T1, TArg, TR> inner, TArg arg)
    {
        _inner = inner;
        _arg = arg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(T1 t1)
    {
        return _inner(t1, _arg);
    }
}

public readonly struct FuncT1T1ScopedRefReadOnlyInvoker<T1, TR> : IFuncT1T1ScopedRefReadOnlyInvoker<T1, TR>
{
    private readonly FuncT1T1ScopedRefReadOnly<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1T1ScopedRefReadOnlyInvoker(FuncT1T1ScopedRefReadOnly<T1, TR> inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(scoped ref readonly T1 t1, scoped ref readonly T1 t2)
    {
        return _inner(in t1, in t2);
    }
}

public readonly struct
    FuncT1T1ScopedRefReadOnlyWithPtrArgInvoker<T1, TArg, TR> : IFuncT1T1ScopedRefReadOnlyInvoker<T1, TR>
    where TArg : unmanaged
{
    private readonly FuncT1T1ScopedRefReadOnlyT2ScopedRef<T1, TArg, TR> _inner;
    private unsafe readonly TArg* _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1T1ScopedRefReadOnlyWithPtrArgInvoker(FuncT1T1ScopedRefReadOnlyT2ScopedRef<T1, TArg, TR> inner,
        ref TArg arg)
    {
        _inner = inner;
        unsafe
        {
            _arg = (TArg*) Unsafe.AsPointer(ref arg);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(scoped ref readonly T1 t1_1, scoped ref readonly T1 t1_2)
    {
        unsafe
        {
            return _inner(in t1_1, in t1_2, ref Unsafe.AsRef<TArg>(_arg));
        }
    }
}

public interface IHasPtr
{
    UIntPtr Ptr { get; }
}

public readonly struct PtrKeyPtrValuePair<TKey, TValue> where TKey : unmanaged where TValue : unmanaged
{
    public readonly unsafe TKey* Key;
    public readonly unsafe TValue* Value;

    public unsafe PtrKeyPtrValuePair(TKey* key, TValue* value)
    {
        Key = key;
        Value = value;
    }
}

public readonly struct KeyPtrValuePair<TKey, TValue> where TValue : unmanaged
{
    public readonly TKey Key;
    public readonly unsafe TValue* Value;

    public unsafe KeyPtrValuePair(TKey key, TValue* value)
    {
        Key = key;
        Value = value;
    }
}

public struct RawKeyValuePair<TKey, TValue> where TValue : unmanaged where TKey : unmanaged
{
    public TKey Key;
    public TValue Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RawKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

//public readonly struct PtrComparerInvoker<TKey, TValue, TR, TInner> : IFuncT1T1ScopedRefReadOnlyInvoker<KeyPtrValuePair<TKey, TValue>, TR>
//    where TInner : IFuncT1T1ScopedRefReadOnlyInvoker<TKey, TR> where TValue : unmanaged
//{
//    private readonly TInner _inner;

//    public PtrComparerInvoker(TInner inner)
//    {
//        _inner = inner;
//    }

//    public TR Invoke(scoped ref readonly KeyPtrValuePair<TKey, TValue> t1_1, scoped ref readonly KeyPtrValuePair<TKey, TValue> t1_2)
//    {
//        unsafe
//        {
//            return _inner.Invoke(in Unsafe.AsRef<TKey>(in t1_1.Key), in Unsafe.AsRef<TKey>(in t1_2.Key));
//        }
//    }
//}
public readonly struct
    KeyPtrValuePairWrappedKeyComparer<TKey, TValue, TInner> : IComparer<KeyPtrValuePair<TKey, TValue>>
    where TInner : IComparer<TKey> where TValue : unmanaged
{
    private readonly TInner _inner;

    public KeyPtrValuePairWrappedKeyComparer(TInner inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(KeyPtrValuePair<TKey, TValue> a, KeyPtrValuePair<TKey, TValue> b)
    {
        return _inner.Compare(a.Key, b.Key);
    }
}

public readonly struct RawKeyValuePairKeyComparer<TKey, TValue, TInner> : IComparer<RawKeyValuePair<TKey, TValue>>
    where TInner : IComparer<TKey> where TValue : unmanaged where TKey : unmanaged
{
    private readonly TInner _inner;

    public RawKeyValuePairKeyComparer(TInner inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(RawKeyValuePair<TKey, TValue> a, RawKeyValuePair<TKey, TValue> b)
    {
        return _inner.Compare(a.Key, b.Key);
    }
}

public readonly struct
    RawKeyValuePairValueComparer<TKey, TValue, TInner> : IComparer<RawKeyValuePair<TKey, TValue>>
    where TInner : IComparer<TValue> where TValue : unmanaged where TKey : unmanaged
{
    private readonly TInner _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RawKeyValuePairValueComparer(TInner inner)
    {
        _inner = inner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(RawKeyValuePair<TKey, TValue> a, RawKeyValuePair<TKey, TValue> b)
    {
        return _inner.Compare(a.Value, b.Value);
    }
}

public readonly struct StructComparer<T> : IComparer<T>
{
    private readonly Comparison<T> _comparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructComparer(Comparison<T> comparer) => _comparer = comparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y) => _comparer(x!, y!);
}

public struct StructComparerWithPtrArg<T, TArg> : IComparer<T> where TArg : unmanaged
{
    private readonly ComparisonWithRefArg<T, TArg> _comparer;
    private unsafe TArg* _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructComparerWithPtrArg(ComparisonWithRefArg<T, TArg> comparer, ref TArg arg)
    {
        _comparer = comparer;
        unsafe
        {
            _arg = (TArg*) Unsafe.AsPointer(ref arg);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y)
    {
        unsafe
        {
            return _comparer(x!, y!, ref Unsafe.AsRef<TArg>(_arg));
        }
    }
}

public readonly struct StructComparerWithArg<T, TArg> : IComparer<T> where TArg : unmanaged
{
    private readonly ComparisonWithArg<T, TArg> _comparer;
    private readonly TArg _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructComparerWithArg(ComparisonWithArg<T, TArg> comparer, TArg arg)
    {
        _comparer = comparer;
        _arg = arg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y) => _comparer(x!, y!, _arg);
}

public readonly struct FuncT1RefT2Invoker<T1, TArg, TR> : IFuncT1RefInvoker<T1, TR>
{
    private readonly FuncT1Ref<T1, TArg, TR> _inner;
    private readonly TArg _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1RefT2Invoker(FuncT1Ref<T1, TArg, TR> inner, TArg arg)
    {
        _inner = inner;
        _arg = arg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(ref T1 t1) => _inner(ref t1, _arg);
}

public readonly struct FuncT1RefInvoker<T1, TR> : IFuncT1RefInvoker<T1, TR>
{
    private readonly FuncT1Ref<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1RefInvoker(FuncT1Ref<T1, TR> inner) => _inner = inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(ref T1 t1) => _inner(ref t1);
}

public readonly struct FuncT1ScopedRefReadOnlyInvoker<T1, TR> : IFuncT1ScopedRefReadOnlyInvoker<T1, TR>
{
    private readonly FuncT1ScopedRefReadOnly<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1ScopedRefReadOnlyInvoker(FuncT1ScopedRefReadOnly<T1, TR> inner) => _inner = inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(scoped ref readonly T1 t1) => _inner(in t1);
}

public readonly struct FuncT1InInvoker<T1, TR> : IFuncT1InInvoker<T1, TR>
{
    private readonly FuncT1In<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1InInvoker(FuncT1In<T1, TR> inner) => _inner = inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(in T1 t1) => _inner(in t1);
}

public readonly struct FuncT1TRRefInvoker<T1, TR> : IFuncT1TRRefInvoker<T1, TR>
{
    private readonly FuncT1TRRef<T1, TR> _inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1TRRefInvoker(FuncT1TRRef<T1, TR> inner) => _inner = inner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TR Invoke(ref T1 t1) => ref _inner(ref t1);
}

public readonly struct FuncT1TRRefWithArgInvoker<T1, TArg, TR> : IFuncT1TRRefInvoker<T1, TR>
{
    private readonly FuncT1TRRef<T1, TArg, TR> _inner;
    private readonly TArg _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1TRRefWithArgInvoker(FuncT1TRRef<T1, TArg, TR> inner, TArg arg)
    {
        _inner = inner;
        _arg = arg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TR Invoke(ref T1 t1) => ref _inner(ref t1, _arg);
}

public readonly struct FuncT1InWithArgInvoker<T1, TArg, TR> : IFuncT1InInvoker<T1, TR>
{
    private readonly FuncT1In<T1, TArg, TR> _inner;
    private readonly TArg _arg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FuncT1InWithArgInvoker(FuncT1In<T1, TArg, TR> inner, TArg arg)
    {
        _inner = inner;
        _arg = arg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TR Invoke(in T1 t1) => _inner(in t1, _arg);
}
