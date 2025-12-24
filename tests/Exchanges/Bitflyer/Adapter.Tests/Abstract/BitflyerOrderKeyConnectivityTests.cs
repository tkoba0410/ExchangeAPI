using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Services;
using ExchangeApi.Common.Dtos;
using Xunit;
using ContractSide = ExchangeApi.Common.Enums.Side;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;

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
                ProductCode = ProductCode.BtcJpy,
                Side = RawSide.Buy,
                ChildOrderType = ChildOrderType.Limit,
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
        var api = new BitflyerTradingApi(tradingApi, privateApi, CreateResolver());

        var result = await api.PlaceMarketOrderAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var status = await api.GetOrderAsync(new Symbol("BTC/JPY"), result.Key);
        await api.CancelOrderAsync(new Symbol("BTC/JPY"), result.Key);

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal(acceptanceId, result.Key.Value);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal(acceptanceId, status.Key.Value);
        Assert.Equal(acceptanceId, tradingApi.LastCancelRequest!.ChildOrderAcceptanceId);
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
                ProductCode = ProductCode.BtcJpy,
                Side = RawSide.Buy,
                ChildOrderType = ChildOrderType.Limit,
                Size = 0.01m,
                ExecutedSize = 0m,
                OutstandingSize = 0.01m
            }
        };

        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>(), childOrders: childOrders);
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var api = new BitflyerTradingApi(tradingApi, privateApi, CreateResolver());

        var openOrders = await api.GetOrdersAsync(new Symbol("BTC/JPY"));
        var key = openOrders[0].Key;

        var status = await api.GetOrderAsync(new Symbol("BTC/JPY"), key);
        await api.CancelOrderAsync(new Symbol("BTC/JPY"), key);

        Assert.Equal(OrderIdKind.AcceptanceId, key.Kind);
        Assert.Equal(acceptanceId, key.Value);
        Assert.Equal(acceptanceId, tradingApi.LastCancelRequest!.ChildOrderAcceptanceId);
        Assert.Equal(acceptanceId, status.Key.Value);
    }

    private static IExchangeMarketResolver CreateResolver() =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(
            new[] { new ExchangeMarketInfo("BTC/JPY", "BTC_JPY", "Spot") },
            null,
            null,
            null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoApi
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_info);
    }
}
