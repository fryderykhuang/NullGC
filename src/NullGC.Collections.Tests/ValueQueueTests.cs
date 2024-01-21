using NullGC.TestCommons;
using Xunit.Abstractions;

namespace NullGC.Collections.Tests;

public class ValueQueueTests :AssertMemoryAllFreedBase
{
    public ValueQueueTests(ITestOutputHelper logger) : base(logger, false)
    {
    }

    [Fact]
    public void EmptyQueueFacts()
    {
        var queue = new ValueQueue<int>();
        Assert.False(queue.IsAllocated);
        Assert.Equal(0, queue.Count);
        Assert.Empty(queue);
        Assert.Throws<InvalidOperationException>(() => queue.Peek());
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        queue.Dispose();
        Assert.True(AllocTracker.ClientIsAllFreed);
        queue.Dispose();
    }
    
    [Fact]
    public void EnqueueDequeueFacts()
    {
        var queue = new ValueQueue<int>();
        queue.Enqueue(1);
        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(0, queue.Count);
        Assert.Empty(queue);
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        queue.Enqueue(1);
        Assert.Single(queue);
        queue.Enqueue(2);
        queue.Enqueue(3);
        queue.Enqueue(4);
        queue.Enqueue(5);
        queue.Enqueue(6);
        Assert.Equal(6, queue.Count);
        Assert.Equal(6, queue.Count());
        Assert.Equal(1, queue.Dequeue());
        queue.Enqueue(7);
        Assert.Equal(2, queue.Dequeue());
        Assert.Equal(3, queue.Dequeue());
        Assert.Equal(4, queue.Dequeue());
        Assert.Equal(5, queue.Dequeue());
        Assert.Equal(6, queue.Dequeue());
        Assert.Equal(7, queue.Dequeue());
        
        queue.Dispose();
        Assert.True(AllocTracker.ClientIsAllFreed);
        queue.Dispose();
    }
}