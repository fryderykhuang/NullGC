using System.Runtime.CompilerServices;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.Collections;
using NullGC.Linq;

namespace NullGC.Analyzer.Tests.Project;

public static class Program
{
    private static ValueList<int> _list;
    private static int _key;
    private static ValueList<int> _list2;

    public static void Main(string[] args)
    {
        AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl());
        AllocatorContext.Impl.ConfigureDefault();
        _list = new ValueList<int>(AllocatorTypes.DefaultUnscoped) {1, 2, 3, 4, 5, 6};
        _list2 = new ValueList<int>(AllocatorTypes.DefaultUnscoped) {1};
        UseValueList(_list); // should borrow

        var stA = new StructA(_list);
        Console.WriteLine(/*ReadOnly*/stA.NoBorrowList); // ok
        Console.WriteLine(stA.NoBorrowList); // should borrow
        Console.WriteLine(stA.BorrowList); // ok
        Console.WriteLine(stA.Borrow()); // ok
        Console.WriteLine(stA.BorrowNotInterface()); // ok
        Console.WriteLine(stA.PartiallyExplicit); // partially explicit
        Console.WriteLine(stA.BorrowNoAttribute()); // should have attribute

        // ref ValueList<int> localList = ref _list2;
        ValueList<int> localList = default;
        stA.RefParamMethod(ref localList);
        if (localList.SequenceEqual(new[] {1})) throw new InvalidOperationException();
        if (!localList.SequenceEqual(new[] {1, 2, 3, 4, 5, 6})) throw new InvalidOperationException();
        if (Unsafe.IsNullRef(ref localList)) throw new InvalidOperationException();
        localList.Dispose();
        stA.Dispose();
        _list.Dispose();
    }

    private static void UseValueList(ValueList<int> lst)
    {
        _key = lst.LinqRef().GroupBy(x => x).First().Key; // GroupBy not implemented
        lst.LinqRef().WorkOnIEnumerable(); // GroupBy not implemented
        
    }
}

internal static class Extensions
{
    public static void WorkOnIEnumerable<T>(this IEnumerable<T> obj)
    {
        
    }
}

struct StructA : IExplicitOwnership<StructA>
{
    private bool flag;
    private ValueList<int> _list;
    public ValueList<int> NoBorrowList => _list;

    public ValueList<int> BorrowList
    {
        get { return _list.Borrow(); }
    }

    public ValueList<int> PartiallyExplicit
    {
        get
        {
            if (flag)
                return _list.Borrow();
            else
                return _list;
        }
    }

    public StructA(ValueList<int> list)
    {
        _list = list;
    }

    // [UnscopedRef]
    // public void RefAssignRefParamMethod(ref ValueList<int> refParam)
    // {
    //     if (Unsafe.IsNullRef(ref _list)) throw new InvalidOperationException();
    //     if (!_list.SequenceEqual(new []{1,2,3,4,5,6})) throw new InvalidOperationException();
    //     refParam = ref _list;
    // }

    public void RefParamMethod(ref ValueList<int> refParam)
    {
        refParam = _list;
    }

    public void Dispose()
    {
        _list.Dispose();
    }

    public StructA Borrow()
    {
        return new StructA(_list.Borrow());
    }

    [return: Borrowed]
    public StructA BorrowNotInterface()
    {
        return new StructA(_list.Borrow());
    }

    public StructA BorrowNoAttribute()
    {
        return new StructA(_list.Borrow());
    }

    [return: Owned]
    public StructA Take()
    {
        return new StructA(_list.Take());
    }
}