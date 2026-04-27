using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;

public interface IBitflyerPublicRealtimeClient : IAsyncDisposable
{
    IAsyncEnumerable<BitflyerRealtimeTickerMessage> SubscribeTickerAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeTickerMessage>> SubscribeTickerStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeExecutionMessage> SubscribeExecutionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeExecutionMessage>> SubscribeExecutionsStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeBoardSnapshotMessage> SubscribeBoardSnapshotsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeBoardSnapshotMessage>> SubscribeBoardSnapshotsStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeBoardDeltaMessage> SubscribeBoardDeltasAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeBoardDeltaMessage>> SubscribeBoardDeltasStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
