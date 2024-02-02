namespace NullGC;

[AttributeUsage(AttributeTargets.ReturnValue)]
public class BorrowedAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.ReturnValue)]
public class OwnedAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Parameter)]
public class ReadOnlyAttribute : Attribute
{
    
}