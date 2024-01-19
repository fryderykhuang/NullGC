#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
using System.Runtime.CompilerServices;
namespace NullGC;

/// <summary>
/// Simple unmanaged pointer wrapper.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Ptr<T>
{
    public readonly unsafe T* Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Ptr(T* value)
    {
        Value = value;
    }

    /// <summary>
    /// Take pointer from arbitrary object.
    /// </summary>
    /// <param name="obj">The object you want to take the pointer from.</param>
    /// <returns></returns>
    /// <remarks>
    /// NOTE: Object on managed heap will be moved by GC, pin it before take the pointer.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(ref T obj)
    {
        unsafe
        {
            Value = (T*) Unsafe.AsPointer(ref obj);
        }
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator T*(Ptr<T> ptr) => ptr.Value;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator Ptr<T>(T* ptr) => new Ptr<T>(ptr);
}