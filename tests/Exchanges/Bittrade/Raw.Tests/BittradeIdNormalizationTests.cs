using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Tests;

public sealed class BittradeIdNormalizationTests
{
    [Fact]
    public void TradeEntry_Id_AllowsNumber()
    {
        var json = """
        { "id": 1, "price": 100, "amount": 0.1, "direction": "buy", "ts": 1700000000001 }
        """;

        var entry = JsonSerializer.Deserialize<TradeEntry>(json);

        Assert.NotNull(entry);
        Assert.Equal("1", entry!.Id.Value);
    }

    [Fact]
    public void TradeEntry_Id_AllowsString()
    {
        var json = """
        { "id": "trade-1", "price": 100, "amount": 0.1, "direction": "buy", "ts": 1700000000001 }
        """;

        var entry = JsonSerializer.Deserialize<TradeEntry>(json);

        Assert.NotNull(entry);
        Assert.Equal("trade-1", entry!.Id.Value);
    }

    [Fact]
    public void MatchResultEntry_IdAndMatchId_AllowString()
    {
        var json = """
        {
          "id": "mr-1",
          "order-id": 100,
          "match-id": "m-10",
          "symbol": "btcjpy",
          "type": "buy-limit",
          "price": 100,
          "filled-amount": 0.1,
          "filled-fees": 0.001,
          "source": "api",
          "created-at": 1700000000000
        }
        """;

        var entry = JsonSerializer.Deserialize<MatchResultEntry>(json);

        Assert.NotNull(entry);
        Assert.Equal("mr-1", entry!.Id.Value);
        Assert.Equal("m-10", entry.MatchId.Value);
        Assert.Equal("100", entry.OrderId.Value);
    }

    [Fact]
    public void DepositWithdrawEntry_Id_AllowsNumber()
    {
        var json = """
        {
          "id": 200,
          "type": "deposit",
          "currency": "btc",
          "amount": 0.01,
          "address": null,
          "tx-hash": null,
          "state": null,
          "created-at": 1700000000000
        }
        """;

        var entry = JsonSerializer.Deserialize<DepositWithdrawEntry>(json);

        Assert.NotNull(entry);
        Assert.Equal("200", entry!.Id.Value);
    }

    [Fact]
    public void RetailOrderEntry_Id_AllowsString()
    {
        var json = """
        {
          "id": "r-1",
          "symbol": "btcjpy",
          "type": 1,
          "price": "100",
          "amount": "0.1",
          "cash_amount": null,
          "status": 0,
          "created_at": 1700000000000
        }
        """;

        var entry = JsonSerializer.Deserialize<RetailOrderEntry>(json);

        Assert.NotNull(entry);
        Assert.Equal("r-1", entry!.Id.Value);
    }

    [Fact]
    public void KlineEntry_Id_IsLongWrapper()
    {
        var json = """
        {
          "ch": "market.btcjpy.kline.1min",
          "status": "ok",
          "ts": 1700000000000,
          "data": [
            { "id": 1700000000, "open": 1, "close": 2, "low": 1, "high": 3, "amount": 1, "vol": 10, "count": 1 }
          ]
        }
        """;

        var response = JsonSerializer.Deserialize<KlinesResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response!.Data);
        Assert.Single(response.Data!);
        Assert.Equal(1700000000, response.Data![0].Id.Value);
    }

    [Fact]
    public void CancelOpenOrdersResult_NextId_IsCursorId()
    {
        var json = """
        { "status": "ok", "data": { "success-count": 1, "failed-count": 0, "next-id": 10 } }
        """;

        var response = JsonSerializer.Deserialize<CancelOpenOrdersResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response!.Data);
        Assert.Equal(10L, response.Data!.NextId!.Value.Value);
    }
}
