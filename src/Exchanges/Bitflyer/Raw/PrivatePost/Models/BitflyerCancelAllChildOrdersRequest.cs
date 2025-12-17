using System.Text.Json.Serialization;
namespace Exchange.Bitflyer.Raw;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class BitflyerCancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
}
