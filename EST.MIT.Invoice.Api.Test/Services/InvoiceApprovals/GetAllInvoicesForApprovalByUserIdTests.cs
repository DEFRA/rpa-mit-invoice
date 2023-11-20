using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService;
using EST.MIT.Invoice.Api.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EST.MIT.Invoice.Api.Test.Services.InvoiceApprovals;
public class GetAllInvoicesForApprovalByUserIdTests
{
	private PaymentRequestsBatchApprovalService _serviceToTest;

	public GetAllInvoicesForApprovalByUserIdTests()
	{
	}
	
}
