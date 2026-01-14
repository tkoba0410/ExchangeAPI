using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawBalanceEntry(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("balance")] string Balance);
