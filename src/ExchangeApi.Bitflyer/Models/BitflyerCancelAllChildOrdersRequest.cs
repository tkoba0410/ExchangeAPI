using System.Text.Json.Serialization;

namespace ExchangeApi.Bitflyer.Models;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class BitflyerCancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
}
