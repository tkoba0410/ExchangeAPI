namespace ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;

public sealed class BitflyerRealtimeClientOptions
{
    public Uri EndpointUri { get; init; } = new("wss://ws.lightstream.bitflyer.com/json-rpc");
    public TimeSpan? ConnectTimeout { get; init; }
    public BitflyerRealtimeReconnectOptions Reconnect { get; init; } = new();
    public TimeSpan? IdleTimeout { get; init; }
}

public sealed class BitflyerRealtimeReconnectOptions
{
    public int MaxAttempts { get; init; } = 3;
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromSeconds(1);
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(10);
}
