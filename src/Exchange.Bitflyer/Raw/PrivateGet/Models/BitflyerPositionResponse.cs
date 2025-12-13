using System;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>
/// /v1/me/getpositions のレスポンス DTO。
/// </summary>
public sealed class BitflyerPositionResponse
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("pnl")] public decimal Pnl { get; init; }
    [JsonPropertyName("open_date")] public DateTimeOffset OpenDate { get; init; }
}
