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

    public BitflyerPrivateRealtimeClient(
        IBitflyerPrivateRealtimeProtocolClient protocol,
        IApiCredentialProvider credentialProvider)
    {
        _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        _credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
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
}
