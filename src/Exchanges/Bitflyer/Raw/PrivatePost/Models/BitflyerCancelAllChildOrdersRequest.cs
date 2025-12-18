using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class BitflyerCancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
}
