using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.ProductCode;
using ContractSide = ExchangeApi.Common.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_SendOrder_Tests
{
    [Fact]
    public async Task PlaceMarketOrder_MapsDomainToDtoAndReturnsResult()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(new BitflyerBalanceResponse[0]);
        var tradingResponse = new BitflyerSendChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-123" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);

        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var result = await client.PlaceMarketOrderAsync(Symbol.BtcJpy, ContractSide.Buy, 0.01m);

        Assert.Equal("ACCEPT-123", result.OrderId);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal(RawProductCode.BtcJpy, fakeTrading.LastRequest!.ProductCode);
        Assert.Equal(ExchangeApi.Exchanges.Bitflyer.Raw.Side.Buy, fakeTrading.LastRequest!.Side);
        Assert.Equal(ChildOrderType.Market, fakeTrading.LastRequest!.ChildOrderType);
        Assert.Equal(0.01m, fakeTrading.LastRequest!.Size);
    }

    [Fact]
    public async Task PlaceStopOrderAsync_ThrowsNotSupported()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var tradingResponse = new BitflyerSendChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-STOP" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        await Assert.ThrowsAsync<NotSupportedException>(() =>
            client.PlaceStopOrderAsync(Symbol.BtcJpy, ContractSide.Sell, 0.5m, triggerPrice: 3990000m));
    }

    [Fact]
    public async Task PlaceMarketOrder_WhenApiReturns429_AddsRateLimitCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var exception = new ExchangeApiException(
            message: "too many requests",
            statusCode: (System.Net.HttpStatusCode)429,
            exchangeErrorCode: "TOO_MANY_REQUESTS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new BitflyerSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceMarketOrderAsync(Symbol.BtcJpy, ContractSide.Buy, 0.01m));
        Assert.Equal(ExchangeErrorCategory.RateLimit, ex.ErrorCategory);
        Assert.Equal("TOO_MANY_REQUESTS", ex.ExchangeErrorCode);
        Assert.Equal("bitFlyer", ex.ExchangeId);
        Assert.Equal("SendOrder", ex.Operation);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenInsufficientFunds_MapsBalanceCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var exception = new ExchangeApiException(
            message: "insufficient funds",
            exchangeErrorCode: "INSUFFICIENT_FUNDS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new BitflyerSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceMarketOrderAsync(Symbol.BtcJpy, ContractSide.Buy, 10m));
        Assert.Equal(ExchangeErrorCategory.Balance, ex.ErrorCategory);
        Assert.Equal("INSUFFICIENT_FUNDS", ex.ExchangeErrorCode);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenAuthError_MapsAuthCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var exception = new ExchangeApiException(
            message: "auth failed",
            statusCode: System.Net.HttpStatusCode.Unauthorized,
            exchangeErrorCode: "AUTHENTICATION_ERROR");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new BitflyerSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceMarketOrderAsync(Symbol.BtcJpy, ContractSide.Buy, 0.01m));
        Assert.Equal(ExchangeErrorCategory.Auth, ex.ErrorCategory);
        Assert.Equal("AUTHENTICATION_ERROR", ex.ExchangeErrorCode);
    }
}
