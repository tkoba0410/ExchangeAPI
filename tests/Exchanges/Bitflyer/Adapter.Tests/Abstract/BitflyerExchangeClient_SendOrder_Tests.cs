using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ContractSide = ExchangeApi.Common.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_SendOrder_Tests
{
    [Fact]
    public async Task PlaceMarketOrder_MapsDomainToDtoAndReturnsResult()
    {
        var fakePublic = new FakeBitflyerPublicApi(new Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(new BalanceResponse[0]);
        var tradingResponse = new CreateChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-123" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);

        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var result = await client.PlaceMarketOrderAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal("ACCEPT-123", result.Key.Value);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("BTC_JPY", fakeTrading.LastRequest!.Body.ProductCode);
        Assert.Equal("BUY", fakeTrading.LastRequest!.Body.Side);
        Assert.Equal("MARKET", fakeTrading.LastRequest!.Body.ChildOrderType);
        Assert.Equal(0.01m, fakeTrading.LastRequest!.Body.Size);
    }

    [Fact]
    public async Task PlaceStopOrderAsync_ThrowsNotSupported()
    {
        var fakePublic = new FakeBitflyerPublicApi(new Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var tradingResponse = new CreateChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-STOP" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        await Assert.ThrowsAsync<ExchangeFeatureNotSupportedException>(() =>
            client.PlaceStopOrderAsync(new Symbol("BTC/JPY"), ContractSide.Sell, new Size(0.5m), triggerPrice: new Price(3990000m)));
    }

    [Fact]
    public async Task PlaceMarketOrder_WhenApiReturns429_AddsRateLimitCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "too many requests",
            statusCode: (System.Net.HttpStatusCode)429,
            exchangeErrorCode: "TOO_MANY_REQUESTS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new CreateChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceMarketOrderAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m)));
        Assert.Equal(ExchangeErrorCategory.RateLimit, ex.ErrorCategory);
        Assert.Equal(ExchangeCode.Bitflyer, ex.Exchange);
        Assert.Equal(BitflyerOperations.Trading.PlaceOrder, ex.Operation);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenInsufficientFunds_MapsBalanceCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "insufficient funds",
            exchangeErrorCode: "INSUFFICIENT_FUNDS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new CreateChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceMarketOrderAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(10m)));
        Assert.Equal(ExchangeErrorCategory.Request, ex.ErrorCategory);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenAuthError_MapsAuthCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "auth failed",
            statusCode: System.Net.HttpStatusCode.Unauthorized,
            exchangeErrorCode: "AUTHENTICATION_ERROR");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new CreateChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => client.PlaceMarketOrderAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m)));
        Assert.Equal(ExchangeErrorCategory.Auth, ex.ErrorCategory);
    }

        private static BitflyerExchangeClient CreateClient(
            IBitflyerRawMarketDataApi marketData,
            IBitflyerPrivateApi accountApi,
            IBitflyerRawPrivateTradingApi tradingApi)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
        var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
        var normalizedMargin = BitflyerTestHelpers.CreateMarginApi(accountApi, markets);
        var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, accountApi, markets);

        return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedMargin, normalizedTrading);
    }
}
