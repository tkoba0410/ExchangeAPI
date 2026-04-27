using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Realtime;

public sealed class BitflyerRealtimeProtocolClientTests
{
    [Fact]
    public async Task AuthenticateAsync_SendsAuthJsonRpcShapeAndUsesTimestampNonceSignature()
    {
        var transport = new FakeRealtimeTransport();
        transport.EnqueueIncoming("""{"jsonrpc":"2.0","id":1,"result":true}""");
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1_775_000_000_123);
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"),
            () => timestamp,
            () => "0123456789abcdef");

        await client.AuthenticateAsync(new FakeCredentialSession());

        var root = JsonDocument.Parse(transport.SentTexts.Single()).RootElement;
        Assert.Equal("2.0", root.GetProperty("jsonrpc").GetString());
        Assert.Equal(1, root.GetProperty("id").GetInt32());
        Assert.Equal("auth", root.GetProperty("method").GetString());
        var parameters = root.GetProperty("params");
        Assert.Equal("test-api-key", parameters.GetProperty("api_key").GetString());
        Assert.Equal(1_775_000_000_123, parameters.GetProperty("timestamp").GetInt64());
        Assert.Equal("0123456789abcdef", parameters.GetProperty("nonce").GetString());
        Assert.Equal("signed:17750000001230123456789abcdef", parameters.GetProperty("signature").GetString());
    }

    [Fact]
    public async Task AuthenticateAsync_ErrorResponseThrowsControlledException()
    {
        var transport = new FakeRealtimeTransport();
        transport.EnqueueIncoming("""{"jsonrpc":"2.0","id":1,"error":{"code":-32000,"message":"auth failed"}}""");
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"),
            () => DateTimeOffset.FromUnixTimeMilliseconds(1_775_000_000_123),
            () => "0123456789abcdef");

        await Assert.ThrowsAsync<BitflyerRealtimeAuthenticationException>(async () =>
        {
            await client.AuthenticateAsync(new FakeCredentialSession());
        });
    }

    [Fact]
    public async Task SubscribeAsync_SendsSubscribeJsonRpcShape()
    {
        var transport = new FakeRealtimeTransport();
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));

        await client.SubscribeAsync("lightning_ticker_BTC_JPY");

        Assert.Equal(new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"), transport.ConnectedEndpoint);
        var root = JsonDocument.Parse(transport.SentTexts.Single()).RootElement;
        Assert.Equal("subscribe", root.GetProperty("method").GetString());
        Assert.Equal("lightning_ticker_BTC_JPY", root.GetProperty("params").GetProperty("channel").GetString());
    }

    [Fact]
    public async Task UnsubscribeAsync_SendsUnsubscribeJsonRpcShape()
    {
        var transport = new FakeRealtimeTransport();
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));

        await client.UnsubscribeAsync("lightning_board_BTC_JPY");

        var root = JsonDocument.Parse(transport.SentTexts.Single()).RootElement;
        Assert.Equal("unsubscribe", root.GetProperty("method").GetString());
        Assert.Equal("lightning_board_BTC_JPY", root.GetProperty("params").GetProperty("channel").GetString());
    }

    [Fact]
    public async Task ReadMessagesAsync_ParsesChannelMessage()
    {
        var transport = new FakeRealtimeTransport();
        var receivedAt = DateTimeOffset.Parse("2026-04-27T00:00:00Z");
        transport.EnqueueIncoming("""
            {"jsonrpc":"2.0","method":"channelMessage","params":{"channel":"lightning_ticker_BTC_JPY","message":{"product_code":"BTC_JPY","ltp":100}}}
            """);
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"),
            () => receivedAt);

        var messages = await client.ReadMessagesAsync().ToListAsync();

        var message = Assert.Single(messages);
        Assert.Equal("lightning_ticker_BTC_JPY", message.Channel);
        Assert.Equal("BTC_JPY", message.Message.GetProperty("product_code").GetString());
        Assert.Equal(100m, message.Message.GetProperty("ltp").GetDecimal());
        Assert.Equal(receivedAt, message.ReceivedAt);
    }

    [Fact]
    public async Task ReadMessagesAsync_IgnoresNonChannelJsonRpcResponse()
    {
        var transport = new FakeRealtimeTransport();
        transport.EnqueueIncoming("""{"jsonrpc":"2.0","result":true,"id":1}""");
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));

        var messages = await client.ReadMessagesAsync().ToListAsync();

        Assert.Empty(messages);
    }

    [Fact]
    public async Task ReadMessagesAsync_InvalidJsonThrowsControlledException()
    {
        var transport = new FakeRealtimeTransport();
        transport.EnqueueIncoming("""{"jsonrpc":"2.0","method":"channelMessage","params":""");
        await using var client = new BitflyerRealtimeProtocolClient(
            transport,
            new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));

        await Assert.ThrowsAsync<BitflyerRealtimeMessageException>(async () =>
        {
            await foreach (var _ in client.ReadMessagesAsync())
            {
            }
        });
    }
}

internal sealed class FakeCredentialSession : IApiCredentialSession
{
    public string ApiKey => "test-api-key";

    public string Sign(string payload)
    {
        return $"signed:{payload}";
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
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
