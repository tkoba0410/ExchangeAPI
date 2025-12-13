using System;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw.PrivateGet.Models;

/// <summary>
/// /v1/me/getpositions のレスポンス DTO。
/// </summary>
public sealed class BitflyerPositionResponse
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("pnl")] public decimal Pnl { get; init; }
    [JsonPropertyName("open_date")] public DateTimeOffset OpenDate { get; init; }
}
