using NullGC.Diagnostics;
using NullGC.TestCommons;
using Xunit.Abstractions;

namespace NullGC.Collections.Tests;

public class SlidingTimeWindowTests : AssertMemoryAllFreedBase
{
    [Fact]
    public void GeneralFacts()
    {
        var stw = new SlidingTimeWindow<int>(1000, 5, default);

        while (Environment.TickCount64 % 1000 > 10) Thread.Yield();
        var w = stw.Update(1);
        Assert.True(w.Count == 1);
        Assert.True(w.Buckets.Count == 1);
        Assert.True(w.Sum == 1);
        Assert.True(w.Buckets.TailRef.Count == 1);
        Assert.True(w.Buckets.TailRef.Sum == 1);
        w = stw.Update(2);
        Assert.True(w.Count == 2);
        Assert.True(w.Buckets.Count == 1);
        Assert.True(w.Sum == 3);
        Assert.True(w.Buckets.TailRef.Count == 2);
        Assert.True(w.Buckets.TailRef.Sum == 3);
        Thread.Sleep(500); // 0.5
        w = stw.Update(1);
        Assert.True(w.Count == 3);
        Assert.True(w.Buckets.Count == 1);
        Assert.True(w.Sum == 4);
        Assert.True(w.Buckets.TailRef.Count == 3);
        Assert.True(w.Buckets.TailRef.Sum == 4);
        Thread.Sleep(1000); // 1.5
        w = stw.Update(1);
        Assert.True(w.Count == 4);
        Assert.True(w.Buckets.Count == 2);
        Assert.True(w.Sum == 5);
        Assert.True(w.Buckets.GetNthItemRefFromTail(1).Count == 3);
        Assert.True(w.Buckets.GetNthItemRefFromTail(1).Sum == 4);
        Assert.True(w.Buckets.TailRef.Count == 1);
        Assert.True(w.Buckets.TailRef.Sum == 1);
        Thread.Sleep(2000); // 3.5
        w = stw.Update(1);
        Assert.Equal(5, w.Count);
        Assert.Equal(4, w.Buckets.Count);
        Assert.Equal(6, w.Sum);
        Assert.Equal(1, w.Buckets.GetNthItemRefFromTail(2).Count);
        Assert.Equal(1, w.Buckets.GetNthItemRefFromTail(2).Sum);
        Assert.Equal(0, w.Buckets.GetNthItemRefFromTail(1).Count);
        Assert.Equal(0, w.Buckets.GetNthItemRefFromTail(1).Sum);
        Assert.Equal(1, w.Buckets.TailRef.Count);
        Assert.Equal(1, w.Buckets.TailRef.Sum);
        Thread.Sleep(5000); // 8.5
        w = stw.Update(1);
        Assert.Equal(1, w.Count);
        Assert.Equal(5, w.Buckets.Count);
        Assert.Equal(1, w.Sum);
        Assert.Equal(0, w.Buckets.GetNthItemRefFromTail(2).Count);
        Assert.Equal(0, w.Buckets.GetNthItemRefFromTail(2).Sum);
        Assert.Equal(0, w.Buckets.GetNthItemRefFromTail(1).Count);
        Assert.Equal(0, w.Buckets.GetNthItemRefFromTail(1).Sum);
        Assert.Equal(1, w.Buckets.TailRef.Count);
        Assert.Equal(1, w.Buckets.TailRef.Sum);
        
        
        stw.Dispose();
    }
    
    public SlidingTimeWindowTests(ITestOutputHelper logger) : base(logger, false)
    {
    }
}