using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Extensions;

namespace NullGC.Allocators;

public static class AllocatorContext
{
    public static IAllocatorContextImpl Impl = null!;

    public static void SetImplementation(IAllocatorContextImpl impl)
    {
        Impl.TryDispose();
        Impl = impl;
    }

    internal static bool ResetImplementation(IAllocatorContextImpl? impl)
    {
        if (Impl != null)
        {
            Impl.TryDispose();
            Impl = impl!;
            return true;
        }
        Impl = impl!;
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
        Impl.FreeAllocations(allocatorProviderId);
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
        Impl.SetAllocatorProvider(provider, allocatorProviderId, scoped);
    }

    public static void FinalizeConfiguration()
    {
        GuardImpl();
        Impl.FinalizeConfiguration();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GuardImpl()
    {
        if (Impl is null)
            ThrowHelper.ThrowInvalidOperationException(
                $"Implementation is not set, call {nameof(SetImplementation)} first.");
    }

    public static void ClearProvidersAndAllocations()
    {
        GuardImpl();
        Impl.ClearProvidersAndAllocations();
    }

    public static IMemoryAllocator GetAllocator(int allocatorProviderId = (int) AllocatorProviderIds.Default)
    {
        GuardImpl();
        return Impl.GetAllocator(allocatorProviderId);
    }

    /// <summary>
    /// Begin a new allocation scope, all allocations made inside the scope to the specified <paramref name="allocatorProviderId"/> will be forcibly freed after scope being disposed even if allocated memories are being referenced. 
    /// </summary>
    /// <param name="allocatorProviderId"></param>
    /// <returns></returns>
    public static IDisposable BeginAllocationScope(int allocatorProviderId = (int) AllocatorProviderIds.Default)
    {
        GuardImpl();
        return Impl.BeginAllocationScope(allocatorProviderId);
    }
}