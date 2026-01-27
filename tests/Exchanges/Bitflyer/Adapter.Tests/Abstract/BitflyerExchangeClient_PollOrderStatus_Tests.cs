using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Application.Interfaces;
using ExchangeApi.Application.Trading;
using ExchangeApi.Composition.Adapters.Application;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Application.UseCases;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerExchangeClient_PollOrderStatus_Tests
{
    [Fact]
    public async Task WaitForOrderAsync_CompletesWhenStateTransitions()
    {
        var acceptanceId = "ACCEPT-1";
        var active = new RawPrivateModels.RawGetChildOrdersResponse
        {
            ProductCode = "BTC_JPY",
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "ACTIVE",
            ExecutedSize = 0m,
            OutstandingSize = 0.01m,
            Price = 3000000m,
            AveragePrice = 0m,
            Side = "BUY",
            ChildOrderType = "LIMIT",
            Size = 0.01m,
        };
        var completed = new RawPrivateModels.RawGetChildOrdersResponse
        {
            ProductCode = active.ProductCode,
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "COMPLETED",
            ExecutedSize = 0.01m,
            OutstandingSize = 0m,
            Price = active.Price,
            AveragePrice = 3000000m,
            Side = active.Side,
            ChildOrderType = active.ChildOrderType,
            Size = active.Size,
        };

        var accountApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(
            new RawPrivateModels.RawSendChildOrderResponse(),
            snapshots: new[] { new[] { active }, new[] { completed } });
        var raw = new FakeBitflyerPublicApi(new RawPublicModels.Ticker(), privateApi: accountApi, tradingApi: tradingApi);
        var client = CreateClient(raw);

        IOrderQueryApi orderQueryApi = new TradingApiOrderQueryAdapter(client);
        var statusCall = await OrderPolling.WaitForOrderAsync(
            api: orderQueryApi,
            symbol: new Symbol("BTC/JPY"),
            orderKey: new OrderKey(OrderIdKind.AcceptanceId, acceptanceId),
            options: new PollingOptions(TimeSpan.FromMilliseconds(1), 5));

        var status = Assert.IsType<CallResult<OrderStatusSnapshot>.Ok>(statusCall.Result).Response;
        Assert.Equal(OrderState.Completed, status.Status);
        Assert.Equal(0m, status.OutstandingSize.Value);
        Assert.Equal(0.01m, status.ExecutedSize.Value);
        Assert.Equal(3000000m, status.AveragePrice!.Value.Value);
    }

    private static BitflyerExchangeClient CreateClient(IBitflyerRawApi raw)
    {
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateNormalizedApi(raw, markets);
        return new BitflyerExchangeClient(normalized);
    }
}
