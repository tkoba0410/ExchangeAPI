using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class CancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }
}
