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

    private ValueTask TryUnsubscribeAsync(string channel)
    {
        return TryUnsubscribeAsync(_protocol, channel);
    }

    private static async ValueTask TryUnsubscribeAsync(
        IBitflyerRealtimeProtocolClient protocol,
        string channel)
    {
        try
        {
            await protocol.UnsubscribeAsync(channel, CancellationToken.None).ConfigureAwait(false);
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
        yield return Diagnostic<T>(
            channel,
            RealtimeDiagnosticEventTypes.Subscribed,
            RealtimeDiagnosticSeverities.Info,
            "Subscription accepted.");

        try
        {
            var attempt = 0;
            while (true)
            {
                var pending = new Queue<BitflyerRealtimeStreamEvent<T>>();
                string? reconnectReason = null;
                using var readCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                await using var enumerator = protocol.ReadMessagesAsync(readCancellation.Token).GetAsyncEnumerator(readCancellation.Token);

                while (reconnectReason is null)
                {
                    StreamReadResult<T> result;
                    try
                    {
                        result = await ReadNextEventAsync(
                            enumerator,
                            readCancellation,
                            channel,
                            decode,
                            pending,
                            cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        yield break;
                    }
                    catch (BitflyerRealtimeException exception) when (IsReconnectTarget(exception))
                    {
                        reconnectReason = exception.Kind.ToString();
                        break;
                    }
                    catch (BitflyerRealtimeException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        reconnectReason = exception.GetType().Name;
                        break;
                    }

                    if (result.Completed)
                    {
                        reconnectReason = "Realtime stream ended before cancellation.";
                        break;
                    }

                    if (result.Event is not null)
                    {
                        yield return result.Event;
                    }
                }

                attempt++;
                await foreach (var lifecycle in ReconnectAsync<T>(channel, reconnectReason, attempt, cancellationToken)
                                   .ConfigureAwait(false))
                {
                    yield return lifecycle;
                }

                protocol = CreateReconnectProtocol();
                try
                {
                    await protocol.ConnectAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new BitflyerRealtimeResilienceException(
                        BitflyerRealtimeErrorKind.ReconnectExhausted,
                        $"Realtime reconnect failed: {exception.GetType().Name}",
                        exception);
                }

                yield return Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.Reconnected,
                    RealtimeDiagnosticSeverities.Info,
                    "Realtime reconnect succeeded.",
                    attempt: attempt);
                yield return new BitflyerRealtimeReconnected<T>
                {
                    Channel = channel,
                    OccurredAt = DateTimeOffset.UtcNow,
                    Attempt = attempt,
                };

                try
                {
                    await protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new BitflyerRealtimeResilienceException(
                        BitflyerRealtimeErrorKind.ResubscribeFailed,
                        $"Realtime resubscribe failed: {exception.GetType().Name}",
                        exception);
                }

                yield return Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.Resubscribed,
                    RealtimeDiagnosticSeverities.Info,
                    "Realtime resubscribe succeeded.",
                    attempt: attempt);
                yield return Resubscribed<T>(channel, attempt);
                yield return Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.ContinuityLost,
                    RealtimeDiagnosticSeverities.Warning,
                    "Realtime stream reconnected after interruption.",
                    attempt: attempt);
                yield return ContinuityLost<T>(channel, "Realtime stream reconnected after interruption.", attempt);
            }
        }
        finally
        {
            await TryUnsubscribeAsync(protocol, channel).ConfigureAwait(false);
        }
    }

    private async ValueTask<StreamReadResult<T>> ReadNextEventAsync<T>(
        IAsyncEnumerator<BitflyerRealtimeChannelMessage> enumerator,
        CancellationTokenSource readCancellation,
        string channel,
        Func<BitflyerRealtimeChannelMessage, IReadOnlyList<T>> decode,
        Queue<BitflyerRealtimeStreamEvent<T>> pending,
        CancellationToken cancellationToken)
    {
        if (pending.TryDequeue(out var pendingEvent))
        {
            return new StreamReadResult<T>(pendingEvent, Completed: false);
        }

        while (await MoveNextWithIdleTimeoutAsync(enumerator, readCancellation, cancellationToken).ConfigureAwait(false))
        {
            var message = enumerator.Current;
            if (message.Channel != channel)
            {
                pending.Enqueue(Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.NonTargetMessageIgnored,
                    RealtimeDiagnosticSeverities.Trace,
                    "Non-target realtime message ignored.",
                    new Dictionary<string, string>
                    {
                        ["receivedChannel"] = message.Channel,
                        ["payloadLength"] = (message.RawTextLength ?? 0).ToString(),
                    }));

                if (pending.TryDequeue(out var ignoredEvent))
                {
                    return new StreamReadResult<T>(ignoredEvent, Completed: false);
                }
            }

            pending.Enqueue(Diagnostic<T>(
                channel,
                RealtimeDiagnosticEventTypes.RawFrameReceived,
                RealtimeDiagnosticSeverities.Trace,
                "Raw realtime frame received.",
                new Dictionary<string, string>
                {
                    ["payloadLength"] = (message.RawTextLength ?? 0).ToString(),
                }));

            var items = DecodeOrReject(message, decode, out var rejected);
            foreach (var item in items)
            {
                pending.Enqueue(Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.MessageDecoded,
                    RealtimeDiagnosticSeverities.Trace,
                    "Realtime message decoded."));
                pending.Enqueue(new BitflyerRealtimeData<T>
                {
                    Channel = channel,
                    OccurredAt = message.ReceivedAt,
                    Value = item,
                });
            }

            if (rejected is not null)
            {
                pending.Enqueue(Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.MessageRejected,
                    RealtimeDiagnosticSeverities.Warning,
                    rejected.Reason,
                    errorKind: rejected.ErrorKind.ToString()));
                pending.Enqueue(rejected);
            }

            if (pending.TryDequeue(out var nextEvent))
            {
                return new StreamReadResult<T>(nextEvent, Completed: false);
            }
        }

        return new StreamReadResult<T>(Event: null, Completed: true);
    }

    private static bool IsReconnectTarget(BitflyerRealtimeException exception)
    {
        return exception.Kind is BitflyerRealtimeErrorKind.ConnectionFailed or BitflyerRealtimeErrorKind.TransportFailed;
    }

    private async ValueTask<bool> MoveNextWithIdleTimeoutAsync(
        IAsyncEnumerator<BitflyerRealtimeChannelMessage> enumerator,
        CancellationTokenSource readCancellation,
        CancellationToken cancellationToken)
    {
        if (_resilience.IdleTimeout is not { } idleTimeout)
        {
            return await enumerator.MoveNextAsync().ConfigureAwait(false);
        }

        var moveNextTask = enumerator.MoveNextAsync().AsTask();
        var delayTask = Task.Delay(idleTimeout, cancellationToken);
        var completed = await Task.WhenAny(moveNextTask, delayTask).ConfigureAwait(false);
        if (completed == moveNextTask)
        {
            return await moveNextTask.ConfigureAwait(false);
        }

        await readCancellation.CancelAsync().ConfigureAwait(false);
        throw new BitflyerRealtimeResilienceException(
            BitflyerRealtimeErrorKind.TransportFailed,
            "Realtime idle timeout elapsed.");
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
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_resilience.MaxAttempts == 0 || attempt > _resilience.MaxAttempts)
        {
            throw new BitflyerRealtimeResilienceException(
                BitflyerRealtimeErrorKind.ReconnectExhausted,
                "Realtime reconnect attempts were exhausted.");
        }

        yield return Diagnostic<T>(
            channel,
            RealtimeDiagnosticEventTypes.Reconnecting,
            RealtimeDiagnosticSeverities.Warning,
            reason,
            attempt: attempt);

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

    private static BitflyerRealtimeDiagnostic<T> Diagnostic<T>(
        string channel,
        string eventType,
        string severity,
        string reason,
        IReadOnlyDictionary<string, string>? attributes = null,
        string? errorKind = null,
        int? attempt = null)
    {
        var now = DateTimeOffset.UtcNow;
        var effectiveAttributes = attributes;
        if (attempt is not null)
        {
            var values = attributes is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(attributes);
            values["attempt"] = attempt.Value.ToString();
            effectiveAttributes = values;
        }

        return new BitflyerRealtimeDiagnostic<T>
        {
            Channel = channel,
            OccurredAt = now,
            Diagnostic = new RealtimeDiagnosticEvent
            {
                EventType = eventType,
                ObservedAt = now,
                Venue = "bitFlyer",
                Channel = channel,
                Severity = severity,
                Reason = reason,
                ErrorKind = errorKind,
                Attributes = effectiveAttributes,
            },
        };
    }

    private readonly record struct StreamReadResult<T>(
        BitflyerRealtimeStreamEvent<T>? Event,
        bool Completed);
}
