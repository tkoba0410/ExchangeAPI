using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerTradingApi_NotFoundTests
{
    [Fact]
    public async Task GetOrderAsync_ByAcceptanceId_NotFound_Throws()
    {
        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, privateApi, markets);
        var api = new BitflyerTradingApi(normalized);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-404");
        var call = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), key);
        var err = Assert.IsType<CallResult<OrderStatus>.Err>(call.Result);
        Assert.Contains("Order not found", err.Error.Message);
    }

    [Fact]
    public async Task GetOrderAsync_ByExchangeOrderId_NotFound_Throws()
    {
        var privateApi = new RecordingPrivateApi(Array.Empty<ChildOrderResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, privateApi, markets);
        var api = new BitflyerTradingApi(normalized);

        var key = new OrderKey(OrderIdKind.ExchangeOrderId, "JRF-404");
        var call = await api.GetOrderCallAsync(new Symbol("BTC/JPY"), key);
        var err = Assert.IsType<CallResult<OrderStatus>.Err>(call.Result);
        Assert.Contains("Order not found", err.Error.Message);

        Assert.Equal(key.Value, privateApi.LastChildOrderId);
        Assert.Null(privateApi.LastChildOrderAcceptanceId);
    }

    private sealed class RecordingPrivateApi : IBitflyerPrivateApi
    {
        private readonly FakeBitflyerPrivateApi _inner;

        public string? LastChildOrderId { get; private set; }
        public string? LastChildOrderAcceptanceId { get; private set; }

        public RecordingPrivateApi(IReadOnlyList<ChildOrderResponse> orders)
        {
            _inner = new FakeBitflyerPrivateApi(
                Array.Empty<BalanceResponse>(),
                childOrders: orders);
        }

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

        public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersCallAsync(
            GetChildOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            LastChildOrderId = request.ChildOrderId;
            LastChildOrderAcceptanceId = request.ChildOrderAcceptanceId;
            return _inner.GetChildOrdersCallAsync(request, cancellationToken);
        }

        public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersCallAsync(
            GetParentOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetParentOrdersCallAsync(request, cancellationToken);

        public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderCallAsync(
            GetParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            _inner.GetParentOrderCallAsync(request, cancellationToken);

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
