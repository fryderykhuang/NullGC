using CommunityToolkit.Diagnostics;
using Xunit.Sdk;

namespace NullGC.TestCommons;

public static class AssertEx
{
    private static Exception Throws(Type exceptionType, Exception? exception)
    {
        Guard.IsNotNull(exceptionType);

        if (exception == null)
            throw new ThrowsException(exceptionType);

        if (exceptionType != exception.GetType())
            throw new ThrowsException(exceptionType, exception);

        return exception;
    }
    
    private static Exception? RecordException<TArg>(ActionT1Ref<TArg> testCode, ref TArg arg)
    {
        Guard.IsNotNull(testCode);

        try
        {
            testCode(ref arg);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    
    public static T Throws<T, TArg>(ActionT1Ref<TArg> testCode, ref TArg arg)
        where T : Exception
    {
        return (T)Throws(typeof(T), RecordException(testCode, ref arg));
    }
}