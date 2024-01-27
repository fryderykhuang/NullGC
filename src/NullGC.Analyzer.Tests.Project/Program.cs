using NullGC.Collections;
using NullGC.Linq;

public static class Program
{
    private static ValueList<int> _list;
    private static int _key;

    public static void Main(string[] args)
    {
        _list = new ValueList<int>() { 1, 2, 3, 4, 5, 6 };
        UseValueList(_list);
    }

    private static void UseValueList(ValueList<int> lst)
    {
        _key = lst.LinqRef().GroupBy(x => x).First().Key;
    }
}