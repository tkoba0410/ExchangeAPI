using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerExchangeClient_SendOrder_Tests
{
    [Fact]
    public async Task PlaceMarketOrder_MapsDomainToDtoAndReturnsResult()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawPublicModels.Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(new RawPrivateModels.BalanceResponse[0]);
        var tradingResponse = new RawPrivateModels.RawSendChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-123" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);

        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var result = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<OrderResult>.Ok>(call.Result).Response;

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal("ACCEPT-123", result.Key.Value);
        Assert.NotNull(fakeTrading.LastBodyJson);
        Assert.Contains("\"product_code\":\"BTC_JPY\"", fakeTrading.LastBodyJson!);
        Assert.Contains("\"side\":\"BUY\"", fakeTrading.LastBodyJson!);
        Assert.Contains("\"child_order_type\":\"MARKET\"", fakeTrading.LastBodyJson!);
        Assert.Contains("\"size\":0.01", fakeTrading.LastBodyJson!);
    }

    [Fact]
    public async Task PlaceMarketOrder_WhenApiReturns429_AddsRateLimitCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawPublicModels.Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "too many requests",
            statusCode: (System.Net.HttpStatusCode)429,
            exchangeErrorCode: "TOO_MANY_REQUESTS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new RawPrivateModels.RawSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<OrderResult>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        Assert.Equal(BitflyerOperations.Trading.PlaceOrder, call.Meta.Component);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenInsufficientFunds_MapsBalanceCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawPublicModels.Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "insufficient funds",
            exchangeErrorCode: "INSUFFICIENT_FUNDS");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new RawPrivateModels.RawSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(10m));
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<OrderResult>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
    }

    [Fact]
    public async Task PlaceMarketOrderAsync_WhenAuthError_MapsAuthCategory()
    {
        var fakePublic = new FakeBitflyerPublicApi(new RawPublicModels.Ticker());
        var fakeAccount = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
        var exception = new ExchangeApiException(
            message: "auth failed",
            statusCode: System.Net.HttpStatusCode.Unauthorized,
            exchangeErrorCode: "AUTHENTICATION_ERROR");
        var fakeTrading = new FakeBitflyerPrivateTradingApi(
            new RawPrivateModels.RawSendChildOrderResponse(),
            exceptionToThrow: exception);
        var client = CreateClient(fakePublic, fakeAccount, fakeTrading);

        var call = await client.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<OrderResult>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
    }

    private static BitflyerExchangeClient CreateClient(
        IBitflyerRawMarketDataApi marketData,
        IBitflyerPrivateApi accountApi,
        IBitflyerRawTradingApi tradingApi)
    {
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
        var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
        var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, markets);

        return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedTrading);
    }
}
