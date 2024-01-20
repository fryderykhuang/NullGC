using BenchmarkDotNet.Attributes;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;

namespace NullGC.DragRace;

public abstract class BenchmarkBase : IDisposable
{
    protected BenchmarkBase()
    {
        AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl());
    }
    
    // [GlobalSetup]
    // public void Setup()
    // {
    //     AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl());
    // }
    //
    // [GlobalCleanup]
    // public void Cleanup()
    // {
    //     AllocatorContext.ResetImplementation(null);
    // }
    public void Dispose()
    {
        AllocatorContext.ResetImplementation(null);
    }
}