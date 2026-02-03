using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;

/// <summary>
/// /v1/me/cancelallchildorders のリクエスト DTO。
/// </summary>
public sealed class CancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; } = ProductCode.Empty;
}
