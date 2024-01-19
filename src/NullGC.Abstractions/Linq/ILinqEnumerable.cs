namespace NullGC.Linq;

public interface ILinqEnumerable<out T, out TEnumerator> : IEnumerable<T>, IMaybeCountable where TEnumerator : ILinqEnumerator<T>
{
    new TEnumerator GetEnumerator();
}
