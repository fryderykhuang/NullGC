namespace NullGC;

/// <summary>
/// A marker interface denotes that the implemented type contains only objects that have a fixed memory address. 
/// </summary>
/// <remarks>
/// Currently used in Linq implementation to enable faster algorithms on native-heap allocated objects. 
/// </remarks>
public interface IItemAddressFixed
{
}