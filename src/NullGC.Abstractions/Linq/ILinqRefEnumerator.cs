namespace NullGC.Linq;

public interface ILinqRefEnumerator<T> : ILinqEnumerator<T>
{
    new ref T Current { get; }
    unsafe T* CurrentPtr { get; }
}