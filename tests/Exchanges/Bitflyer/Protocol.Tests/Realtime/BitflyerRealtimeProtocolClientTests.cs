using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Realtime;

public sealed class BitflyerRealtimeProtocolClientTests
{
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
