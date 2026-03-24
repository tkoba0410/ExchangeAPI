namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;

public sealed class GetWithdrawalsRequest
{
    public int? Count { get; init; }
    public long? Before { get; init; }
    public long? After { get; init; }
    public string? MessageId { get; init; }
}
