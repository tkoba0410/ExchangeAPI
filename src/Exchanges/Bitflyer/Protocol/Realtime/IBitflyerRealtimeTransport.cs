namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public interface IBitflyerRealtimeTransport : IAsyncDisposable
{
    ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default);

    ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> ReadTextAsync(CancellationToken cancellationToken = default);
}
