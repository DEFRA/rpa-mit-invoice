using System.Text.Json;
using System.Text;
using EST.MIT.Invoice.Api.Util;

namespace EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService;
public class HttpContentDeserializerTests
{
    private readonly HttpContentDeserializer _deserializer;

    public HttpContentDeserializerTests()
    {
        _deserializer = new HttpContentDeserializer();
    }

    [Fact]
    public async Task DeserializeList_ShouldReturnDeserializedList()
    {
        // Arrange
        var list = new List<string> { "item1", "item2" };
        var json = JsonSerializer.Serialize(list);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var result = await _deserializer.DeserializeList<string>(content);

        // Assert
        Assert.Equal(list, result);
    }

    [Fact]
    public async Task DeserializeList_ShouldThrowExceptionWhenContentIsInvalidJson()
    {
        // Arrange
        var content = new StringContent("invalid json", Encoding.UTF8, "application/json");

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(async () => await _deserializer.DeserializeList<string>(content));
    }

    [Fact]
    public async Task DeserializeList_ShouldReturnEmptyListWhenContentIsEmptyArray()
    {
        // Arrange
        var content = new StringContent("[]", Encoding.UTF8, "application/json");

        // Act
        var result = await _deserializer.DeserializeList<string>(content);

        // Assert
        Assert.Empty(result);
    }
}

