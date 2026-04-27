using System.Runtime.CompilerServices;
using ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Composition.Tests.Realtime;

public sealed class BitflyerRealtimeClientFactoryTests
{
    [Fact]
    public async Task CreatePublicClient_WithTransport_WiresTypedStream()
    {
        var transport = new FakeRealtimeTransport();
        transport.EnqueueIncoming("""
            {"jsonrpc":"2.0","method":"channelMessage","params":{"channel":"lightning_ticker_BTC_JPY","message":{"product_code":"BTC_JPY","timestamp":"2026-04-27T00:00:00.000","tick_id":1,"best_bid":100,"best_ask":101,"best_bid_size":1,"best_ask_size":2,"total_bid_depth":3,"total_ask_depth":4,"ltp":100.5,"volume":5,"volume_by_product":6}}}
            """);
        await using var client = BitflyerRealtimeClientFactory.CreatePublicClient(
            transport,
            new BitflyerRealtimeClientOptions { EndpointUri = new Uri("wss://example.test/json-rpc") });

        var tickers = await client.SubscribeTickerAsync(ProductCodes.BtcJpy).ToListAsync();

        Assert.Equal(new Uri("wss://example.test/json-rpc"), transport.ConnectedEndpoint);
        Assert.Collection(
            transport.SentTexts,
            subscribe =>
            {
                Assert.Contains("\"method\":\"subscribe\"", subscribe, StringComparison.Ordinal);
                Assert.Contains("lightning_ticker_BTC_JPY", subscribe, StringComparison.Ordinal);
            },
            unsubscribe =>
            {
                Assert.Contains("\"method\":\"unsubscribe\"", unsubscribe, StringComparison.Ordinal);
                Assert.Contains("lightning_ticker_BTC_JPY", unsubscribe, StringComparison.Ordinal);
            });
        Assert.Equal(100.5m, Assert.Single(tickers).Ltp);
    }

    [Fact]
    public async Task CreatePublicClient_DisposesTransport()
    {
        var transport = new FakeRealtimeTransport();
        await using (BitflyerRealtimeClientFactory.CreatePublicClient(transport))
        {
        }

        Assert.True(transport.Disposed);
    }

    [Fact]
    public async Task CreatePrivateClient_WithTransport_WiresAuthAndTypedStream()
    {
        var transport = new FakeRealtimeTransport();
        transport.EnqueueIncoming("""{"jsonrpc":"2.0","id":1,"result":true}""");
        transport.EnqueueIncoming("""
            {"jsonrpc":"2.0","method":"channelMessage","params":{"channel":"child_order_events","message":[{"product_code":"BTC_JPY","event_date":"2026-04-27T00:00:00Z","event_type":"ORDER","child_order_type":"LIMIT"}]}}
            """);
        await using var client = BitflyerRealtimeClientFactory.CreatePrivateClient(
            new FakeCredentialProvider(),
            transport,
            new BitflyerRealtimeClientOptions { EndpointUri = new Uri("wss://example.test/json-rpc") });

        var events = await client.SubscribeChildOrderEventsAsync().ToListAsync();

        Assert.Equal(new Uri("wss://example.test/json-rpc"), transport.ConnectedEndpoint);
        Assert.Collection(
            transport.SentTexts,
            auth =>
            {
                Assert.Contains("\"method\":\"auth\"", auth, StringComparison.Ordinal);
                Assert.Contains("\"api_key\":\"test-api-key\"", auth, StringComparison.Ordinal);
                Assert.DoesNotContain("test-api-secret", auth, StringComparison.Ordinal);
            },
            subscribe =>
            {
                Assert.Contains("\"method\":\"subscribe\"", subscribe, StringComparison.Ordinal);
                Assert.Contains("child_order_events", subscribe, StringComparison.Ordinal);
            },
            unsubscribe =>
            {
                Assert.Contains("\"method\":\"unsubscribe\"", unsubscribe, StringComparison.Ordinal);
                Assert.Contains("child_order_events", unsubscribe, StringComparison.Ordinal);
            });
        Assert.Equal("ORDER", Assert.Single(events).EventType);
    }

    private sealed class FakeRealtimeTransport : IBitflyerRealtimeTransport
    {
        private readonly Queue<string> _incoming = new();

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

    private sealed class FakeCredentialProvider : IApiCredentialProvider
    {
        public ValueTask<IApiCredentialSession> OpenSessionAsync(CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult<IApiCredentialSession>(new FakeCredentialSession());
        }
    }

    private sealed class FakeCredentialSession : IApiCredentialSession
    {
        public string ApiKey => "test-api-key";

        public string Sign(string payload)
        {
            return "test-signature";
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}

internal static class AsyncEnumerableTestExtensions
{
    internal static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var items = new List<T>();
        await foreach (var item in source)
        {
            items.Add(item);
        }

        return items;
    }
}
