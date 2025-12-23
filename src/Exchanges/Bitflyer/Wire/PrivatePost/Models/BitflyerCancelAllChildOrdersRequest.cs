using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class CancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
}
