using NullGC.TestCommons;

namespace NullGC.Collections.Tests;

public class FixedCountValueDequeTTests : AssertMemoryAllFreedBase
{
    [Fact]
    public void ZeroCapacityQueue()
    {
        var q = new FixedCountValueDeque<int>();
        Assert.Equal(0, q.Capacity);
        Assert.Empty(q);
        Assert.True(q.IsEmpty);
        Assert.True(q.IsFull);
    }

    [Fact]
    public void EmptyQueue()
    {
        var q = new FixedCountValueDeque<int>(5);
        Assert.Empty(q);
        Assert.True(q.IsEmpty);
        Assert.False(q.IsFull);
        AssertEx.Throws<InvalidOperationException, FixedCountValueDeque<int>>((ref FixedCountValueDeque<int> x) =>
        {
            var _ = x.HeadRef;
        }, ref q);
        AssertEx.Throws<InvalidOperationException, FixedCountValueDeque<int>>((ref FixedCountValueDeque<int> x) =>
        {
            var _ = x.TailRef;
        }, ref q);
        AssertEx.Throws<ArgumentOutOfRangeException, FixedCountValueDeque<int>>((ref FixedCountValueDeque<int> x) =>
        {
            var _ = x.GetNthItemRefFromHead(0);
        }, ref q);
        AssertEx.Throws<ArgumentOutOfRangeException, FixedCountValueDeque<int>>((ref FixedCountValueDeque<int> x) =>
        {
            var _ = x.GetNthItemRefFromTail(0);
        }, ref q);
        AssertEx.Throws<InvalidOperationException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.RemoveFront(out _); }, ref q);
        AssertEx.Throws<InvalidOperationException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.RemoveBack(out _); }, ref q);
        q.Dispose();
    }

    [Fact]
    public void ThrowOnAddWhenFull()
    {
        var q = new FixedCountValueDeque<int>(1);
        q.AddBack(1);
        AssertEx.Throws<InvalidOperationException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.AddBack(1); }, ref q);
        AssertEx.Throws<InvalidOperationException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.AddFront(1); }, ref q);

        q.Dispose();
    }

    [Fact]
    public void CanAddRemovePushFrontBack()
    {
        var q = new FixedCountValueDeque<int>(5);
        q.AddBack(1);
        q.AddBack(2);
        Assert.Equal(new[] {1, 2}, q);
        q.AddBack(3);
        q.AddBack(4);
        q.AddBack(5);
        Assert.Equal(new[] {1, 2, 3, 4, 5}, q);
        q.RemoveBack(out var e);
        Assert.Equal(5, e);
        Assert.Equal(new[] {1, 2, 3, 4}, q);
        q.AddBack(6);
        Assert.Equal(new[] {1, 2, 3, 4, 6}, q);
        q.RemoveFront(out e);
        Assert.Equal(1, e);
        Assert.Equal(new[] {2, 3, 4, 6}, q);
        q.AddFront(7);
        Assert.Equal(new[] {7, 2, 3, 4, 6}, q);
        q.PushBack(1, out e);
        Assert.Equal(new[] {2, 3, 4, 6, 1}, q);
        Assert.Equal(7, e);
        q.PushFront(8, out e);
        Assert.Equal(new[] {8, 2, 3, 4, 6}, q);
        Assert.Equal(1, e);
        q.PushBack(1, out e);
        Assert.Equal(new[] {2, 3, 4, 6, 1}, q);
        q.Clear();
        Assert.Empty(q);
        q.PushBack(1, out e);
        q.PushBack(1, out e);
        Assert.Equal(new[] {1, 1}, q);

        q.Dispose();
    }

    [Fact]
    public void GetNthFromHeadOrTail()
    {
        var q = new FixedCountValueDeque<int>(5);
        q.AddBack(1);
        Assert.Equal(1, q.GetNthItemRefFromHead(0));
        Assert.Equal(1, q.GetNthItemRefFromTail(0));
        q.AddBack(2);
        q.AddBack(3);
        q.AddBack(4);
        q.AddBack(5);
        Assert.Equal(1, q.GetNthItemRefFromHead(0));
        Assert.Equal(5, q.GetNthItemRefFromTail(0));
        AssertEx.Throws<ArgumentOutOfRangeException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.GetNthItemRefFromHead(-1); }, ref q);
        AssertEx.Throws<ArgumentOutOfRangeException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.GetNthItemRefFromTail(-1); }, ref q);
        q.RemoveFront(out _);
        AssertEx.Throws<ArgumentOutOfRangeException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.GetNthItemRefFromHead(4); }, ref q);
        AssertEx.Throws<ArgumentOutOfRangeException, FixedCountValueDeque<int>>(
            (ref FixedCountValueDeque<int> x) => { x.GetNthItemRefFromTail(4); }, ref q);
        q.AddBack(6);

        Assert.Equal(new[] {2, 3, 4, 5, 6}, q);
        Assert.Equal(6, q.GetNthItemRefFromHead(4));
        Assert.Equal(2, q.GetNthItemRefFromTail(4));

        q.Dispose();
    }

    public FixedCountValueDequeTTests() : base(false)
    {
    }
}