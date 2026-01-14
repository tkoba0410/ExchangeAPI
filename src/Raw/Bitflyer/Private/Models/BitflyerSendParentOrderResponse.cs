using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

/// <summary>
/// /v1/me/sendparentorder のレスポンス DTO。
/// </summary>
public sealed class CreateParentOrderResponse
{
    [JsonPropertyName("parent_order_acceptance_id")] public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}
