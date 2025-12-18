using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeMergedTick(
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("ts")] long? Ts,
    [property: JsonPropertyName("bid")] decimal[] Bid,
    [property: JsonPropertyName("ask")] decimal[] Ask);
