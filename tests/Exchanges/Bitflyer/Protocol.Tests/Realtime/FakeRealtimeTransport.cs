using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Realtime;

internal sealed class FakeRealtimeTransport : IBitflyerRealtimeTransport
{
    private readonly ConcurrentQueue<string> _incoming = new();

    public Uri? ConnectedEndpoint { get; private set; }
    public List<string> SentTexts { get; } = [];
    public bool Disposed { get; private set; }

    public void EnqueueIncoming(string text)
    {
        _incoming.Enqueue(text);
    }

    public ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default)
    {
        ConnectedEndpoint = endpointUri;
        return ValueTask.CompletedTask;
    }

    public ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default)
    {
        SentTexts.Add(text);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<string> ReadTextAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (_incoming.TryDequeue(out var text))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return text;
            await Task.Yield();
        }
    }

    public ValueTask DisposeAsync()
    {
        Disposed = true;
        return ValueTask.CompletedTask;
    }
}
