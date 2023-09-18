using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Api.Models;

public class PaymentRequest
{
    [JsonProperty("paymentRequestId")]
    public string PaymentRequestId { get; init; } = default!;

    [JsonProperty("sourceSystem")]
    public string SourceSystem { get; init; } = default!;

    // The FRN will replace the frn field in the future, this is just to allow for a smooth transition
    [JsonProperty("frn")]
    public long FRN { get; init; } = default!;

    [JsonProperty("value")]
    [Range(0, 999999999999.99, ErrorMessage = "Value must be between 0 and 999999999999.99")]
    public decimal Value { get; init; }

    [JsonProperty("currency")]
    public string Currency { get; init; } = default!;

    [JsonProperty("description")]
    public string Description { get; init; } = default!;

    [JsonProperty("originalInvoiceNumber")]
    public string OriginalInvoiceNumber { get; init; } = default!;

    [JsonProperty("originalSettlementDate")]
    public string OriginalSettlementDate { get; init; } = default!;

    [JsonProperty("recoveryDate")]
    public string RecoveryDate { get; init; } = default!;

    [JsonProperty("invoiceCorrectionReference")]
    public string InvoiceCorrectionReference { get; init; } = default!;

    [JsonProperty("invoiceLines")]
    public List<InvoiceLine> InvoiceLines { get; init; } = default!;

    [JsonProperty("marketingYear")]
    [Range(2021, 2099, ErrorMessage = "Marketing Year must be between 2021 and 2099 ")]
    public int MarketingYear { get; init; }

    [JsonProperty("paymentRequestNumber")]
    public int PaymentRequestNumber { get; init; }

    [JsonProperty("agreementNumber")]
    public string AgreementNumber { get; init; } = default!;

    [JsonProperty("dueDate")]
    public string DueDate { get; init; } = default!;

    [JsonProperty("appendixReferences")]
    public AppendixReferences AppendixReferences { get; init; } = default!;

    [JsonProperty("sbi")]
    public int SBI { get; init; } = default!;

    [JsonProperty("vendorId")]
    public string VendorID { get; init; } = default!;
}