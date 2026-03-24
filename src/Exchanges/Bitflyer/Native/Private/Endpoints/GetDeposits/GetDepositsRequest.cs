namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;

public sealed class GetDepositsRequest
{
    public int? Count { get; init; }
    public long? Before { get; init; }
    public long? After { get; init; }
}
