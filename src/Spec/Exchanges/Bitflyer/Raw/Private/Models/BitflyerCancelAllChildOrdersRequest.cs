using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class CancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
}
