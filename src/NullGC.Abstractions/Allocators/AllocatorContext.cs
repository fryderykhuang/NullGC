using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Extensions;

namespace NullGC.Allocators;

public static class AllocatorContext
{
    private static IAllocatorContextImpl _impl = null!;
    public static IAllocatorContextImpl Impl => _impl;

    public static void SetImplementation(IAllocatorContextImpl impl)
    {
        _impl.TryDispose();
        _impl = impl;
    }

    internal static bool ResetImplementation(IAllocatorContextImpl? impl)
    {
        if (_impl != null)
        {
            _impl.TryDispose();
            _impl = impl!;
            return true;
        }
        _impl = impl!;
        return false;
    }

    /// <summary>
    ///     Free all allocations out there for specified allocator provider.
    /// </summary>
    /// <remarks>
    ///     If there are any un-freed allocations lingering there, it will become invalid, which is dangerous.
    /// </remarks>
    public static void FreeAllocations(int allocatorProviderId)
    {
        GuardImpl();
        _impl.FreeAllocations(allocatorProviderId);
    }

    /// <summary>
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="allocatorProviderId"></param>
    /// <param name="scoped"></param>
    /// <remarks>This method must be called before any other code that uses the allocator context infrastructure. </remarks>
    public static void SetAllocatorProvider(IAllocatorProvider provider, int allocatorProviderId, bool scoped)
    {
        GuardImpl();
        _impl.SetAllocatorProvider(provider, allocatorProviderId, scoped);
    }

    public static void FinalizeConfiguration()
    {
        GuardImpl();
        _impl.FinalizeConfiguration();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GuardImpl()
    {
        if (_impl is null)
            ThrowHelper.ThrowInvalidOperationException(
                $"Implementation is not set, call {nameof(SetImplementation)} first.");
    }

    public static void ClearProvidersAndAllocations()
    {
        GuardImpl();
        _impl.ClearProvidersAndAllocations();
    }

    public static IMemoryAllocator GetAllocator(int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        GuardImpl();
        return _impl.GetAllocator(allocatorProviderId);
    }

    /// <summary>
    /// Begin a new allocation scope, all allocations made inside the scope to the specified <paramref name="allocatorProviderId"/> will be forcibly freed after scope being disposed even if allocated memories are being referenced. 
    /// </summary>
    /// <param name="allocatorProviderId"></param>
    /// <returns></returns>
    public static IDisposable BeginAllocationScope(int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        GuardImpl();
        return _impl.BeginAllocationScope(allocatorProviderId);
    }
}