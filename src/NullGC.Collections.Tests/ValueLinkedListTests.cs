using System.Runtime.CompilerServices;
using NullGC.Linq;
using NullGC.TestCommons;
using Xunit.Abstractions;

namespace NullGC.Collections.Tests;

public class ValueLinkedListTests : AssertMemoryAllFreedBase
{
    public ValueLinkedListTests(ITestOutputHelper logger) : base(logger, false)
    {
    }

    [Fact]
    public void CanAddRemoveFirst()
    {
        var lst = new ValueLinkedList<int>();
        lst.AddFront(100);
        Assert.Equal(100, lst.HeadRefOrNullRef.Value);
        lst.RemoveFront();
        Assert.True(Unsafe.IsNullRef(ref lst.HeadRefOrNullRef));
        lst.AddFront(200);
        Assert.Equal(200, lst.HeadRefOrNullRef.Value);
        lst.AddFront(300);
        Assert.Equal(300, lst.HeadRefOrNullRef.Value);
        lst.RemoveFront();
        Assert.Equal(200, lst.HeadRefOrNullRef.Value);
        lst.RemoveFront();
        Assert.True(Unsafe.IsNullRef(ref lst.HeadRefOrNullRef));
        AssertEx.Throws<InvalidOperationException, ValueLinkedList<int>>(
            (ref ValueLinkedList<int> a) => a.RemoveFront(), ref lst);
        Assert.True(Unsafe.IsNullRef(ref lst.HeadRefOrNullRef));
        lst.AddFront(400);
        Assert.Equal(400, lst.HeadRefOrNullRef.Value);
        lst.Dispose();
    }

    [Fact]
    public void CanAddRemoveLast()
    {
        var lst = new ValueLinkedList<int>();
        lst.AddBack(100);
        Assert.Equal(100, lst.TailRefOrNullRef.Value);
        lst.RemoveBack();
        Assert.True(Unsafe.IsNullRef(ref lst.TailRefOrNullRef));
        lst.AddBack(200);
        Assert.Equal(200, lst.TailRefOrNullRef.Value);
        lst.AddBack(300);
        Assert.Equal(300, lst.TailRefOrNullRef.Value);
        lst.RemoveBack();
        Assert.Equal(200, lst.TailRefOrNullRef.Value);
        lst.RemoveBack();
        Assert.True(Unsafe.IsNullRef(ref lst.TailRefOrNullRef));
        AssertEx.Throws<InvalidOperationException, ValueLinkedList<int>>(
            (ref ValueLinkedList<int> a) => a.RemoveBack(), ref lst);
        Assert.True(Unsafe.IsNullRef(ref lst.TailRefOrNullRef));
        lst.AddBack(400);
        Assert.Equal(400, lst.TailRefOrNullRef.Value);
        lst.Dispose();
    }

    [Fact]
    public void CanAddRemoveFirstLastMixed()
    {
        var lst = new ValueLinkedList<int>();
        lst.AddFront(100);
        Assert.Equal(100, lst.TailRefOrNullRef.Value);
        lst.RemoveBack();
        Assert.True(Unsafe.IsNullRef(ref lst.TailRefOrNullRef));
        lst.AddFront(200);
        Assert.Equal(200, lst.TailRefOrNullRef.Value);
        lst.AddFront(300);
        Assert.Equal(300, lst.HeadRefOrNullRef.Value);
        Assert.Equal(200, lst.TailRefOrNullRef.Value);
        lst.RemoveBack();
        Assert.Equal(300, lst.TailRefOrNullRef.Value);
        lst.RemoveFront();
        Assert.True(Unsafe.IsNullRef(ref lst.HeadRefOrNullRef));
        Assert.True(Unsafe.IsNullRef(ref lst.TailRefOrNullRef));
        lst.AddBack(1);
        lst.AddFront(2);
        lst.AddBack(3);
        Assert.Equal(new[]{2, 1, 3}, (lst.LinqRef().Select(x => x.Value)));
        lst.RemoveFront();
        Assert.Equal(new[]{1, 3}, (lst.LinqRef().Select(x => x.Value)));
        lst.RemoveFront();
        lst.RemoveFront();
        Assert.True(Unsafe.IsNullRef(ref lst.HeadRefOrNullRef));
        Assert.True(Unsafe.IsNullRef(ref lst.TailRefOrNullRef));
        lst.Dispose();
    }

    [Fact]
    public void GetNthNode()
    {
        var lst = new ValueLinkedList<int>();
        lst.AddFront(1);
        lst.AddFront(2);
        lst.AddFront(3);
        lst.AddFront(4);
        lst.AddFront(5);
        Assert.Equal(2, lst.GetNthNodeRefFromHead(3).Value);
        Assert.Equal(1, lst.GetNthNodeRefFromHead(4).Value);
        AssertEx.Throws<ArgumentOutOfRangeException, ValueLinkedList<int>>((ref ValueLinkedList<int> x) =>
            x.GetNthNodeRefFromHead(5), ref lst);
        AssertEx.Throws<ArgumentOutOfRangeException, ValueLinkedList<int>>((ref ValueLinkedList<int> x) =>
            x.GetNthNodeRefFromHead(-1), ref lst);
        Assert.Equal(5, lst.GetNthNodeRefFromHead(0).Value);
        Assert.Equal(4, lst.GetNthNodeRefFromHead(1).Value);
        
        Assert.Equal(4, lst.GetNthNodeRefFromTail(3).Value);
        Assert.Equal(5, lst.GetNthNodeRefFromTail(4).Value);
        AssertEx.Throws<ArgumentOutOfRangeException, ValueLinkedList<int>>((ref ValueLinkedList<int> x) =>
            x.GetNthNodeRefFromTail(5), ref lst);
        AssertEx.Throws<ArgumentOutOfRangeException, ValueLinkedList<int>>((ref ValueLinkedList<int> x) =>
            x.GetNthNodeRefFromTail(-1), ref lst);
        Assert.Equal(1, lst.GetNthNodeRefFromTail(0).Value);
        Assert.Equal(2, lst.GetNthNodeRefFromTail(1).Value);
        lst.Dispose();
    }

    [Fact]
    public void CanRemoveAny()
    {
        var lst = new ValueLinkedList<int>();
        lst.AddFront(1);
        lst.AddFront(2);
        lst.AddFront(3);
        lst.AddFront(4);
        lst.AddFront(5);
        Assert.Equal(new[] {5, 4, 3, 2, 1}, (lst.LinqRef().Select(x => x.Value)));

        // remove 2
        lst.Remove(lst.GetNthNodeRefFromHead(3).Index);
        Assert.Equal(new[] {5, 4, 3, 1}, (lst.LinqRef().Select(x => x.Value)));
        // remove 4
        lst.Remove(lst.GetNthNodeRefFromHead(1).Index);
        Assert.Equal(new[] {5, 3, 1}, (lst.LinqRef().Select(x => x.Value)));
        lst.Remove(lst.GetNthNodeRefFromHead(0).Index);
        Assert.Equal(new[] {3, 1}, (lst.LinqRef().Select(x => x.Value)));
        lst.Remove(lst.GetNthNodeRefFromHead(1).Index);
        Assert.Equal(new[] {3}, (lst.LinqRef().Select(x => x.Value)));
        lst.Remove(lst.GetNthNodeRefFromHead(0).Index);
        Assert.Equal(Enumerable.Empty<int>(), (lst.LinqRef().Select(x => x.Value)));
        lst.Dispose();
    }

    [Fact]
    public void CanMove()
    {
        var lst = new ValueLinkedList<int>();
        lst.AddFront(1);
        lst.AddFront(2);
        lst.AddFront(3);
        lst.AddFront(4);
        lst.AddFront(5);
        Assert.Equal(new[] {5, 4, 3, 2, 1}, (lst.LinqRef().Select(x => x.Value)));

        lst.Move(lst.TailRefOrNullRef.Index, lst.HeadRefOrNullRef.Index);
        Assert.Equal(new[] {1, 5, 4, 3, 2}, (lst.LinqRef().Select(x => x.Value)));
        lst.Move(lst.HeadRefOrNullRef.Index, ValueLinkedList.LastPosition);
        Assert.Equal(new[] {5, 4, 3, 2, 1}, (lst.LinqRef().Select(x => x.Value)));

        lst.Move(lst.HeadRefOrNullRef.Next, lst.TailRefOrNullRef.Previous);
        Assert.Equal(new[] {5, 3, 4, 2, 1}, (lst.LinqRef().Select(x => x.Value)));
        
        lst.Move(lst.TailRefOrNullRef.Previous, ValueLinkedList.FirstPosition);
        Assert.Equal(new[] {2, 5, 3, 4, 1}, (lst.LinqRef().Select(x => x.Value)));
        
        lst.Dispose();
    }

    private struct StructForUsing : IDisposable
    {
        public int SomeInt;

        public void Dispose()
        {
            if (SomeInt != 100)
            {
                throw new InvalidOperationException("Copied.");
            }

            SomeInt = -1;
        }
    }

    private static void MutateStruct(ref StructForUsing s)
    {
        s.SomeInt = 100;
    }

    [Fact]
    public void UsingMadeACopy()
    {
        bool copied = false;
        var s = new StructForUsing();
        try
        {
            using (s)
            {
                MutateStruct(ref s);
                Assert.Equal(100, s.SomeInt);
            }
        }
        catch (InvalidOperationException ex) when (ex.Message == "Copied.")
        {
            copied = true;
        }
        catch (Exception)
        {
            // ignored.
        }

        Assert.Equal(100, s.SomeInt);
        Assert.True(copied);
    }

    private static void MutateStructIn(in StructForUsing s)
    {
        Unsafe.AsRef(in s).SomeInt = 100;
    }

    [Fact]
    public void UsingMadeACopyAnyway()
    {
        bool copied = false;
        var s = new StructForUsing();
        try
        {
            using (s)
            {
                MutateStructIn(in s);
                Assert.Equal(100, s.SomeInt);
            }
        }
        catch (InvalidOperationException ex) when (ex.Message == "Copied.")
        {
            copied = true;
        }
        catch (Exception)
        {
            // ignored.
        }

        Assert.Equal(100, s.SomeInt);
        Assert.True(copied);
    }

    [Fact]
    public void UsingVarNotMakeACopy()
    {
        bool copied = false;
        try
        {
            using var s = new StructForUsing();
            MutateStructIn(in s);
            Assert.Equal(100, s.SomeInt);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Copied.")
        {
            copied = true;
        }
        catch (Exception)
        {
            // ignored.
        }

        Assert.False(copied);
    }

    [Fact]
    public void UsingVarNotMakeACopy2()
    {
        bool copied = false;
        try
        {
            using (var s = new StructForUsing())
            {
                MutateStructIn(in s);
                Assert.Equal(100, s.SomeInt);
            }
        }
        catch (InvalidOperationException ex) when (ex.Message == "Copied.")
        {
            copied = true;
        }
        catch (Exception)
        {
            // ignored.
        }

        Assert.False(copied);
    }
}