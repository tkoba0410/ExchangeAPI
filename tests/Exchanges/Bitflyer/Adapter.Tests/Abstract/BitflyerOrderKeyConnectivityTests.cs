using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using Xunit;
using ContractSide = ExchangeApi.Contracts.Common.DomainCommon.Enums.Side;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

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
        var result = Assert.IsType<ExchangeApi.Contracts.Common.CallCommon.CallResult<OrderResult>.Ok>(resultCall.Result).Response;
        var statusCall = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), result.Key);
        var status = Assert.IsType<ExchangeApi.Contracts.Common.CallCommon.CallResult<OrderStatus>.Ok>(statusCall.Result).Response;
        var cancelCall = await api.CancelOrderCallAsync(new Symbol("BTC/JPY"), result.Key);
        Assert.IsType<ExchangeApi.Contracts.Common.CallCommon.CallResult<CancelResult>.Ok>(cancelCall.Result);

        Assert.Equal(OrderIdKind.AcceptanceId, result.Key.Kind);
        Assert.Equal(acceptanceId, result.Key.Value);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal(acceptanceId, status.Key.Value);
        Assert.Equal(acceptanceId, tradingApi.LastCancelRequest!.Body.ChildOrderAcceptanceId);
    }

}
