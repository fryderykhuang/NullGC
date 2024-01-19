using System.Runtime.CompilerServices;

namespace NullGC.Linq;

internal static class ThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SequenceContainsNoElement()
    {
        throw new InvalidOperationException("Sequence contains no element.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void PreviousEnumeratorTypeNotSupported(string paramName)
    {
        throw new ArgumentException("Given previous enumerator type not supported.", paramName);
    }

    public static void DelegateTypeNotSupported(string paramName)
    {
        throw new ArgumentException("Given delegate type not supported.", paramName);
    }
}