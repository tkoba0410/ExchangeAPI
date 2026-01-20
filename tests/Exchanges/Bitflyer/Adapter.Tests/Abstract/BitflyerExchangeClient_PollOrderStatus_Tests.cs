using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Application.Interfaces;
using ExchangeApi.Application.Trading;
using ExchangeApi.Composition.Adapters.Application;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Application.UseCases;
using ExecutionResponse = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models.ExecutionPrivateResponse;
using Xunit;
using RawTicker = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models.Ticker;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerExchangeClient_PollOrderStatus_Tests
{
    [Fact]
    public async Task WaitForOrderAsync_CompletesWhenStateTransitions()
    {
        var acceptanceId = "ACCEPT-1";
        var active = new ChildOrderResponse
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
        var completed = new ChildOrderResponse
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

        var publicApi = new FakeBitflyerPublicApi(new RawTicker());
        var sequenceApi = new SequenceChildOrderApi(new[] { active }, new[] { completed });
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var client = CreateClient(publicApi, sequenceApi, tradingApi);

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

    private sealed class SequenceChildOrderApi : IBitflyerPrivateApi
    {
        private readonly Queue<IReadOnlyList<ChildOrderResponse>> _queue;
        private readonly FakeBitflyerPrivateApi _inner;

        public SequenceChildOrderApi(params IReadOnlyList<ChildOrderResponse>[] snapshots)
        {
            _queue = new Queue<IReadOnlyList<ChildOrderResponse>>(snapshots);
            _inner = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        }

        public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersCallAsync(
            GetChildOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ChildOrderResponse> snapshot = _queue.Count == 0
                ? Array.Empty<ChildOrderResponse>()
                : _queue.Dequeue();

            var meta = CallMeta.CreateInternal("Raw", "SequenceChildOrderApi");
            var call = new Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<IReadOnlyList<ChildOrderResponse>>.Ok(snapshot),
                Meta: meta);
            return Task.FromResult(call);
        }

        public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersCallAsync(
            GetParentOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetParentOrdersCallAsync(request, cancellationToken);

        public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderCallAsync(
            GetParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetParentOrderCallAsync(request, cancellationToken);

        public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
            GetPermissionsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetPermissionsCallAsync(request, cancellationToken);

        public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalanceCallAsync(
            GetBalancesRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetBalanceCallAsync(request, cancellationToken);

        public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
            GetAccountExecutionsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetExecutionsPrivateCallAsync(request, cancellationToken);

        public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsCallAsync(
            GetPositionsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetPositionsCallAsync(request, cancellationToken);

        public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralCallAsync(
            GetCollateralRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCollateralCallAsync(request, cancellationToken);

        public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsCallAsync(
            GetCollateralAccountsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCollateralAccountsCallAsync(request, cancellationToken);

        public Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryCallAsync(
            GetBalanceHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetBalanceHistoryCallAsync(request, cancellationToken);

        public Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryCallAsync(
            GetCollateralHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCollateralHistoryCallAsync(request, cancellationToken);

        public Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionCallAsync(
            GetTradingCommissionRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetTradingCommissionCallAsync(request, cancellationToken);

        public Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesCallAsync(
            GetAddressesRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetAddressesCallAsync(request, cancellationToken);

        public Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsCallAsync(
            GetCoinInsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCoinInsCallAsync(request, cancellationToken);

        public Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsCallAsync(
            GetCoinOutsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCoinOutsCallAsync(request, cancellationToken);

        public Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsCallAsync(
            GetDepositsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetDepositsCallAsync(request, cancellationToken);

        public Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsCallAsync(
            GetWithdrawalsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetWithdrawalsCallAsync(request, cancellationToken);

        public Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsCallAsync(
            GetBankAccountsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetBankAccountsCallAsync(request, cancellationToken);
    }
}
