using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Klines;

public sealed class GetKlinesResponse
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("interval")]
    public required string Interval { get; init; }

    [JsonPropertyName("candles")]
    public required IReadOnlyList<KlineCandle> Candles { get; init; }
}
