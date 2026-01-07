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
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Contracts.Dtos;
using RawTicker = ExchangeApi.Exchanges.Bitflyer.Raw.Ticker;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_SendOrder_Tests
{
    [Fact]
    public async Task PlaceMarketOrder_MapsDomainToDtoAndReturnsResult()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(new BalanceResponse[0]);
        var tradingResponse = new CreateChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-123" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);

        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var result = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderResult>.Ok>(call.Result).Response;

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal("ACCEPT-123", result.Key.Value);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("BTC_JPY", fakeTrading.LastRequest!.Body.ProductCode);
        Assert.Equal("BUY", fakeTrading.LastRequest!.Body.Side);
        Assert.Equal("MARKET", fakeTrading.LastRequest!.Body.ChildOrderType);
        Assert.Equal(0.01m, fakeTrading.LastRequest!.Body.Size);
    }

    [Fact]
    public async Task PlaceMarketOrder_WhenApiReturns429_AddsRateLimitCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "too many requests",
            statusCode: (System.Net.HttpStatusCode)429,
            exchangeErrorCode: "TOO_MANY_REQUESTS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new CreateChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var err = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderResult>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        Assert.Equal(BitflyerOperations.Trading.PlaceOrder, call.Meta.Component);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenInsufficientFunds_MapsBalanceCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "insufficient funds",
            exchangeErrorCode: "INSUFFICIENT_FUNDS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new CreateChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(10m));
        var err = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderResult>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenAuthError_MapsAuthCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawTicker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "auth failed",
            statusCode: System.Net.HttpStatusCode.Unauthorized,
            exchangeErrorCode: "AUTHENTICATION_ERROR");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new CreateChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var err = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderResult>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
    }

    private static BitflyerExchangeClient CreateClient(
        IBitflyerRawMarketDataApi marketData,
        IBitflyerPrivateApi accountApi,
        IBitflyerRawPrivateTradingApi tradingApi)
    {
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
        var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
        var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, accountApi, markets);

        return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedTrading);
    }
}
