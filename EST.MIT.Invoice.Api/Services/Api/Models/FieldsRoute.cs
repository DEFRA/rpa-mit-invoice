﻿using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services.Api.Models;

[ExcludeFromCodeCoverage]
public class FieldsRoute
{
    public string? AccountType { get; set; }
    public string? Organisation { get; set; }
    public string? PaymentType { get; set; }
    public string? SchemeType { get; set; }
}