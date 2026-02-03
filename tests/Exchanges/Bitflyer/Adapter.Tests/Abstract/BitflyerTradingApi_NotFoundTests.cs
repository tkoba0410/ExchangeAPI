using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerTradingApi_NotFoundTests
{
    [Fact]
    public async Task GetOrderAsync_ByAcceptanceId_NotFound_Throws()
    {
        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, markets, privateApi);
        var api = new BitflyerTradingApi(normalized);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-404");
        var call = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), key);
        var err = Assert.IsType<CallResult<OrderStatus>.Err>(call.Result);
        Assert.Contains("Order not found", err.Error.Message);
    }

    [Fact]
    public async Task GetOrderAsync_ByExchangeOrderId_NotFound_Throws()
    {
        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, markets, privateApi);
        var api = new BitflyerTradingApi(normalized);

        var key = new OrderKey(OrderIdKind.ExchangeOrderId, "JRF-404");
        var call = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), key);
        var err = Assert.IsType<CallResult<OrderStatus>.Err>(call.Result);
        Assert.Contains("Order not found", err.Error.Message);

        Assert.Equal(key.Value, tradingApi.LastGetChildOrdersRequest?.ChildOrderId?.Value);
        Assert.Null(tradingApi.LastGetChildOrdersRequest?.ChildOrderAcceptanceId);
    }

}
