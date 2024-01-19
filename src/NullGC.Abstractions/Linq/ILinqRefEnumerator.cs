namespace NullGC.Linq;

public interface ILinqRefEnumerator<T> : ILinqEnumerator<T>
{
    new ref T Current { get; }
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    unsafe T* CurrentPtr { get; }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
}