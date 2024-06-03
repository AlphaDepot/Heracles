using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public class MockMemoryCache
{
    public static Mock<IMemoryCache> Get()
    {
        var mockMemoryCache = new Mock<IMemoryCache>();
        mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());
        return mockMemoryCache;
    }
    
}