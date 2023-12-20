using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Models;

public class PaymentRequestsBatch
{
    [JsonProperty("id")]
    public string Id { get; init; } = default!;

    [JsonProperty("accountType")]
    public string AccountType { get; init; } = default!;

    [JsonProperty("organisation")]
    public string Organisation { get; init; } = default!;

    [JsonProperty("paymentType")]
    public string PaymentType { get; init; } = default!;

    [JsonProperty("schemeType")]
    public string SchemeType { get; init; } = default!;

    [JsonProperty("paymentRequests")]
    public List<PaymentRequest> PaymentRequests { get; init; } = default!;

    [JsonProperty("status")]
    public string Status { get; init; } = default!;

    [JsonProperty("reference")]
    public string Reference { get; set; } = default!;

    [JsonProperty("created")]
    public DateTime Created { get; init; } = DateTime.UtcNow;

    [JsonProperty("approverId")]
    public string ApproverId { get; set; } = default!;

    [JsonProperty("approverEmail")]
    public string ApproverEmail { get; set; } = default!;

    [JsonProperty("approvedBy")]
    public string ApprovedBy { get; set; } = default!;

    [JsonProperty("approved")]
    public DateTime? Approved { get; set; } = default!;

    [JsonProperty]
    public string ApprovalRequestedByEmail { get; set; } = default!;    

    [JsonProperty("updated")]
    public DateTime? Updated { get; init; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; } = default!;

    [JsonProperty("updatedBy")]
    public string UpdatedBy { get; set; } = default!;
}