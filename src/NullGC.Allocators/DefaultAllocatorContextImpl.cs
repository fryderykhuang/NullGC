using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Extensions;

namespace NullGC.Allocators;

public sealed class DefaultAllocatorContextImpl : IAllocatorContextImpl, IDisposable
{
    private FrozenDictionary<int, PerProviderContainer>? _allocatorProviders;
    private readonly Dictionary<int, PerProviderContainer> _tmpAllocatorProviders = new(4);
    private readonly IAllocatorProvider _defaultUncachedUnscopedAllocator = new DefaultAlignedNativeMemoryAllocator();

    internal readonly Stack<ContextContainer> ContextPool = new(Environment.ProcessorCount * 2 * 4);

    public void ClearProvidersAndAllocations()
    {
        GuardConfigured();
        if (_allocatorProviders != null)
            foreach (var val in _allocatorProviders.Values)
            {
                val.Context?.Value?.FreeAllocations();
                val.Provider.TryDispose();
            }

        lock (ContextPool)
            while (ContextPool.TryPop(out var context))
                context.FreeAllocations();

        _allocatorProviders = null;
        Debug.Assert(_tmpAllocatorProviders.Count == 0);
        _tmpAllocatorProviders.Clear();
    }

    /// <summary>
    ///     Free all allocations out there for specified allocator provider.
    /// </summary>
    /// <remarks>
    ///     If there are any un-freed allocations lingering there, it will become invalid, which is dangerous.
    /// </remarks>
    public void FreeAllocations(int allocatorProviderId)
    {
        GuardConfigured();
        var cont = GetPerProviderContainer(allocatorProviderId);
        if (cont.IsScoped) cont.Context!.Value?.FreeAllocations();
        if (cont.Provider is IAllocatorCacheable p) p.ClearCachedMemory();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GuardConfigured()
    {
        if (_allocatorProviders is null) FinalizeConfiguration();
    }

    public IMemoryAllocator GetAllocator(int allocatorProviderId = (int) AllocatorProviderIds.Default)
    {
        if (allocatorProviderId == (int) AllocatorProviderIds.DefaultUncachedUnscoped)
            return _defaultUncachedUnscopedAllocator.GetAllocator();
        GuardConfigured();
        var cont = GetPerProviderContainer(allocatorProviderId);
        if (cont.IsScoped)
            return GetContextOrThrow(ref cont).GetAllocatorOrThrow();
        else
            return cont.Provider.GetAllocator();
    }

    public IDisposable BeginAllocationScope(int allocatorProviderId = (int) AllocatorProviderIds.Default)
    {
        GuardConfigured();
        var cont = GetPerProviderContainer(allocatorProviderId);
        var context = GetOrAddContext(ref cont);
        context.PushAllocator(
            (cont.Provider ?? throw new InvalidOperationException("Allocator provider is not set."))
            .GetAllocator());
        return context;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void FinalizeConfiguration()
    {
        GuardNotConfigured();
        _allocatorProviders = _tmpAllocatorProviders.ToFrozenDictionary();
        _tmpAllocatorProviders.Clear();
    }

    private void GuardNotConfigured()
    {
        if (_allocatorProviders is not null)
            ThrowHelper.ThrowInvalidOperationException(
                $"Allocation context is already configured, to reconfigure, call {nameof(ClearProvidersAndAllocations)}() first.");
    }

    /// <summary>
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="allocatorProviderId"></param>
    /// <param name="scoped"></param>
    /// <remarks>This method must be called before any other code that uses the allocator context infrastructure. </remarks>
    public void SetAllocatorProvider(IAllocatorProvider provider, int allocatorProviderId, bool scoped)
    {
        GuardNotConfigured();
        Guard.IsNotEqualTo(allocatorProviderId, (int) AllocatorProviderIds.DefaultUncachedUnscoped);
        Guard.IsNotEqualTo(allocatorProviderId, (int) AllocatorProviderIds.Invalid);
        ref var cont = ref _tmpAllocatorProviders.GetValueRefOrAddDefault(allocatorProviderId, out var exists);
        if (exists)
            ThrowHelper.ThrowArgumentException(nameof(allocatorProviderId),
                $"Provider with the same ID {allocatorProviderId} is already set.");
        cont = new PerProviderContainer(this, provider, scoped);
    }

    private void ReturnContext(ContextContainer context)
    {
        context.FreeAllocations();
        lock (ContextPool)
        {
            ContextPool.Push(context);
        }
    }

    private ContextContainer GetContextOrThrow(scoped ref readonly PerProviderContainer cont)
    {
        if (!cont.IsScoped)
            throw new InvalidOperationException("Allocation provider is not scoped.");
        return cont.Context!.Value ??
               throw new InvalidOperationException($"No allocator context is set for current execution context.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ContextContainer GetOrAddContext(scoped ref readonly PerProviderContainer cont)
    {
        if (!cont.IsScoped)
            throw new InvalidOperationException("Allocation provider is not scoped.");
        if (!cont.Context!.Value!.IsInvalid)
            return cont.Context!.Value!;
        return cont.Context!.Value = RentContext() ?? new ContextContainer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref readonly PerProviderContainer GetPerProviderContainer(int providerId)
    {
        ref readonly var ret = ref _allocatorProviders!.GetValueRefOrNullRef(providerId);
        if (Unsafe.IsNullRef(in ret))
            ThrowHelper.ThrowInvalidOperationException($"Provider with ID '{providerId}' is not found.");

        return ref ret;
    }

    private ContextContainer? RentContext()
    {
        lock (ContextPool)
            if (ContextPool.TryPop(out var context))
                return context;
        return null;
    }


    internal class ContextContainer : IDisposable
    {
        internal static readonly ContextContainer Invalid = new() {IsInvalid = true};
        private readonly Stack<IMemoryAllocator> _allocators = new(4);

        public bool IsInvalid { get; private init; }

        void IDisposable.Dispose()
        {
            var allocator = PopAllocator();
            if (allocator is IPoolableAllocator poolableAllocator)
                poolableAllocator.ReturnToPool();
            else
                allocator.TryDispose();
        }

        public void FreeAllocations()
        {
            lock (_allocators)
            {
                foreach (var allocator in _allocators) allocator.TryDispose();

                _allocators.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMemoryAllocator GetAllocatorOrThrow()
        {
            lock (_allocators)
            {
                if (!_allocators.TryPeek(out var allocator))
                    throw new InvalidOperationException("No allocator is set for current allocator context.");

                return allocator;
            }
        }

        public void PushAllocator(IMemoryAllocator allocator)
        {
            lock (_allocators)
            {
                _allocators.Push(allocator);
            }
        }

        public IMemoryAllocator PopAllocator()
        {
            lock (_allocators)
            {
                return _allocators.Pop();
            }
        }
    }
    
    internal class PerProviderContainer
    {
        private readonly DefaultAllocatorContextImpl _parent;
        public readonly AsyncLocal<ContextContainer>? Context;
        public readonly IAllocatorProvider Provider;
        public bool IsScoped => Context != null;

        public PerProviderContainer(DefaultAllocatorContextImpl parent, IAllocatorProvider provider, bool scoped)
        {
            _parent = parent;
            Provider = provider;
            if (scoped)
                Context = new AsyncLocal<ContextContainer>(ValueChangedHandler)
                {
                    // ValueChanged will not fire if no initial value is set.
                    Value = ContextContainer.Invalid
                };
        }

        private void ValueChangedHandler(AsyncLocalValueChangedArgs<ContextContainer> args)
        {
            if ((args.CurrentValue == null || args.CurrentValue.IsInvalid) && args.PreviousValue != null)
                if (!args.PreviousValue.IsInvalid)
                    _parent.ReturnContext(args.PreviousValue);
        }
    }

    public void Dispose()
    {
        ClearProvidersAndAllocations();
    }
}