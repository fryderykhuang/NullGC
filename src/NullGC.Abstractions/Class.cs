using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NullGC.Allocators;

namespace NullGC;

/// <summary>
/// A wrapper to make unmanaged struct work like a class (a pointer) without using 'ref', by allocating struct memory on unmanaged heap.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Class<T> : IDisposable where T : unmanaged
{
    public readonly unsafe T* Value;
    public readonly int AllocatorProviderId;

    public Class(int allocatorProviderId = (int) AllocatorProviderIds.Default)
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
    public static Class<T> CreateScoped() => new((int) AllocatorProviderIds.Default);

    /// <summary>
    /// Allocate memory for <typeparamref name="T"/> from default unscoped allocator, dispose is mandatory when lifetime ends.
    /// </summary>
    /// <returns></returns>
    public static Class<T> CreateUnscoped() => new((int) AllocatorProviderIds.DefaultUnscoped);

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

    public void Dispose()
    {
        if (AllocatorProviderId == (int) AllocatorProviderIds.Invalid)
            return;
        
        unsafe
        {
            AllocatorContext.GetAllocator(AllocatorProviderId).Free((UIntPtr) Value);
        }
    }
}