using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal.Encoding;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Contracts.Common.Dtos;
using Xunit;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerOrderKeyConnectivityTests
{
    [Fact]
    public async Task OrderResultKey_CanGetAndCancel()
    {
        var acceptanceId = "ACCEPT-1";
        var childOrders = new[]
        {
            new RawPrivateDtos.RawGetChildOrdersResponse
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

        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse
        {
            ChildOrderAcceptanceId = acceptanceId
        }, childOrders);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, markets, privateApi);
        var api = new BitflyerTradingApi(normalized);

        var resultCall = await api.PlaceMarketOrderCallAsync(new Symbol("BTC/JPY"), ContractSide.Buy, new Size(0.01m));
        var result = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<OrderResult>.Ok>(resultCall.Result).Response;
        var statusCall = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), result.Key);
        var status = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<OrderStatus>.Ok>(statusCall.Result).Response;
        var cancelCall = await api.CancelOrderCallAsync(new Symbol("BTC/JPY"), result.Key);
        Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<CancelResult>.Ok>(cancelCall.Result);

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal(acceptanceId, result.Key.Value);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal(acceptanceId, status.Key.Value);
        Assert.Equal(acceptanceId, tradingApi.LastCancelRequest!.ChildOrderAcceptanceId);
    }

}
