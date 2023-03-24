using Newtonsoft.Json;

namespace Invoices.Api.Models;

public class InvoiceHeader
{
    [JsonProperty("paymentRequestId")]
    public string PaymentRequestId { get; init; } = default!;
    [JsonProperty("frn")]
    public int FRN { get; init; }
    [JsonProperty("sourceSystem")]
    public string SourceSystem { get; init; } = default!;
    [JsonProperty("marketingYear")]
    public int MarketingYear { get; init; }
    [JsonProperty("ledger")]
    public string Ledger { get; init; } = default!;
    [JsonProperty("deliveryBody")]
    public string DeliveryBody { get; init; } = default!;
    [JsonProperty("paymentRequestNumber")]
    public int PaymentRequestNumber { get; init; }
    [JsonProperty("agreementNumber")]
    public string AgreementNumber { get; init; } = default!;
    [JsonProperty("contractNumber")]
    public string ContractNumber { get; init; } = default!;
    [JsonProperty("value")]
    public int Value { get; init; }
    [JsonProperty("dueDate")]
    public string DueDate { get; init; } = default!;
    [JsonProperty("invoiceLines")]
    public List<InvoiceLine> InvoiceLines { get; init; } = default!;
    [JsonProperty("appendixReferences")]
    public AppendixReferences AppendixReferences { get; init; } = default!;
}