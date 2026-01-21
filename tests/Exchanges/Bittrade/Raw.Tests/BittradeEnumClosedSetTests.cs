using System.Text.Json;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Tests;

public sealed class BittradeEnumClosedSetTests
{
    [Fact]
    public void OrderDetail_KnownTypeAndState_Deserialize()
    {
        var json = """
        {
          "id": 1,
          "symbol": "btcjpy",
          "account-id": "1000",
          "amount": "0.1",
          "price": "100",
          "state": "submitted",
          "type": "buy-limit",
          "client-order-id": null,
          "created-at": 1700000000000,
          "finished-at": null,
          "field-amount": "0",
          "field-cash-amount": "0",
          "field-fees": "0"
        }
        """;

        var detail = JsonSerializer.Deserialize<RawPrivateModels.RawOrderDetail>(json);

        Assert.NotNull(detail);
        Assert.Equal("submitted", detail!.State);
        Assert.Equal("buy-limit", detail.Type);
    }

    [Fact]
    public void OrderDetail_UnknownState_Deserializes()
    {
        var json = """
        {
          "id": 1,
          "symbol": "btcjpy",
          "account-id": "1000",
          "amount": "0.1",
          "price": "100",
          "state": "mystery",
          "type": "buy-limit",
          "client-order-id": null,
          "created-at": 1700000000000,
          "finished-at": null,
          "field-amount": "0",
          "field-cash-amount": "0",
          "field-fees": "0"
        }
        """;

        var detail = JsonSerializer.Deserialize<RawPrivateModels.RawOrderDetail>(json);
        Assert.NotNull(detail);
        Assert.Equal("mystery", detail!.State);
    }

    [Fact]
    public void OrderDetail_UnknownType_Deserializes()
    {
        var json = """
        {
          "id": 1,
          "symbol": "btcjpy",
          "account-id": "1000",
          "amount": "0.1",
          "price": "100",
          "state": "submitted",
          "type": "buy-unknown",
          "client-order-id": null,
          "created-at": 1700000000000,
          "finished-at": null,
          "field-amount": "0",
          "field-cash-amount": "0",
          "field-fees": "0"
        }
        """;

        var detail = JsonSerializer.Deserialize<RawPrivateModels.RawOrderDetail>(json);
        Assert.NotNull(detail);
        Assert.Equal("buy-unknown", detail!.Type);
    }

    [Fact]
    public void CreateRetailOrderRequest_UnknownType_Deserializes()
    {
        var json = """
        {
          "symbol": "btcjpy",
          "type": 3,
          "price": "100",
          "amount": "0.1",
          "cash_amount": null
        }
        """;

        var request = JsonSerializer.Deserialize<RawPrivateModels.RawCreateRetailOrderRequest>(json);
        Assert.NotNull(request);
        Assert.Equal(3, request!.Type);
    }

    [Fact]
    public void CancelOpenOrdersRequest_KnownSide_Deserialize()
    {
        var json = """
        {
          "side": "buy"
        }
        """;

        var request = JsonSerializer.Deserialize<RawPrivateModels.RawCancelOpenOrdersRequest>(json);

        Assert.NotNull(request);
        Assert.Equal("buy", request!.Side);
    }
}
