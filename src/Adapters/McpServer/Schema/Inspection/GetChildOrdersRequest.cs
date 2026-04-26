using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetChildOrdersRequest : BitflyerPrivateReadRequestBase
{
    [JsonPropertyName("productCode")]
    public string? ProductCode { get; init; }

    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("before")]
    public long? Before { get; init; }

    [JsonPropertyName("after")]
    public long? After { get; init; }

    [JsonPropertyName("childOrderState")]
    public string? ChildOrderState { get; init; }

    [JsonPropertyName("childOrderId")]
    public string? ChildOrderId { get; init; }

    [JsonPropertyName("childOrderAcceptanceId")]
    public string? ChildOrderAcceptanceId { get; init; }

    [JsonPropertyName("parentOrderId")]
    public string? ParentOrderId { get; init; }
}
