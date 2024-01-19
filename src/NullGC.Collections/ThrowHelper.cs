using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ArgumentException = System.ArgumentException;

namespace NullGC.Collections;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName, string message)
    {
        throw new ArgumentOutOfRangeException(paramName, message);
    }
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName)
    {
        throw new ArgumentOutOfRangeException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentNullException(string paramName)
    {
        throw new ArgumentNullException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRange_IndexMustBeLessException()
    {
        // ReSharper disable once NotResolvedInText
        throw new ArgumentOutOfRangeException("index", "IndexMustBeLessException");
    }

    [DoesNotReturn]
    public static void ThrowWrongValueTypeArgumentException(object? value, Type type)
    {
        throw new ArgumentException($"WrongValueType: {value} {type}");
    }

    [DoesNotReturn]
    public static void ThrowArgumentException(string message)
    {
        throw new ArgumentException(message);
    }

    internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, string argName)
    {
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
        if (value == null && !(default(T) == null))
            ThrowHelper.ThrowArgumentNullException(argName);
    }

    [DoesNotReturn]
    public static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException()
    {
        throw new IndexOutOfRangeException("NeedNonNegNumException");
    }

    [DoesNotReturn]
    public static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual()
    {
        // ReSharper disable once NotResolvedInText
        throw new ArgumentOutOfRangeException("ArgumentOutOfRange_IndexMustBeLessOrEqual");
    }

    [DoesNotReturn]
    public static void ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
    {
        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
    }

    [DoesNotReturn]
    public static void ThrowArgumentException_Argument_IncompatibleArrayType()
    {
        throw new ArgumentException("Argument_IncompatibleArrayType");
    }
    
    [DoesNotReturn]
    public static void ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count()
    {
        // ReSharper disable once NotResolvedInText
        throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Count");
    }

    [DoesNotReturn]
    public static void ThrowKeyNotFoundException<TKey>(TKey key) where TKey : notnull
    {
        throw new KeyNotFoundException($"{key} not found.");
    }

    [DoesNotReturn]
    public static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported()
    {
        throw new InvalidOperationException("ConcurrentOperationsNotSupported");
    }

    [DoesNotReturn]
    public static void ThrowAddingDuplicateWithKeyArgumentException<TKey>(TKey key) where TKey : notnull
    {
        throw new ArgumentException($"AddingDuplicateWithKey {key}");
    }

    [DoesNotReturn]
    public static void ThrowNoMoreElements()
    {
        throw new InvalidOperationException("No more elements.");
    }

    // public static void ThrowBoxingNotAllowed()
    // {
    //     if (GlobalSettings.AllowBoxing)
    //         return;
    //     throw new NotSupportedException(
    //         $"Boxing is not allowed by default, to allow boxing, set {nameof(GlobalSettings)}.{nameof(GlobalSettings.AllowBoxing)} to true.");
    // }

    [DoesNotReturn]
    public static void CollectionIsFull()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Collection is full.");
    }
    
    [DoesNotReturn]
    public static void CollectionIsEmpty()
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Collection is empty.");
    }
}