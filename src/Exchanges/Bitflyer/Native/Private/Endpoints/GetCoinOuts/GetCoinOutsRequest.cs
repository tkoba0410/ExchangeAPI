namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;

public sealed class GetCoinOutsRequest
{
    public int? Count { get; init; }
    public long? Before { get; init; }
    public long? After { get; init; }
}
