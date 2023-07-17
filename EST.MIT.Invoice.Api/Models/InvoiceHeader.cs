using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Api.Models;

public class InvoiceHeader
{
    [JsonProperty("paymentRequestId")]
    public string PaymentRequestId { get; init; } = default!;
    [JsonProperty("frn")]
    [Range(1000000000, 9999999999, ErrorMessage = "FRN must be between 1000000000 and 9999999999 ")]
    public int FRN { get; init; }
    [JsonProperty("sourceSystem")]
    public string SourceSystem { get; init; } = default!;
    [JsonProperty("marketingYear")]
    [Range(2021, 2099, ErrorMessage = "FRN must be between 2021 and 2099 ")]
    public int MarketingYear { get; init; }
    [JsonProperty("deliveryBody")]
    public string DeliveryBody { get; init; } = default!;
    [JsonProperty("paymentRequestNumber")]
    public int PaymentRequestNumber { get; init; }
    [JsonProperty("agreementNumber")]
    public string AgreementNumber { get; init; } = default!;
    [JsonProperty("contractNumber")]
    public string ContractNumber { get; init; } = default!;
    [JsonProperty("value")]
    [Range(0, 999999999999.99, ErrorMessage = "Value must be between 0 and 999999999999.99")]
    public decimal Value { get; init; }
    [JsonProperty("dueDate")]
    public string DueDate { get; init; } = default!;
    [JsonProperty("invoiceLines")]
    public List<InvoiceLine> InvoiceLines { get; init; } = default!;
    [JsonProperty("appendixReferences")]
    public AppendixReferences AppendixReferences { get; init; } = default!;
}