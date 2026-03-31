using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Binance.Vocabulary;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

/// <summary>
/// Requests Binance public kline candles for a symbol and interval.
/// </summary>
public sealed class GetKlinesRequest
{
    /// <summary>
    /// Trading symbol such as <c>BTCJPY</c> or <c>BTCUSDT</c>.
    /// </summary>
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }
    /// <summary>
    /// Binance kline interval literal.
    /// </summary>
    [JsonPropertyName("interval")]
    public required BinanceInterval Interval { get; init; }
    /// <summary>
    /// Optional start timestamp in epoch milliseconds.
    /// </summary>
    [JsonPropertyName("startTime")]
    public long? StartTime { get; init; }
    /// <summary>
    /// Optional end timestamp in epoch milliseconds.
    /// </summary>
    [JsonPropertyName("endTime")]
    public long? EndTime { get; init; }
    /// <summary>
    /// Optional Binance time zone override. Stage11 MCP does not expose this field.
    /// </summary>
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }
    /// <summary>
    /// Optional candle count limit.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }
}
