using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>
/// /v1/me/sendchildorder のレスポンス DTO。
/// </summary>
public sealed class BitflyerSendChildOrderResponse
{
    [JsonPropertyName("child_order_acceptance_id")] public string ChildOrderAcceptanceId { get; init; } = string.Empty;
}

