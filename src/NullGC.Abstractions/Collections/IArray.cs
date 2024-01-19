namespace NullGC.Collections;

public interface IArray<out T>
{
    T[] Items { get; }
    int Length { get; }
    bool IsInitialized { get; }
}