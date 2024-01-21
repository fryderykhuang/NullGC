using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.TestCommons;
using Xunit.Abstractions;

namespace NullGC.Collections.Tests;

public class ValueArrayTests : AssertMemoryAllFreedBase
{
    public ValueArrayTests(ITestOutputHelper logger) : base(logger, false)
    {
    }

    [Fact]
    public void DefaultArrayFacts()
    {
        ValueArray<int> defaultArr = default;
        Assert.False(defaultArr.IsAllocated);
        Assert.True(EqualityComparer<ValueArray<int>>.Default.Equals(ValueArray<int>.Empty, defaultArr));
        Assert.Empty(defaultArr);
        foreach (ref var item in defaultArr) Assert.Fail("Should be empty.");
        defaultArr.Dispose();
    }

    [Fact]
    public void SpecificLengthArrayCanBeConstructed()
    {
        var arr = new ValueArray<int>(0);
        arr.Dispose();
        arr = new ValueArray<int>(1);
        arr.Dispose();
        arr = new ValueArray<int>(2);
        arr.Dispose();
        arr = new ValueArray<int>(200);
        arr.Dispose();
        arr = new ValueArray<int>(20000);
        arr.Dispose();
        arr = new ValueArray<int>(500000);
        arr.Dispose();
        arr = new ValueArray<int>(50000000);
        arr.Dispose();
    }

    // TODO hard to mock
    // [Fact]
    // public void ValueArrayMaxLengthFacts()
    // {
    // }

    [Fact]
    public void ValueSetShouldBePreserved()
    {
        var count = 10000000;
        var arr = new ValueArray<int>(count);
        for (var i = 0; i < count; i++) arr[i] = i;

        for (var i = 0; i < count; i++) Assert.Equal(arr[i], i);

        arr.Dispose();
    }
}