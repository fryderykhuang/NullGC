#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
namespace NullGC;

public delegate TR FuncT1RefReadOnly<T1, in T2, out TR>(ref readonly T1 t1, T2 t2);

public delegate TR FuncT1RefReadOnly<T1, out TR>(ref readonly T1 t1);

public delegate ref readonly TR FuncT1TRScopedRefReadOnly<T1, TR>(scoped ref readonly T1 t1);

public delegate ref readonly TR FuncT1TRScopedRefReadOnly<T1, in T2, TR>(scoped ref readonly T1 t1, T2 t2);

public delegate ref readonly TR FuncT1T2TRScopedRefReadOnly<T1, T2, TR>(scoped ref readonly T1 t1,
    scoped ref readonly T2 t2);

public delegate ref readonly TR FuncT1TRScopedRefReadOnlyT2ScopedRef<T1, T2, TR>(scoped ref readonly T1 t1,
    scoped ref T2 t2);

public delegate ref TR FuncT1TRRef<T1, TR>(ref T1 t1);

public delegate ref TR FuncT1TRRef<T1, in T2, TR>(ref T1 t1, T2 t2);

public delegate TR FuncT1ScopedRefReadOnly<T1, out TR>(scoped ref readonly T1 t1);

public delegate TR FuncT1In<T1, out TR>(in T1 t1);

public unsafe delegate TR FuncT1Ptr<T1, out TR>(T1* t1);

public unsafe delegate TR* FuncT1TRPtr<T1, TR>(T1* t1);

public delegate TR FuncT1In<T1, T2, out TR>(in T1 t1, T2 t2);

public delegate TR FuncT1T2In<T1, T2, out TR>(in T1 t1, in T2 t2);

public delegate TR FuncT1InT2ScopedRef<T1, T2, out TR>(in T1 t1, scoped ref T2 t2);

public delegate TR FuncT1ScopedRefReadOnlyT2ScopedRef<T1, T2, out TR>(scoped ref readonly T1 t1, scoped ref T2 t2);

public delegate TR FuncT1T1ScopedRefReadOnly<T1, out TR>(scoped ref readonly T1 t1, scoped ref readonly T1 t2);

public delegate TR FuncT1T1ScopedRefReadOnly<T1, in T2, out TR>(scoped ref readonly T1 t1_1,
    scoped ref readonly T1 t1_2, T2 t2);

public delegate TR FuncT1T1ScopedRefReadOnlyT2ScopedRef<T1, T2, out TR>(scoped ref readonly T1 t1_1,
    scoped ref readonly T1 t1_2, scoped ref T2 t2);

public delegate TR FuncT1Ref<T1, in T2, out TR>(ref T1 t1, T2 t2);

public delegate TR FuncT1Ref<T1, out TR>(ref T1 t1);

public delegate TR FuncT1T2Ref<T1, T2, out TR>(ref T1 t1, ref T2 t2);

public delegate void ActionT1Ref<T1>(ref T1 t1);
public delegate void ActionT1Ref<T1, in T2>(ref T1 t1, T2 t2);
public delegate void ActionT1Ref<T1, in T2, in T3>(ref T1 t1, T2 t2, T3 t3);

public delegate void ActionT1RefReadOnly<T1>(ref readonly T1 t1);
public delegate void ActionT1RefReadOnly<T1, in T2>(ref readonly T1 t1, T2 t2);
public delegate void ActionT1RefReadOnly<T1, in T2, in T3>(ref readonly T1 t1, T2 t2, T3 t3);

public delegate void ActionT1T2Ref<T1, T2>(ref T1 t1, ref T2 t2);
public delegate void ActionT1T2T3Ref<T1, T2, T3>(ref T1 t1, ref T2 t2, ref T3 t3);

public delegate void ActionT1T2RefReadOnly<T1, T2>(ref readonly T1 t1, ref readonly T2 t2);
public delegate void ActionT1T2T3RefReadOnly<T1, T2, T3>(ref readonly T1 t1, ref readonly T2 t2, ref readonly T3 t3);

public delegate void ActionT3Ref<in T1, in T2, T3>(T1 t1, T2 t2, ref T3 t3);

public delegate void ActionT4Ref<in T1, in T2, in T3, T4>(T1 t1, T2 t2, T3 t3, ref T4 t4);

public delegate int ComparisonWithRefArg<T, TArg>(T a, T b, ref TArg arg);

public delegate int ComparisonWithArg<T, TArg>(T a, T b, TArg arg);