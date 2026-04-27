using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Private;

public interface IBitflyerPrivateRealtimeClient : IAsyncDisposable
{
    IAsyncEnumerable<BitflyerRealtimeChildOrderEventMessage> SubscribeChildOrderEventsAsync(
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeChildOrderEventMessage>> SubscribeChildOrderEventsStreamAsync(
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeParentOrderEventMessage> SubscribeParentOrderEventsAsync(
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeParentOrderEventMessage>> SubscribeParentOrderEventsStreamAsync(
        CancellationToken cancellationToken = default);
}
