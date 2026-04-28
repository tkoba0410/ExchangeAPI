using System.Runtime.CompilerServices;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Private;

public sealed class BitflyerPrivateRealtimeClient : IBitflyerPrivateRealtimeClient
{
    private readonly IBitflyerPrivateRealtimeProtocolClient _protocol;
    private readonly IApiCredentialProvider _credentialProvider;
    private readonly Func<IBitflyerPrivateRealtimeProtocolClient>? _protocolFactory;
    private readonly BitflyerRealtimeResilienceOptions _resilience;

    public BitflyerPrivateRealtimeClient(
        IBitflyerPrivateRealtimeProtocolClient protocol,
        IApiCredentialProvider credentialProvider)
        : this(protocol, credentialProvider, null, null)
    {
    }

    internal BitflyerPrivateRealtimeClient(
        IBitflyerPrivateRealtimeProtocolClient protocol,
        IApiCredentialProvider credentialProvider,
        Func<IBitflyerPrivateRealtimeProtocolClient>? protocolFactory,
        BitflyerRealtimeResilienceOptions? resilience)
    {
        _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        _credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
        _protocolFactory = protocolFactory;
        _resilience = resilience ?? BitflyerRealtimeResilienceOptions.Disabled;
    }

    public async IAsyncEnumerable<BitflyerRealtimeChildOrderEventMessage> SubscribeChildOrderEventsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.ChildOrderEvents();
        await using var session = await _credentialProvider.OpenSessionAsync(cancellationToken).ConfigureAwait(false);
        await _protocol.AuthenticateAsync(session, cancellationToken).ConfigureAwait(false);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
            {
                if (message.Channel == channel)
                {
                    foreach (var item in BitflyerRealtimeMessageDecoder.DecodeChildOrderEvents(message))
                    {
                        yield return item;
                    }
                }
            }
        }
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    public IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeChildOrderEventMessage>> SubscribeChildOrderEventsStreamAsync(
        CancellationToken cancellationToken = default)
    {
        return SubscribeStreamAsync(
            BitflyerRealtimeChannels.ChildOrderEvents(),
            BitflyerRealtimeMessageDecoder.DecodeChildOrderEvents,
            cancellationToken);
    }

    public async IAsyncEnumerable<BitflyerRealtimeParentOrderEventMessage> SubscribeParentOrderEventsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.ParentOrderEvents();
        await using var session = await _credentialProvider.OpenSessionAsync(cancellationToken).ConfigureAwait(false);
        await _protocol.AuthenticateAsync(session, cancellationToken).ConfigureAwait(false);
        await _protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            await foreach (var message in _protocol.ReadMessagesAsync(cancellationToken).ConfigureAwait(false))
            {
                if (message.Channel == channel)
                {
                    foreach (var item in BitflyerRealtimeMessageDecoder.DecodeParentOrderEvents(message))
                    {
                        yield return item;
                    }
                }
            }
        }
        finally
        {
            await TryUnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }

    public IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeParentOrderEventMessage>> SubscribeParentOrderEventsStreamAsync(
        CancellationToken cancellationToken = default)
    {
        return SubscribeStreamAsync(
            BitflyerRealtimeChannels.ParentOrderEvents(),
            BitflyerRealtimeMessageDecoder.DecodeParentOrderEvents,
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
        await using var session = await _credentialProvider.OpenSessionAsync(cancellationToken).ConfigureAwait(false);
        await protocol.AuthenticateAsync(session, cancellationToken).ConfigureAwait(false);
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
                    await protocol.AuthenticateAsync(session, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new BitflyerRealtimeResilienceException(
                        BitflyerRealtimeErrorKind.AuthenticationFailed,
                        $"Realtime authentication replay failed: {exception.GetType().Name}",
                        exception);
                }

                yield return new BitflyerRealtimeAuthenticationReplayed<T>
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
                yield return new BitflyerRealtimeResubscribed<T>
                {
                    Channel = channel,
                    OccurredAt = DateTimeOffset.UtcNow,
                    Attempt = attempt,
                };

                yield return Diagnostic<T>(
                    channel,
                    RealtimeDiagnosticEventTypes.ContinuityLost,
                    RealtimeDiagnosticSeverities.Warning,
                    "Realtime stream reconnected after interruption.",
                    attempt: attempt);
                yield return new BitflyerRealtimeContinuityLost<T>
                {
                    Channel = channel,
                    OccurredAt = DateTimeOffset.UtcNow,
                    Reason = "Realtime stream reconnected after interruption.",
                    ReconnectAttempt = attempt,
                };
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
                continue;
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

    private IBitflyerPrivateRealtimeProtocolClient CreateReconnectProtocol()
    {
        if (_protocolFactory is null)
        {
            throw new BitflyerRealtimeResilienceException(
                BitflyerRealtimeErrorKind.ReconnectExhausted,
                "Realtime reconnect requires a protocol factory.");
        }

        return _protocolFactory();
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
