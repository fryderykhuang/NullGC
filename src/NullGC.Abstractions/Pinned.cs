using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NullGC;

/// <summary>
/// Make managed pointer (object) works like a unmanaged pointer.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Since pin a managed object on the GC heap will reduce the effectiveness of GC compaction, either use and dispose in a short time or pin as early in the application lifetime as possible.
/// </remarks>
public struct Pinned<T> : IDisposable where T : class
{
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    public readonly unsafe T* Ptr;
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    private GCHandle _pin;

    public Pinned(T obj)
    {
        unsafe
        {
            _pin = GCHandle.Alloc(obj, GCHandleType.Pinned);
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            Ptr = (T*) Unsafe.AsPointer(ref obj);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
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

    public void Dispose()
    {
        _pin.Free();
    }
}