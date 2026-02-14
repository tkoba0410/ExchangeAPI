using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

/// <summary>
/// bitFlyer /v1/me/getbalance の Raw レスポンス。
/// </summary>
public sealed class GetBalanceItem
{
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("available")]
    public decimal Available { get; init; }
}
