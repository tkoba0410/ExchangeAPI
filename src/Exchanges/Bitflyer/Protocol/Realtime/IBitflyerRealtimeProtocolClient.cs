namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public interface IBitflyerRealtimeProtocolClient : IAsyncDisposable
{
    ValueTask ConnectAsync(CancellationToken cancellationToken = default);

    ValueTask SubscribeAsync(string channel, CancellationToken cancellationToken = default);

    ValueTask UnsubscribeAsync(string channel, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeChannelMessage> ReadMessagesAsync(CancellationToken cancellationToken = default);
}
