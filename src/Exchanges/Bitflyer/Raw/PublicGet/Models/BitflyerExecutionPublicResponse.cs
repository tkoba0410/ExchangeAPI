using System;
using System.Text.Json.Serialization;
namespace Exchange.Bitflyer.Raw;

/// <summary>
/// Public 約定履歴 (/v1/getexecutions, /v1/executions) のレスポンス DTO。
/// </summary>
public sealed class BitflyerExecutionPublicResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("exec_date")] public DateTimeOffset ExecDate { get; init; }
    [JsonPropertyName("child_order_acceptance_id")] public string? ChildOrderAcceptanceId { get; init; }
}
