using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Domain.UseCases;
using ExecutionResponse = ExchangeApi.Exchanges.Bitflyer.Raw.Private.ExecutionPrivateResponse;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

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

        var publicApi = new FakeBitflyerPublicApi(new Ticker());
        var sequenceApi = new SequenceChildOrderApi(new[] { active }, new[] { completed });
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var client = CreateClient(publicApi, sequenceApi, tradingApi);

        var status = await OrderPolling.WaitForOrderAsync(
            api: client,
            symbol: new Symbol("BTC/JPY"),
            orderKey: new OrderKey(OrderIdKind.AcceptanceId, acceptanceId),
            options: new PollingOptions(TimeSpan.FromMilliseconds(1), 5));

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
        var normalizedMargin = BitflyerTestHelpers.CreateMarginApi(accountApi, markets);
        var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, accountApi, markets);

        return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedMargin, normalizedTrading);
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

        public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
            GetChildOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ChildOrderResponse> snapshot = _queue.Count == 0
                ? Array.Empty<ChildOrderResponse>()
                : _queue.Dequeue();

            var meta = new CallMeta(
                Layer: "Raw",
                Component: "SequenceChildOrderApi",
                Tags: null,
                Children: null);
            var call = new Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<IReadOnlyList<ChildOrderResponse>>.Ok(snapshot),
                Meta: meta);
            return Task.FromResult(call);
        }

        public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(
            GetPermissionsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetPermissionsAsync(request, cancellationToken);

        public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
            GetBalancesRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetBalancesAsync(request, cancellationToken);

        public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
            GetAccountExecutionsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetExecutionsAsync(request, cancellationToken);

        public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsAsync(
            GetPositionsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetPositionsAsync(request, cancellationToken);

        public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralAsync(
            GetCollateralRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCollateralAsync(request, cancellationToken);

        public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsAsync(
            GetCollateralAccountsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCollateralAccountsAsync(request, cancellationToken);

        public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersAsync(
            GetParentOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetParentOrdersAsync(request, cancellationToken);

        public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderAsync(
            GetParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetParentOrderAsync(request, cancellationToken);

        public Task<Call<GetBalanceHistoryRequest, IReadOnlyList<JsonElement>>> GetBalanceHistoryAsync(
            GetBalanceHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetBalanceHistoryAsync(request, cancellationToken);

        public Task<Call<GetCollateralHistoryRequest, IReadOnlyList<JsonElement>>> GetCollateralHistoryAsync(
            GetCollateralHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCollateralHistoryAsync(request, cancellationToken);

        public Task<Call<GetTradingCommissionRequest, JsonElement>> GetTradingCommissionAsync(
            GetTradingCommissionRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetTradingCommissionAsync(request, cancellationToken);

        public Task<Call<GetAddressesRequest, IReadOnlyList<JsonElement>>> GetAddressesAsync(
            GetAddressesRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetAddressesAsync(request, cancellationToken);

        public Task<Call<GetCoinInsRequest, IReadOnlyList<JsonElement>>> GetCoinInsAsync(
            GetCoinInsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCoinInsAsync(request, cancellationToken);

        public Task<Call<GetCoinOutsRequest, IReadOnlyList<JsonElement>>> GetCoinOutsAsync(
            GetCoinOutsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetCoinOutsAsync(request, cancellationToken);

        public Task<Call<GetDepositsRequest, IReadOnlyList<JsonElement>>> GetDepositsAsync(
            GetDepositsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetDepositsAsync(request, cancellationToken);

        public Task<Call<GetWithdrawalsRequest, IReadOnlyList<JsonElement>>> GetWithdrawalsAsync(
            GetWithdrawalsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetWithdrawalsAsync(request, cancellationToken);

        public Task<Call<GetBankAccountsRequest, IReadOnlyList<JsonElement>>> GetBankAccountsAsync(
            GetBankAccountsRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetBankAccountsAsync(request, cancellationToken);
    }
}
