using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

public sealed class GetKlinesRequest
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }
    [JsonPropertyName("interval")]
    public required string Interval { get; init; }
    [JsonPropertyName("startTime")]
    public long? StartTime { get; init; }
    [JsonPropertyName("endTime")]
    public long? EndTime { get; init; }
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }
}
