using System.Runtime.CompilerServices;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;

public sealed class BitflyerPublicRealtimeClient : IBitflyerPublicRealtimeClient
{
    private readonly IBitflyerRealtimeProtocolClient _protocol;
    private readonly Func<IBitflyerRealtimeProtocolClient>? _protocolFactory;
    private readonly BitflyerRealtimeResilienceOptions _resilience;

    public BitflyerPublicRealtimeClient(IBitflyerRealtimeProtocolClient protocol)
        : this(protocol, null, null)
    {
    }

    internal BitflyerPublicRealtimeClient(
        IBitflyerRealtimeProtocolClient protocol,
        Func<IBitflyerRealtimeProtocolClient>? protocolFactory,
        BitflyerRealtimeResilienceOptions? resilience)
    {
        _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        _protocolFactory = protocolFactory;
        _resilience = resilience ?? BitflyerRealtimeResilienceOptions.Disabled;
    }

    public async IAsyncEnumerable<BitflyerRealtimeTickerMessage> SubscribeTickerAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Ticker(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
            {
                if (message.Channel == channel)
                {
                    yield return BitflyerRealtimeMessageDecoder.DecodeTicker(message);
                }
            }
        }
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    public IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeTickerMessage>> SubscribeTickerStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Ticker(productCode);
        return SubscribeStreamAsync<BitflyerRealtimeTickerMessage>(
            channel,
            static message => [BitflyerRealtimeMessageDecoder.DecodeTicker(message)],
            cancellationToken);
    }

    public async IAsyncEnumerable<BitflyerRealtimeExecutionMessage> SubscribeExecutionsAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Executions(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
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
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    public IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeExecutionMessage>> SubscribeExecutionsStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Executions(productCode);
        return SubscribeStreamAsync<BitflyerRealtimeExecutionMessage>(
            channel,
            message => BitflyerRealtimeMessageDecoder.DecodeExecutions(message, productCode),
            cancellationToken);
    }

    public async IAsyncEnumerable<BitflyerRealtimeBoardSnapshotMessage> SubscribeBoardSnapshotsAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.BoardSnapshot(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
            {
                if (message.Channel == channel)
                {
                    yield return BitflyerRealtimeMessageDecoder.DecodeBoardSnapshot(message, productCode);
                }
            }
        }
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    public IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeBoardSnapshotMessage>> SubscribeBoardSnapshotsStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.BoardSnapshot(productCode);
        return SubscribeStreamAsync<BitflyerRealtimeBoardSnapshotMessage>(
            channel,
            message => [BitflyerRealtimeMessageDecoder.DecodeBoardSnapshot(message, productCode)],
            cancellationToken);
    }

    public async IAsyncEnumerable<BitflyerRealtimeBoardDeltaMessage> SubscribeBoardDeltasAsync(
        string productCode,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Board(productCode);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
            {
                if (message.Channel == channel)
                {
                    yield return BitflyerRealtimeMessageDecoder.DecodeBoardDelta(message, productCode);
                }
            }
        }
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    public IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeBoardDeltaMessage>> SubscribeBoardDeltasStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Board(productCode);
        return SubscribeStreamAsync<BitflyerRealtimeBoardDeltaMessage>(
            channel,
            message => [BitflyerRealtimeMessageDecoder.DecodeBoardDelta(message, productCode)],
            cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _protocol.DisposeAsync();
    }

    private async ValueTask TryUnsubscribeAsync(string channel)
    {
        try
        {
            await _protocol.UnsubscribeAsync(channel, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception)
        {
        }
    }

    private async IAsyncEnumerable<BitflyerRealtimeStreamEvent<T>> SubscribeStreamAsync<T>(
        string channel,
        Func<BitflyerRealtimeChannelMessage, IReadOnlyList<T>> decode,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var protocol = _protocol;
        await protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            var attempt = 0;
            while (true)
            {
                var completed = false;
                await foreach (var message in protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (message.Channel != channel)
                    {
                        continue;
                    }

                    var items = DecodeOrReject(message, decode, out var rejected);
                    foreach (var item in items)
                    {
                        yield return new BitflyerRealtimeData<T>
                        {
                            Channel = channel,
                            OccurredAt = message.ReceivedAt,
                            Value = item,
                        };
                    }

                    if (rejected is not null)
                    {
                        yield return rejected;
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                completed = true;
                if (completed)
                {
                    await foreach (var lifecycle in ReconnectAsync<T>(
                        channel,
                        "Realtime stream ended before cancellation.",
                        ++attempt,
                        () => ValueTask.CompletedTask,
                        cancellationToken).ConfigureAwait(false))
                    {
                        yield return lifecycle;
                    }

                    protocol = CreateReconnectProtocol();
                    await protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);
                    yield return Resubscribed<T>(channel, attempt);
                    yield return ContinuityLost<T>(channel, "Realtime stream reconnected after interruption.", attempt);
                }
            }
        }
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    private IReadOnlyList<T> DecodeOrReject<T>(
        BitflyerRealtimeChannelMessage message,
        Func<BitflyerRealtimeChannelMessage, IReadOnlyList<T>> decode,
        out BitflyerRealtimeMessageRejected<T>? rejected)
    {
        try
        {
            rejected = null;
            return decode(message);
        }
        catch (Exception exception)
        {
            rejected = new BitflyerRealtimeMessageRejected<T>
            {
                Channel = message.Channel,
                OccurredAt = message.ReceivedAt,
                ErrorKind = BitflyerRealtimeErrorKind.MessageDecodeFailed,
                Reason = exception.GetType().Name,
            };
            return [];
        }
    }

    private async IAsyncEnumerable<BitflyerRealtimeStreamEvent<T>> ReconnectAsync<T>(
        string channel,
        string reason,
        int attempt,
        Func<ValueTask> afterReconnect,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_resilience.MaxAttempts == 0 || attempt > _resilience.MaxAttempts)
        {
            throw new BitflyerRealtimeResilienceException(
                BitflyerRealtimeErrorKind.ReconnectExhausted,
                "Realtime reconnect attempts were exhausted.");
        }

        yield return new BitflyerRealtimeReconnecting<T>
        {
            Channel = channel,
            OccurredAt = DateTimeOffset.UtcNow,
            Attempt = attempt,
            Reason = reason,
        };

        var delay = _resilience.GetDelay(attempt);
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        yield return new BitflyerRealtimeReconnected<T>
        {
            Channel = channel,
            OccurredAt = DateTimeOffset.UtcNow,
            Attempt = attempt,
        };

        await afterReconnect().ConfigureAwait(false);
    }

    private IBitflyerRealtimeProtocolClient CreateReconnectProtocol()
    {
        if (_protocolFactory is null)
        {
            throw new BitflyerRealtimeResilienceException(
                BitflyerRealtimeErrorKind.ReconnectExhausted,
                "Realtime reconnect requires a protocol factory.");
        }

        return _protocolFactory();
    }

    private static BitflyerRealtimeResubscribed<T> Resubscribed<T>(string channel, int attempt)
    {
        return new BitflyerRealtimeResubscribed<T>
        {
            Channel = channel,
            OccurredAt = DateTimeOffset.UtcNow,
            Attempt = attempt,
        };
    }

    private static BitflyerRealtimeContinuityLost<T> ContinuityLost<T>(string channel, string reason, int attempt)
    {
        return new BitflyerRealtimeContinuityLost<T>
        {
            Channel = channel,
            OccurredAt = DateTimeOffset.UtcNow,
            Reason = reason,
            ReconnectAttempt = attempt,
        };
    }
}
