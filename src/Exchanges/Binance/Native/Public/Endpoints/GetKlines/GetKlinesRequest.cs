namespace ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

public sealed class GetKlinesRequest
{
    public required string Symbol { get; init; }
    public required string Interval { get; init; }
    public long? StartTime { get; init; }
    public long? EndTime { get; init; }
    public string? TimeZone { get; init; }
    public int? Limit { get; init; }
}
