using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Contracts.Dtos;
using Xunit;
using ContractSide = ExchangeApi.Common.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerOrderKeyConnectivityTests
{
    [Fact]
    public async Task OrderResultKey_CanGetAndCancel()
    {
        var acceptanceId = "ACCEPT-1";
        var childOrders = new[]
        {
            new ChildOrderResponse
            {
                ChildOrderId = "JRF-1",
                ChildOrderAcceptanceId = acceptanceId,
                ProductCode = "BTC_JPY",
                Side = "BUY",
                ChildOrderType = "LIMIT",
                Size = 0.01m,
                ExecutedSize = 0m,
                OutstandingSize = 0.01m
            }
        };

        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>(), childOrders: childOrders);
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse
        {
            ChildOrderAcceptanceId = acceptanceId
        });
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, privateApi, markets);
        var api = new BitflyerTradingApi(normalized);

        var resultCall = await api.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var result = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderResult>.Ok>(resultCall.Result).Response;
        var statusCall = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), result.Key);
        var status = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderStatus>.Ok>(statusCall.Result).Response;
        var cancelCall = await api.CancelOrderCallAsync(new Symbol("BTC/JPY"), result.Key);
        Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<CancelResult>.Ok>(cancelCall.Result);

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal(acceptanceId, result.Key.Value);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal(acceptanceId, status.Key.Value);
        Assert.Equal(acceptanceId, tradingApi.LastCancelRequest!.Body.ChildOrderAcceptanceId);
    }

    [Fact]
    public async Task OpenOrderKey_CanGetAndCancel()
    {
        var acceptanceId = "ACCEPT-2";
        var childOrders = new[]
        {
            new ChildOrderResponse
            {
                ChildOrderId = "JRF-2",
                ChildOrderAcceptanceId = acceptanceId,
                ProductCode = "BTC_JPY",
                Side = "BUY",
                ChildOrderType = "LIMIT",
                Size = 0.01m,
                ExecutedSize = 0m,
                OutstandingSize = 0.01m
            }
        };

        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>(), childOrders: childOrders);
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, privateApi, markets);
        var api = new BitflyerTradingApi(normalized);

        var openOrdersCall = await api.GetOrdersCallAsync(new Symbol("BTC/JPY"));
        var openOrders = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<IReadOnlyList<OpenOrder>>.Ok>(openOrdersCall.Result).Response;
        var key = openOrders[0].Key;

        var statusCall = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), key);
        var status = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<OrderStatus>.Ok>(statusCall.Result).Response;
        var cancelCall = await api.CancelOrderCallAsync(new Symbol("BTC/JPY"), key);
        Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<CancelResult>.Ok>(cancelCall.Result);

        Assert.Equal(OrderIdKind.AcceptanceId, key.Kind);
        Assert.Equal(acceptanceId, key.Value);
        Assert.Equal(acceptanceId, tradingApi.LastCancelRequest!.Body.ChildOrderAcceptanceId);
        Assert.Equal(acceptanceId, status.Key.Value);
    }

}
