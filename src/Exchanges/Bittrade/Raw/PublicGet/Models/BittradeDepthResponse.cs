using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeDepthResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] BittradeDepthTick? Tick,
    [property: JsonPropertyName("ts")] long? Ts);
