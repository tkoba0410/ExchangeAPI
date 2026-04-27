namespace ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;

public sealed class BitflyerRealtimeClientOptions
{
    public Uri EndpointUri { get; init; } = new("wss://ws.lightstream.bitflyer.com/json-rpc");
    public TimeSpan? ConnectTimeout { get; init; }
}
