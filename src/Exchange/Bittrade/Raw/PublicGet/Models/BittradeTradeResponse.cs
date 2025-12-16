using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeTradeResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] BittradeTradeTick? Tick,
    [property: JsonPropertyName("ts")] long? Ts);
