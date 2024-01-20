using System.Diagnostics;
using System.Runtime.CompilerServices;
using NullGC.Allocators;

namespace NullGC;

/// <summary>
/// A wrapper to make unmanaged struct work like a class (a pointer) without using 'ref', by allocating struct on unmanaged heap.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// <see cref="Borrow"/> is needed when passed by value to avoid double-free.
/// </remarks>
public readonly struct Allocated<T> : ISingleDisposable<Allocated<T>> where T : unmanaged
{
    public readonly unsafe T* Value;
    public readonly int AllocatorProviderId;

    private unsafe Allocated(T* value, int allocatorProviderId)
    {
        Value = value;
        AllocatorProviderId = allocatorProviderId;
    }

    public Allocated(AllocatorTypes allocatorProviderId) : this((int) allocatorProviderId)
    {
    }

    public Allocated(int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        AllocatorProviderId = allocatorProviderId;
        unsafe
        {
            var ptr = AllocatorContext.GetAllocator(allocatorProviderId).Allocate((uint) sizeof(T))
                .ToPointer();
            Debug.Assert((UIntPtr) ptr % (nuint) UIntPtr.Size == 0);
            Unsafe.InitBlock(ptr, 0, (uint) sizeof(T));
            Value = (T*) ptr;
        }
    }

    /// <summary>
    /// Allocate memory for <typeparamref name="T"/> from default scoped allocator, dispose is not mandatory.
    /// </summary>
    /// <returns></returns>
    public static Allocated<T> CreateScoped() => new((int) AllocatorTypes.Default);

    /// <summary>
    /// Allocate memory for <typeparamref name="T"/> from default unscoped allocator, dispose is mandatory when lifetime ends.
    /// </summary>
    /// <returns></returns>
    public static Allocated<T> CreateUnscoped() => new((int) AllocatorTypes.DefaultUnscoped);

    public ref T Ref
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return ref Unsafe.AsRef<T>(Value);
            }
        }
    }

    public Allocated<T> Borrow()
    {
        unsafe
        {
            return new Allocated<T>(Value, AllocatorProviderId);
        }
    }

    public void Dispose()
    {
        if (AllocatorProviderId == (int) AllocatorTypes.Invalid)
            return;

        unsafe
        {
            AllocatorContext.GetAllocator(AllocatorProviderId).Free((UIntPtr) Value);
        }
    }
}