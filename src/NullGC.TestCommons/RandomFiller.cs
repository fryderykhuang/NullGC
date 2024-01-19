using System.Numerics;

namespace NullGC.TestCommons;

public static class RandomFiller
{
    public static void FillArrayRandom<TNum, TCollection>(TCollection arr, int seed = 0) where TCollection : IList<TNum> where TNum : IBinaryInteger<TNum>
    {
        var rand = new Random(seed);
        for (var i = 0; i < arr.Count; i++)
        {
            arr[i] = TNum.CreateTruncating(rand.Next());
        }
    }
    public static void FillArrayRandom<T, TNum>(Span<T> arr, FuncT1TRRef<T, TNum> selector, int seed = 0) where TNum : IBinaryInteger<TNum>
        where T : unmanaged
    {
        var rand = new Random(seed);
        for (var i = 0; i < arr.Length; i++)
        {
            selector(ref arr[i]) = TNum.CreateTruncating(rand.Next());
        }
    }
}