using EST.MIT.Invoice.Api.Services.Api;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace EST.MIT.Invoice.Api.Test.Services.Api;
public class CacheServiceTests
{
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _cacheService = new CacheService(_memoryCacheMock.Object);
    }

    [Fact]
    public void GetData_WhenCalled_ReturnsCachedData()
    {
        // Arrange
        var key = "TestKey";
        var expectedValue = "TestData";

        object outValue = expectedValue;
        _memoryCacheMock.Setup(mc => mc.TryGetValue(key, out outValue)).Returns(true);

        // Act
        var result = _cacheService.GetData<string>(key);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void SetData_WhenCalled_SetsValueInMemoryCache()
    {
        // Arrange
        var key = "TestKey";
        var value = "TestValue";

        _memoryCacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny)).Returns(false);
        _memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

        // Act
        _cacheService.SetData(key, value);

        // Assert
        _memoryCacheMock.Verify(mc => mc.CreateEntry(key), Times.Once);
    }

    [Fact]
    public void RemoveData_WhenCalled_RemovesDataFromMemoryCache()
    {
        // Arrange
        var key = "TestKey";

        _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(new Mock<ICacheEntry>().Object);

        // Act
        _cacheService.RemoveData(key);

        // Assert
        _memoryCacheMock.Verify(mc => mc.Remove(key), Times.Once);
    }

}

