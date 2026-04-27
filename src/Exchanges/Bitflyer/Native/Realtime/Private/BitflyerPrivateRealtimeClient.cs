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
        await using var session = await _credentialProvider.OpenSessionAsync(cancellationToken).ConfigureAwait(false);
        await protocol.AuthenticateAsync(session, cancellationToken).ConfigureAwait(false);
        await protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);

        try
        {
            var attempt = 0;
            while (true)
            {
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

                attempt++;
                await foreach (var lifecycle in ReconnectAsync<T>(
                    channel,
                    "Realtime stream ended before cancellation.",
                    attempt,
                    cancellationToken).ConfigureAwait(false))
                {
                    yield return lifecycle;
                }

                protocol = CreateReconnectProtocol();
                await protocol.AuthenticateAsync(session, cancellationToken).ConfigureAwait(false);
                yield return new BitflyerRealtimeAuthenticationReplayed<T>
                {
                    Channel = channel,
                    OccurredAt = DateTimeOffset.UtcNow,
                    Attempt = attempt,
                };

                await protocol.SubscribeAsync(channel, cancellationToken).ConfigureAwait(false);
                yield return new BitflyerRealtimeResubscribed<T>
                {
                    Channel = channel,
                    OccurredAt = DateTimeOffset.UtcNow,
                    Attempt = attempt,
                };

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

        yield return new BitflyerRealtimeReconnected<T>
        {
            Channel = channel,
            OccurredAt = DateTimeOffset.UtcNow,
            Attempt = attempt,
        };
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
}
