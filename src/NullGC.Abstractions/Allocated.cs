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
public struct Allocated<T> : IExplicitOwnership<Allocated<T>> where T : unmanaged
{
    public readonly unsafe T* Value;
    private int _allocatorProviderId;
    public int AllocatorProviderId => _allocatorProviderId;

    private unsafe Allocated(T* value, int allocatorProviderId)
    {
        Value = value;
        _allocatorProviderId = allocatorProviderId;
    }

    public Allocated(AllocatorTypes allocatorProviderId) : this((int) allocatorProviderId)
    {
    }

    public Allocated(int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _allocatorProviderId = allocatorProviderId;
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
            return new Allocated<T>(Value, _allocatorProviderId);
        }
    }

    public Allocated<T> Take()
    {
        unsafe
        {
            var allocId = AllocatorProviderId;
            _allocatorProviderId = (int) AllocatorTypes.Invalid;
            return new Allocated<T>(Value, allocId);
        }
    }

    public void Dispose()
    {
        if (_allocatorProviderId == (int) AllocatorTypes.Invalid)
            return;

        unsafe
        {
            AllocatorContext.GetAllocator(_allocatorProviderId).Free((UIntPtr) Value);
        }
    }
}