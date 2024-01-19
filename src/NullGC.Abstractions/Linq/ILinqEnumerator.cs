namespace NullGC.Linq;

// ReSharper disable once UnusedTypeParameter
// ReSharper disable once TypeParameterCanBeVariant
public interface ILinqEnumerator<T> : IEnumerator<T>, IMaybeCountable, ISkipTakeAware;