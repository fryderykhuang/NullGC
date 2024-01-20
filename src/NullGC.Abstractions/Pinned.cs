using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace NullGC;

/// <summary>
/// Make managed pointer (object) works like a unmanaged pointer.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Since pin a managed object on the GC heap will reduce the effectiveness of GC compaction, either use and dispose in a short time or pin as early in the application lifetime as possible.
/// </remarks>
public struct Pinned<T> : IAddressFixed, ISingleDisposable<Pinned<T>> where T : class
{
    public readonly unsafe T* Ptr;
    private GCHandle _pin;

    private unsafe Pinned(T* ptr)
    {
        Ptr = ptr;
        _pin = default;
    }
    
    public Pinned(T obj)
    {
        unsafe
        {
            _pin = GCHandle.Alloc(obj, GCHandleType.Pinned);
            Ptr = (T*) _pin.AddrOfPinnedObject();
        }
    }

    public ref T Ref
    {
        get
        {
            unsafe
            {
                return ref Unsafe.AsRef<T>(Ptr);
            }
        }
    }

    public Pinned<T> Borrow()
    {
        unsafe
        {
            return new Pinned<T>(Ptr);
        }
    }

    public void Dispose()
    {
        if (_pin.IsAllocated)
            _pin.Free();
    }
}