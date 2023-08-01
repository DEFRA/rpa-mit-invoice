﻿
using EST.MIT.Invoice.Api.Services.API.Models;

namespace EST.MIT.Invoice.Api.Services.API.Interfaces;


public interface IReferenceDataApi
{
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync(string? invoiceType, string? organisation);
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string? invoiceType);
}
