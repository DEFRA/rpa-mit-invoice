using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Invoices.Api.Endpoints;
using Invoices.Api.Models;
using Invoices.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Invoices.Api.Test;

public class InvoiceBulkPostEndpointsTest
{
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();
    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();
    private readonly IValidator<BulkInvoices> _validator = new BulkInvoiceValidator();
    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnOk()
    {
        // Arrange
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Reference = "Reference1",
            Invoices = new List<Invoice>
            {
                invoiceTestData
            }
        };

        _cosmosService.CreateBulk(bulkInvoices).Returns(bulkInvoices);

        // Act
        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(200);
    }


    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnBadRequest_ValidationFailed()
    {
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Invoices = new List<Invoice>
            {
                invoiceTestData
            }
        };

        _cosmosService.CreateBulk(bulkInvoices).ReturnsNull();

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey("Reference");
    }

    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnBadRequest()
    {
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Reference = "Reference1",
            Invoices = new List<Invoice>
            {
                invoiceTestData
            }
        };

        _cosmosService.CreateBulk(bulkInvoices).ReturnsNull();
        _eventQueueService.CreateMessage(bulkInvoices.Reference, "failed", "bulk-invoice-creation-falied", "Bulk invoice creation failed").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }
}