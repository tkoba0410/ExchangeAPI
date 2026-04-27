using System.Runtime.CompilerServices;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;

public sealed class BitflyerPublicRealtimeClient : IBitflyerPublicRealtimeClient
{
    private readonly IBitflyerRealtimeProtocolClient _protocol;

    public BitflyerPublicRealtimeClient(IBitflyerRealtimeProtocolClient protocol)
    {
        _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
    }

    public async IAsyncEnumerable<BitflyerRealtimeTickerMessage> SubscribeTickerAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Ticker(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            if (message.Channel == channel)
            {
                yield return BitflyerRealtimeMessageDecoder.DecodeTicker(message);
            }
        }
    }

    public async IAsyncEnumerable<BitflyerRealtimeExecutionMessage> SubscribeExecutionsAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Executions(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            if (message.Channel == channel)
            {
                foreach (var execution in BitflyerRealtimeMessageDecoder.DecodeExecutions(message, productCode))
                {
                    yield return execution;
                }
            }
        }
    }

    public async IAsyncEnumerable<BitflyerRealtimeBoardSnapshotMessage> SubscribeBoardSnapshotsAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.BoardSnapshot(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            if (message.Channel == channel)
            {
                yield return BitflyerRealtimeMessageDecoder.DecodeBoardSnapshot(message, productCode);
            }
        }
    }

    public async IAsyncEnumerable<BitflyerRealtimeBoardDeltaMessage> SubscribeBoardDeltasAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Board(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            if (message.Channel == channel)
            {
                yield return BitflyerRealtimeMessageDecoder.DecodeBoardDelta(message, productCode);
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        return _protocol.DisposeAsync();
    }
}
