using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Realtime;

public sealed class BitflyerPublicRealtimeClientTests
{
    [Fact]
    public async Task SubscribeTickerAsync_DecodesTicker()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_ticker_BTC_JPY", """
            {
              "product_code": "BTC_JPY",
              "timestamp": "2026-04-27T12:34:56.789",
              "tick_id": 123,
              "best_bid": 100,
              "best_ask": 101,
              "best_bid_size": 0.1,
              "best_ask_size": 0.2,
              "total_bid_depth": 10,
              "total_ask_depth": 11,
              "ltp": 100.5,
              "volume": 12.34,
              "volume_by_product": 56.78
            }
            """);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var tickers = await client.SubscribeTickerAsync(ProductCodes.BtcJpy).ToListAsync();

        var ticker = Assert.Single(tickers);
        Assert.Equal(["lightning_ticker_BTC_JPY"], protocol.SubscribedChannels);
        Assert.Equal("lightning_ticker_BTC_JPY", ticker.Channel);
        Assert.Equal(ProductCodes.BtcJpy, ticker.ProductCode);
        Assert.Equal(DateTimeOffset.Parse("2026-04-27T12:34:56.789Z"), ticker.Timestamp);
        Assert.Equal(123, ticker.TickId);
        Assert.Equal(100.5m, ticker.Ltp);
        Assert.Equal(["lightning_ticker_BTC_JPY"], protocol.UnsubscribedChannels);
    }

    [Fact]
    public async Task SubscribeExecutionsAsync_FlattensExecutionArray()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_executions_BTC_JPY", """
            [
              {
                "id": 1,
                "side": "BUY",
                "price": 100,
                "size": 0.01,
                "exec_date": "2026-04-27T12:00:00.000",
                "buy_child_order_acceptance_id": "buy-1",
                "sell_child_order_acceptance_id": "sell-1"
              },
              {
                "id": 2,
                "side": "SELL",
                "price": 101,
                "size": 0.02,
                "exec_date": "2026-04-27T12:00:01.000",
                "buy_child_order_acceptance_id": "buy-2",
                "sell_child_order_acceptance_id": "sell-2"
              }
            ]
            """);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var executions = await client.SubscribeExecutionsAsync(ProductCodes.BtcJpy).ToListAsync();

        Assert.Equal(["lightning_executions_BTC_JPY"], protocol.SubscribedChannels);
        Assert.Collection(
            executions,
            first =>
            {
                Assert.Equal(1, first.Id);
                Assert.Equal("BUY", first.Side);
                Assert.Equal(ProductCodes.BtcJpy, first.ProductCode);
            },
            second =>
            {
                Assert.Equal(2, second.Id);
                Assert.Equal("SELL", second.Side);
                Assert.Equal(101m, second.Price);
            });
    }

    [Fact]
    public async Task SubscribeBoardSnapshotsAsync_DecodesBoardSnapshot()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_board_snapshot_BTC_JPY", """
            {
              "mid_price": 100.5,
              "bids": [{ "price": 100, "size": 1.5 }],
              "asks": [{ "price": 101, "size": 2.5 }]
            }
            """);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var snapshots = await client.SubscribeBoardSnapshotsAsync(ProductCodes.BtcJpy).ToListAsync();

        var snapshot = Assert.Single(snapshots);
        Assert.Equal(["lightning_board_snapshot_BTC_JPY"], protocol.SubscribedChannels);
        Assert.Equal(100.5m, snapshot.MidPrice);
        Assert.Equal(100m, Assert.Single(snapshot.Bids).Price);
        Assert.Equal(101m, Assert.Single(snapshot.Asks).Price);
    }

    [Fact]
    public async Task SubscribeBoardDeltasAsync_DecodesBoardDelta()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_board_BTC_JPY", """
            {
              "mid_price": 100.5,
              "bids": [{ "price": 99, "size": 0 }],
              "asks": [{ "price": 102, "size": 3.5 }]
            }
            """);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var deltas = await client.SubscribeBoardDeltasAsync(ProductCodes.BtcJpy).ToListAsync();

        var delta = Assert.Single(deltas);
        Assert.Equal(["lightning_board_BTC_JPY"], protocol.SubscribedChannels);
        Assert.Equal(0m, Assert.Single(delta.Bids).Size);
        Assert.Equal(3.5m, Assert.Single(delta.Asks).Size);
    }

    [Fact]
    public async Task SubscribeTickerAsync_IgnoresUnknownChannel()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_board_BTC_JPY", """{"mid_price":100,"bids":[],"asks":[]}""");
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var tickers = await client.SubscribeTickerAsync(ProductCodes.BtcJpy).ToListAsync();

        Assert.Empty(tickers);
        Assert.Equal(["lightning_ticker_BTC_JPY"], protocol.UnsubscribedChannels);
    }

    [Fact]
    public async Task SubscribeBoardDeltasAsync_UnsubscribesWhenDecodeFails()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_board_BTC_JPY", """[]""");
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var exception = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await foreach (var _ in client.SubscribeBoardDeltasAsync(ProductCodes.BtcJpy))
            {
            }
        });

        Assert.Contains("must be an object", exception.Message, StringComparison.Ordinal);
        Assert.Equal(["lightning_board_BTC_JPY"], protocol.UnsubscribedChannels);
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
