using NullGC.Allocators;

namespace NullGC.TestCommons;

public class DefaultAllocatorContextFixture : IDisposable
{
    public DefaultAllocatorContextFixture()
    {
        AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl());
    }
    
    public void Dispose()
    {
        AllocatorContext.ResetImplementation(null);
    }
}