using System.Threading.Tasks;
using ExchangeApi.Core.Dtos;
using ExchangeApi.Core.Errors;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Adapter.Bitflyer.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Adapter.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_PlaceOrder_Tests
{
    [Fact]
    public async Task PlaceOrderAsync_MapsDomainToDtoAndReturnsResult()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(new BitflyerBalanceResponse[0]);
        var tradingResponse = new BitflyerSendChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-123" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);

        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 0.01m);

        var result = await client.PlaceOrderAsync(order);

        Assert.Equal("ACCEPT-123", result.OrderId);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("BTC_JPY", fakeTrading.LastRequest!.ProductCode);
        Assert.Equal("BUY", fakeTrading.LastRequest!.Side);
        Assert.Equal("MARKET", fakeTrading.LastRequest!.ChildOrderType);
        Assert.Equal(0.01m, fakeTrading.LastRequest!.Size);
    }

    [Fact]
    public async Task PlaceOrderAsync_StopLimit_MapsChildOrderTypeStopLimit()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var tradingResponse = new BitflyerSendChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-STOP" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Sell,
            OrderType: OrderType.Stop,
            Size: 0.5m,
            Price: 4000000m,
            TriggerPrice: 3990000m);

        var result = await client.PlaceOrderAsync(order);

        Assert.Equal("ACCEPT-STOP", result.OrderId);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("STOP_LIMIT", fakeTrading.LastRequest!.ChildOrderType);
        Assert.Equal(4000000m, fakeTrading.LastRequest!.Price);
        Assert.Equal(3990000m, fakeTrading.LastRequest!.TriggerPrice);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenApiReturns429_AddsRateLimitCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var exception = new ExchangeApiException(
            message: "too many requests",
            statusCode: (System.Net.HttpStatusCode)429,
            exchangeErrorCode: "TOO_MANY_REQUESTS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new BitflyerSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 0.01m);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceOrderAsync(order));
        Assert.Equal(ExchangeErrorCategory.RateLimit, ex.ErrorCategory);
        Assert.Equal("TOO_MANY_REQUESTS", ex.ExchangeErrorCode);
        Assert.Equal("bitFlyer", ex.ExchangeId);
        Assert.Equal("SendOrder", ex.Operation);
    }

    [Fact]
    public async Task PlaceOrderAsync_MarketDisallowsPriceOrTrigger()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var fakeTrading = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 0.01m,
            Price: 1m,
            TriggerPrice: 1m);

        await Assert.ThrowsAsync<ArgumentException>(() => client.PlaceOrderAsync(order));
    }
}
