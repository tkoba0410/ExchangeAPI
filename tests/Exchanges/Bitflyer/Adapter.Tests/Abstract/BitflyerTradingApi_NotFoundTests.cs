using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Spec.CallCommon;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

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
        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));
        Assert.Contains("Order not found", ex.Message);
        Assert.Null(ex.InnerException);
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
        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));
        Assert.Contains("Order not found", ex.Message);
        Assert.Null(ex.InnerException);

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

        public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
            GetChildOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            LastChildOrderId = request.ChildOrderId;
            LastChildOrderAcceptanceId = request.ChildOrderAcceptanceId;
            return _inner.GetChildOrdersAsync(request, cancellationToken);
        }

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
