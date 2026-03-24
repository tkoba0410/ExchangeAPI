namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;

public sealed class GetBalanceHistoryRequest
{
    public string? CurrencyCode { get; init; }
    public int? Count { get; init; }
    public long? Before { get; init; }
    public long? After { get; init; }
}
