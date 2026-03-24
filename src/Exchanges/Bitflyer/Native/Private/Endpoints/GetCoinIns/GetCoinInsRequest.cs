namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;

public sealed class GetCoinInsRequest
{
    public int? Count { get; init; }
    public long? Before { get; init; }
    public long? After { get; init; }
}
