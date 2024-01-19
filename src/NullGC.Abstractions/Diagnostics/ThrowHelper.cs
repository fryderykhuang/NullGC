using System.Runtime.CompilerServices;

namespace NullGC.Diagnostics;

public static class ThrowHelper
{
    public static void ObjectMustBeDisposed<T>()
    {
        throw new InvalidOperationException($"An object with type {typeof(T)} is not disposed.");
    }

    public static void ThrowNoElementsException()
    {
        throw new InvalidOperationException("Sequence contains no element.");
    }

    public static T ThrowNoElementsException<T>()
    {
        throw new InvalidOperationException("Sequence contains no element.");
    }

    public static void ThrowOverflowException()
    {
        throw new OverflowException();
    }
}