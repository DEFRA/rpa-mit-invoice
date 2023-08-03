using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using EST.MIT.Invoice.Api.Services.Api;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Moq.Protected;
using System.Text;
using System.Data;
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
    public async Task DeserializeList_ShouldReturnEmptyListWhenContentIsEmpty()
    {
        // Arrange
        var content = new StringContent("[]", Encoding.UTF8, "application/json");

        // Act
        var result = await _deserializer.DeserializeList<string>(content);

        // Assert
        Assert.Empty(result);
    }
}
