using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Tests;

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

        var detail = JsonSerializer.Deserialize<RawOrderDetail>(json);

        Assert.NotNull(detail);
        Assert.Equal(OrderState.Submitted, detail!.State);
        Assert.Equal(OrderType.BuyLimit, detail.Type);
    }

    [Fact]
    public void OrderDetail_UnknownState_Throws()
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

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RawOrderDetail>(json));
    }

    [Fact]
    public void OrderDetail_UnknownType_Throws()
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

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RawOrderDetail>(json));
    }

    [Fact]
    public void CreateRetailOrderRequest_UnknownType_Throws()
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

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RawCreateRetailOrderRequest>(json));
    }

    [Fact]
    public void CancelOpenOrdersRequest_KnownSide_Deserialize()
    {
        var json = """
        {
          "side": "buy"
        }
        """;

        var request = JsonSerializer.Deserialize<RawCancelOpenOrdersRequest>(json);

        Assert.NotNull(request);
        Assert.Equal(OrderSide.Buy, request!.Side);
    }
}
