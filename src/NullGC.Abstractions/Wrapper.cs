namespace NullGC;

/// <summary>
/// Wrapper object to avoid CS0695 (https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0695)
/// </summary>
/// <typeparam name="T"></typeparam>
public struct Wrapper<T>
{
    public T Value;

    public Wrapper(T value)
    {
        Value = value;
    }

    public Wrapper()
    {
        Value = default!;
    }

    public static implicit operator Wrapper<T>(T value) => new(value);
    public static implicit operator T(Wrapper<T> wrapper) => wrapper.Value;
}
