using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Realtime;

public sealed class BitflyerPublicRealtimeClientTests
{
    [Fact]
    public async Task SubscribeTickerAsync_DecodesRepresentativeTickerPayload()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_ticker_BTC_JPY", SamplePayloads.Ticker);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var tickers = await client.SubscribeTickerAsync(ProductCodes.BtcJpy).ToListAsync();

        var ticker = Assert.Single(tickers);
        Assert.Equal(ProductCodes.BtcJpy, ticker.ProductCode);
        Assert.Equal(DateTimeOffset.Parse("2026-04-27T12:34:56.789Z"), ticker.Timestamp);
        Assert.Equal(123456789, ticker.TickId);
        Assert.Equal(9999999.5m, ticker.BestBid);
        Assert.Equal(10000000.5m, ticker.BestAsk);
        Assert.Equal(0.12345678m, ticker.BestBidSize);
        Assert.Equal(0.87654321m, ticker.BestAskSize);
        Assert.Equal(1234.5678m, ticker.TotalBidDepth);
        Assert.Equal(2345.6789m, ticker.TotalAskDepth);
        Assert.Equal(10000000m, ticker.Ltp);
        Assert.Equal(3456.789m, ticker.Volume);
        Assert.Equal(456.789m, ticker.VolumeByProduct);
    }

    [Fact]
    public async Task SubscribeExecutionsAsync_DecodesRepresentativeExecutionPayload()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_executions_BTC_JPY", SamplePayloads.Executions);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var executions = await client.SubscribeExecutionsAsync(ProductCodes.BtcJpy).ToListAsync();

        Assert.Collection(
            executions,
            first =>
            {
                Assert.Equal(987654321, first.Id);
                Assert.Equal("BUY", first.Side);
                Assert.Equal(10000001m, first.Price);
                Assert.Equal(0.001m, first.Size);
                Assert.Equal(DateTimeOffset.Parse("2026-04-27T12:35:00.123Z"), first.ExecDate);
                Assert.Equal("JRF20260427-123500-001", first.BuyChildOrderAcceptanceId);
                Assert.Equal("JRF20260427-123500-002", first.SellChildOrderAcceptanceId);
            },
            second =>
            {
                Assert.Equal(987654322, second.Id);
                Assert.Equal("SELL", second.Side);
                Assert.Equal(9999998m, second.Price);
                Assert.Equal(0.0025m, second.Size);
                Assert.Equal(DateTimeOffset.Parse("2026-04-27T12:35:01.456Z"), second.ExecDate);
            });
    }

    [Fact]
    public async Task SubscribeBoardSnapshotsAsync_DecodesRepresentativeBoardSnapshotPayload()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_board_snapshot_BTC_JPY", SamplePayloads.BoardSnapshot);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var snapshots = await client.SubscribeBoardSnapshotsAsync(ProductCodes.BtcJpy).ToListAsync();

        var snapshot = Assert.Single(snapshots);
        Assert.Equal(10000000m, snapshot.MidPrice);
        Assert.Collection(
            snapshot.Bids,
            first =>
            {
                Assert.Equal(9999999m, first.Price);
                Assert.Equal(0.3m, first.Size);
            },
            second =>
            {
                Assert.Equal(9999998m, second.Price);
                Assert.Equal(1.25m, second.Size);
            });
        Assert.Collection(
            snapshot.Asks,
            first =>
            {
                Assert.Equal(10000001m, first.Price);
                Assert.Equal(0.4m, first.Size);
            },
            second =>
            {
                Assert.Equal(10000002m, second.Price);
                Assert.Equal(1.5m, second.Size);
            });
    }

    [Fact]
    public async Task SubscribeBoardDeltasAsync_DecodesRepresentativeBoardDeltaPayload()
    {
        var protocol = new FakeRealtimeProtocolClient();
        protocol.EnqueueMessage("lightning_board_BTC_JPY", SamplePayloads.BoardDelta);
        await using var client = new BitflyerPublicRealtimeClient(protocol);

        var deltas = await client.SubscribeBoardDeltasAsync(ProductCodes.BtcJpy).ToListAsync();

        var delta = Assert.Single(deltas);
        Assert.Equal(10000000m, delta.MidPrice);
        Assert.Collection(
            delta.Bids,
            first =>
            {
                Assert.Equal(9999999m, first.Price);
                Assert.Equal(0m, first.Size);
            },
            second =>
            {
                Assert.Equal(9999997m, second.Price);
                Assert.Equal(0.75m, second.Size);
            });
        Assert.Collection(
            delta.Asks,
            first =>
            {
                Assert.Equal(10000001m, first.Price);
                Assert.Equal(0m, first.Size);
            },
            second =>
            {
                Assert.Equal(10000003m, second.Price);
                Assert.Equal(0.9m, second.Size);
            });
    }

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

internal static class SamplePayloads
{
    internal const string Ticker = """
        {
          "product_code": "BTC_JPY",
          "timestamp": "2026-04-27T12:34:56.789",
          "tick_id": 123456789,
          "best_bid": 9999999.5,
          "best_ask": 10000000.5,
          "best_bid_size": 0.12345678,
          "best_ask_size": 0.87654321,
          "total_bid_depth": 1234.5678,
          "total_ask_depth": 2345.6789,
          "ltp": 10000000,
          "volume": 3456.789,
          "volume_by_product": 456.789
        }
        """;

    internal const string Executions = """
        [
          {
            "id": 987654321,
            "side": "BUY",
            "price": 10000001,
            "size": 0.001,
            "exec_date": "2026-04-27T12:35:00.123",
            "buy_child_order_acceptance_id": "JRF20260427-123500-001",
            "sell_child_order_acceptance_id": "JRF20260427-123500-002"
          },
          {
            "id": 987654322,
            "side": "SELL",
            "price": 9999998,
            "size": 0.0025,
            "exec_date": "2026-04-27T12:35:01.456",
            "buy_child_order_acceptance_id": "JRF20260427-123501-001",
            "sell_child_order_acceptance_id": "JRF20260427-123501-002"
          }
        ]
        """;

    internal const string BoardSnapshot = """
        {
          "mid_price": 10000000,
          "bids": [
            { "price": 9999999, "size": 0.3 },
            { "price": 9999998, "size": 1.25 }
          ],
          "asks": [
            { "price": 10000001, "size": 0.4 },
            { "price": 10000002, "size": 1.5 }
          ]
        }
        """;

    internal const string BoardDelta = """
        {
          "mid_price": 10000000,
          "bids": [
            { "price": 9999999, "size": 0 },
            { "price": 9999997, "size": 0.75 }
          ],
          "asks": [
            { "price": 10000001, "size": 0 },
            { "price": 10000003, "size": 0.9 }
          ]
        }
        """;
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
