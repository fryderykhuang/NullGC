using BenchmarkDotNet.Attributes;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.Collections;

namespace NullGC.DragRace.Benchmarks;


[PickedForCicd]
public class Allocator_CachedVsNativeAllocatorBenchmarks : BenchmarkBase
{
    public Allocator_CachedVsNativeAllocatorBenchmarks()
    {
        _lst = new UIntPtr[_maxConsecutiveAllocs];
    }

    private readonly int _count = 500;
    private readonly int _maxConsecutiveAllocs = 100;
    private readonly UIntPtr[] _lst;

    [GlobalSetup]
    public void Setup()
    {
        AllocatorContext.Impl.ConfigureDefaultUnscoped();
    }
    
    [Benchmark]
    public void Cached()
    {
        var rand = new Random(0);
        for (int i = 0; i < _count; i++)
        {
            var c = rand.Next(1, _maxConsecutiveAllocs);
            for (int j = 0; j < c; ++j)
            {
                _lst[j] = AllocatorContext.GetAllocator().Allocate((UIntPtr) rand.Next(0, 1 * 1024 * 1024));
            }
            for (int j = 0; j < c; ++j)
            {
                AllocatorContext.GetAllocator().Free(_lst[j]);
            }
        }
    }

    [Benchmark]
    public void Native()
    {
        var rand = new Random(0);
        for (int i = 0; i < _count; i++)
        {
            var c = rand.Next(1, _maxConsecutiveAllocs);
            for (int j = 0; j < c; ++j)
            {
                _lst[j] = AllocatorContext.GetAllocator((int)AllocatorTypes.DefaultUncachedUnscoped).Allocate((UIntPtr) rand.Next(0, 1 * 1024 * 1024));
            }
            for (int j = 0; j < c; ++j)
            {
                AllocatorContext.GetAllocator((int)AllocatorTypes.DefaultUncachedUnscoped).Free(_lst[j]);
            }
        }
    }
    
    [GlobalCleanup]
    public void Cleanup()
    {
        AllocatorContext.ClearProvidersAndAllocations();
    }
}