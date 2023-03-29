using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Invoices.Api.Models;
using Invoices.Api.Services;
using Invoices.Api.Services.Models;
using Invoices.Api.Util;
using Microsoft.Azure.Cosmos;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Invoices.Api.Test;

public class CosmosServiceTests
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;
    private readonly CosmosService _cosmosService;
    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    public CosmosServiceTests()
    {
        _mockCosmosClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();

        _mockCosmosClient.Setup(client => client.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_mockContainer.Object);

        _cosmosService = new CosmosService(_mockCosmosClient.Object, "TestDatabase", "TestContainer");
    }

    [Fact]
    public async Task Get_ReturnsInvoices()
    {
        const string sqlCosmosQuery = "SELECT * FROM c";
        var data = JsonConvert.SerializeObject(invoiceTestData);
        var invoiceEntities = new List<InvoiceEntity>
        {
            new InvoiceEntity { Id = "1", SchemeType = "Scheme1", Value = 100, Status = "awaiting", Data = data },
            new InvoiceEntity { Id = "2", SchemeType = "Scheme1", Value = 200, Status = "awaiting", Data = data },
        };

        var feedResponseMock = new Mock<FeedResponse<InvoiceEntity>>();
        feedResponseMock.Setup(x => x.GetEnumerator()).Returns(invoiceEntities.GetEnumerator());

        var mockFeedIterator = new Mock<FeedIterator<InvoiceEntity>>();
        mockFeedIterator.Setup(f => f.HasMoreResults).Returns(true);
        mockFeedIterator.Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedResponseMock.Object)
            .Callback(() => mockFeedIterator
                .Setup(f => f.HasMoreResults)
                .Returns(false));

        _mockContainer.Setup(c => c.GetItemQueryIterator<InvoiceEntity>(It.IsAny<QueryDefinition>(), null, null))
            .Returns(mockFeedIterator.Object);

        var result = await _cosmosService.Get(sqlCosmosQuery);
        result.Should().BeEquivalentTo(InvoiceMapper.MapToInvoice(invoiceEntities));
    }

    private static ItemResponse<T> CreateMockResponse<T>(T item)
    {
        var mockResponse = new Mock<ItemResponse<T>>();
        mockResponse.SetupGet(r => r.Resource).Returns(item);
        return mockResponse.Object;
    }

    [Fact]
    public async Task Create_AddsInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockContainer.Setup(c => c.CreateItemAsync<InvoiceEntity>(It.IsAny<InvoiceEntity>(), It.IsAny<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(CreateMockResponse(invoiceEntity)));

        var result = await _cosmosService.Create(invoice);

        result.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task Update_UpdatesInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockContainer.Setup(c => c.CreateItemAsync<InvoiceEntity>(It.IsAny<InvoiceEntity>(), It.IsAny<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(CreateMockResponse(invoiceEntity)));

        var result = await _cosmosService.Update(invoice);

        result.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task Delete_DeletesInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockContainer.Setup(c => c.CreateItemAsync<InvoiceEntity>(It.IsAny<InvoiceEntity>(), It.IsAny<PartitionKey>(), null, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(CreateMockResponse(invoiceEntity)));

        var result = await _cosmosService.Delete(invoice.Id, invoice.SchemeType);

        result.Should().BeEquivalentTo(invoice.Id);
    }
}
