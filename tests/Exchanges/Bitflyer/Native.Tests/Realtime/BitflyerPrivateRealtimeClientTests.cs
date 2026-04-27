using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Private;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Realtime;

public sealed class BitflyerPrivateRealtimeClientTests
{
    [Fact]
    public async Task SubscribeChildOrderEventsAsync_AuthenticatesSubscribesAndDecodesEvents()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("child_order_events", """
            [
              {
                "product_code": "BTC_JPY",
                "child_order_id": "JOR20150101-070921-038077",
                "child_order_acceptance_id": "JRF20150101-070921-194057",
                "event_date": "2015-01-01T07:09:21.9301772Z",
                "event_type": "ORDER",
                "child_order_type": "LIMIT",
                "side": "SELL",
                "price": 500000,
                "size": 0.12,
                "expire_date": "2015-01-01T07:10:21Z"
              }
            ]
            """);
        await using var client = new BitflyerPrivateRealtimeClient(protocol, new FakeCredentialProvider());

        var events = await client.SubscribeChildOrderEventsAsync().ToListAsync();

        var item = Assert.Single(events);
        Assert.Equal(1, protocol.AuthenticateCallCount);
        Assert.Equal([BitflyerRealtimeChannels.ChildOrderEvents()], protocol.SubscribedChannels);
        Assert.Equal([BitflyerRealtimeChannels.ChildOrderEvents()], protocol.UnsubscribedChannels);
        Assert.Equal(ProductCodes.BtcJpy, item.ProductCode);
        Assert.Equal("ORDER", item.EventType);
        Assert.Equal("LIMIT", item.ChildOrderType);
        Assert.Equal("SELL", item.Side);
        Assert.Equal(500000m, item.Price);
        Assert.Equal(0.12m, item.Size);
    }

    [Fact]
    public async Task SubscribeChildOrderEventsStreamAsync_YieldsDataEvent()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("child_order_events", """
            [
              {
                "product_code": "BTC_JPY",
                "child_order_acceptance_id": "JRF20150101-070921-194057",
                "event_date": "2015-01-01T07:09:21.9301772Z",
                "event_type": "ORDER"
              }
            ]
            """);
        await using var client = new BitflyerPrivateRealtimeClient(protocol, new FakeCredentialProvider());

        var first = await StreamEventTestExtensions.ReadFirstAsync(client.SubscribeChildOrderEventsStreamAsync());

        var data = Assert.IsType<BitflyerRealtimeData<BitflyerRealtimeChildOrderEventMessage>>(first);
        Assert.Equal("child_order_events", data.Channel);
        Assert.Equal(ProductCodes.BtcJpy, data.Value.ProductCode);
        Assert.Equal("ORDER", data.Value.EventType);
        Assert.Equal(1, protocol.AuthenticateCallCount);
    }

    [Fact]
    public async Task SubscribeParentOrderEventsAsync_AuthenticatesSubscribesAndDecodesEvents()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("parent_order_events", """
            [
              {
                "product_code": "BTC_JPY",
                "parent_order_id": "JCP20150101-035534-486653",
                "parent_order_acceptance_id": "JRF20150101-035534-188098",
                "event_date": "2015-01-01T03:55:34.9730659Z",
                "event_type": "TRIGGER",
                "parameter_index": 1,
                "child_order_type": "LIMIT",
                "side": "BUY",
                "price": 500000,
                "size": 0.12,
                "expire_date": "2015-01-02T02:35:34.8199789Z",
                "child_order_acceptance_id": "JRF20150101-035534-486668"
              }
            ]
            """);
        await using var client = new BitflyerPrivateRealtimeClient(protocol, new FakeCredentialProvider());

        var events = await client.SubscribeParentOrderEventsAsync().ToListAsync();

        var item = Assert.Single(events);
        Assert.Equal(1, protocol.AuthenticateCallCount);
        Assert.Equal([BitflyerRealtimeChannels.ParentOrderEvents()], protocol.SubscribedChannels);
        Assert.Equal([BitflyerRealtimeChannels.ParentOrderEvents()], protocol.UnsubscribedChannels);
        Assert.Equal(ProductCodes.BtcJpy, item.ProductCode);
        Assert.Equal("TRIGGER", item.EventType);
        Assert.Equal(1, item.ParameterIndex);
        Assert.Equal("LIMIT", item.ChildOrderType);
        Assert.Equal("BUY", item.Side);
        Assert.Equal(500000m, item.Price);
        Assert.Equal(0.12m, item.Size);
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
