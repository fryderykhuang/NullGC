namespace NullGC;

/// <summary>
/// <see cref="System.Nullable{T}"/> does not have a ref return Value property(only ref readonly through <see cref="Nullable"/>), so here it is. 
/// </summary>
/// <typeparam name="T"></typeparam>
public struct RawNullable<T> where T : struct
{
    public readonly bool HasValue;
    public T Value;

    public RawNullable()
    {
        HasValue = false;
    }

    public RawNullable(T value)
    {
        Value = value;
        HasValue = true;
    }

    public override bool Equals(object? other)
    {
        if (!HasValue) return other == null;
        if (other == null) return false;
        return Value.Equals(other);
    }

    public override int GetHashCode() => HasValue ? Value.GetHashCode() : 0;

    public override string? ToString() => HasValue ? Value.ToString() : "";

    public static implicit operator RawNullable<T>(T value) => new(value);

    public static explicit operator T(RawNullable<T> value) => value!.Value;
}