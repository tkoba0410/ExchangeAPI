using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Realtime;

internal sealed class FakeRealtimeProtocolClient : IBitflyerPrivateRealtimeProtocolClient
{
    private readonly ConcurrentQueue<BitflyerRealtimeChannelMessage> _messages = new();

    public List<string> SubscribedChannels { get; } = [];
    public List<string> UnsubscribedChannels { get; } = [];
    public int ConnectCallCount { get; private set; }
    public int AuthenticateCallCount { get; private set; }
    public bool Disposed { get; private set; }

    public void EnqueueMessage(string channel, string payloadJson, DateTimeOffset? receivedAt = null)
    {
        using var document = JsonDocument.Parse(payloadJson);
        _messages.Enqueue(new BitflyerRealtimeChannelMessage
        {
            Channel = channel,
            Message = document.RootElement.Clone(),
            ReceivedAt = receivedAt ?? DateTimeOffset.Parse("2026-04-27T00:00:00Z"),
            RawText = payloadJson,
            RawTextLength = Encoding.UTF8.GetByteCount(payloadJson),
        });
    }

    public ValueTask ConnectAsync(CancellationToken cancellationToken = default)
    {
        ConnectCallCount++;
        return ValueTask.CompletedTask;
    }

    public ValueTask SubscribeAsync(string channel, CancellationToken cancellationToken = default)
    {
        SubscribedChannels.Add(channel);
        return ValueTask.CompletedTask;
    }

    public ValueTask UnsubscribeAsync(string channel, CancellationToken cancellationToken = default)
    {
        UnsubscribedChannels.Add(channel);
        return ValueTask.CompletedTask;
    }

    public ValueTask AuthenticateAsync(
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        AuthenticateCallCount++;
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<BitflyerRealtimeChannelMessage> ReadMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (_messages.TryDequeue(out var message))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return message;
            await Task.Yield();
        }
    }

    public ValueTask DisposeAsync()
    {
        Disposed = true;
        return ValueTask.CompletedTask;
    }
}
