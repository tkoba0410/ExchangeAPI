using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

/// <summary>
/// bitFlyer /v1/me/getbalance の Raw レスポンス。
/// </summary>
public sealed class BitflyerBalanceResponse
{
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("available")]
    public decimal Available { get; init; }
}
