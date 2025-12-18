using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeTradeEntry(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("ts")] long Ts);
