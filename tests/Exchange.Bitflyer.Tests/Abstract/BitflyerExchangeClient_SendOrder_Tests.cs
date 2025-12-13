using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;
using Exchange.Bitflyer.Abstract.Facade;
using Exchange.Bitflyer.Raw.PrivateGet.Models;
using Exchange.Bitflyer.Raw.PrivatePost.Models;
using Exchange.Bitflyer.Raw.PublicGet.Models;
using Exchange.Bitflyer.Tests.Fakes;
using Xunit;

namespace Exchange.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_SendOrder_Tests
{
    [Fact]
    public async Task SendOrderAsync_MapsDomainToDtoAndReturnsResult()
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

        var result = await client.SendOrderAsync(order);

        Assert.Equal("ACCEPT-123", result.OrderId);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("BTC_JPY", fakeTrading.LastRequest!.ProductCode);
        Assert.Equal("BUY", fakeTrading.LastRequest!.Side);
        Assert.Equal("MARKET", fakeTrading.LastRequest!.ChildOrderType);
        Assert.Equal(0.01m, fakeTrading.LastRequest!.Size);
    }

    [Fact]
    public async Task SendOrderAsync_StopLimit_MapsChildOrderTypeStopLimit()
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

        var result = await client.SendOrderAsync(order);

        Assert.Equal("ACCEPT-STOP", result.OrderId);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("STOP_LIMIT", fakeTrading.LastRequest!.ChildOrderType);
        Assert.Equal(4000000m, fakeTrading.LastRequest!.Price);
        Assert.Equal(3990000m, fakeTrading.LastRequest!.TriggerPrice);
    }

    [Fact]
    public async Task SendOrderAsync_WhenApiReturns429_AddsRateLimitCategory()
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

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.SendOrderAsync(order));
        Assert.Equal(ExchangeErrorCategory.RateLimit, ex.ErrorCategory);
        Assert.Equal("TOO_MANY_REQUESTS", ex.ExchangeErrorCode);
        Assert.Equal("bitFlyer", ex.ExchangeId);
        Assert.Equal("SendOrder", ex.Operation);
    }

    [Fact]
    public async Task SendOrderAsync_MarketDisallowsPriceOrTrigger()
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

        await Assert.ThrowsAsync<ArgumentException>(() => client.SendOrderAsync(order));
    }

    [Fact]
    public async Task SendOrderAsync_WhenInsufficientFunds_MapsBalanceCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var exception = new ExchangeApiException(
            message: "insufficient funds",
            exchangeErrorCode: "INSUFFICIENT_FUNDS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new BitflyerSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 10m);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.SendOrderAsync(order));
        Assert.Equal(ExchangeErrorCategory.Balance, ex.ErrorCategory);
        Assert.Equal("INSUFFICIENT_FUNDS", ex.ExchangeErrorCode);
    }

    [Fact]
    public async Task SendOrderAsync_WhenAuthError_MapsAuthCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var exception = new ExchangeApiException(
            message: "auth failed",
            statusCode: System.Net.HttpStatusCode.Unauthorized,
            exchangeErrorCode: "AUTHENTICATION_ERROR");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new BitflyerSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 0.01m);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.SendOrderAsync(order));
        Assert.Equal(ExchangeErrorCategory.Auth, ex.ErrorCategory);
        Assert.Equal("AUTHENTICATION_ERROR", ex.ExchangeErrorCode);
    }
}
