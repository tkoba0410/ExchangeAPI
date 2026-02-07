using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

public sealed class FakeBitflyerPrivateApi
{
    private readonly IReadOnlyList<RawPrivateDtos.BalanceResponse> _response;
    private readonly IReadOnlyList<RawPrivateDtos.PositionResponse> _positions;
    private readonly IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse> _executions;
    private readonly RawPrivateDtos.GetCollateralResponse _collateral;
    private readonly IReadOnlyList<RawPrivateDtos.GetChildOrdersItem> _childOrders;
    private readonly IReadOnlyList<RawPrivateDtos.GetParentOrdersItem> _parentOrders;
    private readonly IReadOnlyList<RawPrivateDtos.CollateralAccount> _collateralAccounts;
    private readonly RawPrivateDtos.RawJsonResponse _rawJsonList = new("[]");
    private readonly RawPrivateDtos.RawJsonResponse _rawJsonObject = new("{}");
    private readonly RawPrivateDtos.RawJsonResponse _tradingCommission;

    public FakeBitflyerPrivateApi(
        IReadOnlyList<RawPrivateDtos.BalanceResponse> response,
        IReadOnlyList<RawPrivateDtos.PositionResponse>? positions = null,
        IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse>? executions = null,
        RawPrivateDtos.GetCollateralResponse? collateral = null,
        IReadOnlyList<RawPrivateDtos.GetChildOrdersItem>? childOrders = null,
        IReadOnlyList<RawPrivateDtos.GetParentOrdersItem>? parentOrders = null,
        string? tradingCommissionJson = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<RawPrivateDtos.PositionResponse>();
        _executions = executions ?? Array.Empty<RawPrivateDtos.ExecutionPrivateResponse>();
        _collateral = collateral ?? new RawPrivateDtos.GetCollateralResponse();
        _childOrders = childOrders ?? Array.Empty<RawPrivateDtos.GetChildOrdersItem>();
        _parentOrders = parentOrders ?? Array.Empty<RawPrivateDtos.GetParentOrdersItem>();
        _collateralAccounts = Array.Empty<RawPrivateDtos.CollateralAccount>();
        _tradingCommission = new RawPrivateDtos.RawJsonResponse(tradingCommissionJson ?? "{}");
    }

    public Task<Call<RawPrivateRequests.GetPermissionsRequest, RawPrivateDtos.GetPermissionsResponse>> GetPermissionsCallAsync(
        RawPrivateRequests.GetPermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetPermissionsResponse();
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetBalanceRequest, RawPrivateDtos.GetBalanceResponse>> GetBalanceCallAsync(
        RawPrivateRequests.GetBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetBalanceResponse();
        response.AddRange(_response);
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetPositionsRequest, RawPrivateDtos.GetPositionsResponse>> GetPositionsCallAsync(
        RawPrivateRequests.GetPositionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetPositionsResponse();
        response.AddRange(_positions);
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetExecutionsPrivateRequest, RawPrivateDtos.GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        RawPrivateRequests.GetExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetExecutionsPrivateResponse();
        response.AddRange(_executions);
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetCollateralRequest, RawPrivateDtos.GetCollateralResponse>> GetCollateralCallAsync(
        RawPrivateRequests.GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateral));

    public Task<Call<RawPrivateRequests.GetCollateralAccountsRequest, RawPrivateDtos.GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        RawPrivateRequests.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetCollateralAccountsResponse();
        response.AddRange(_collateralAccounts);
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetChildOrdersRequest, RawPrivateDtos.GetChildOrdersResponse>> GetChildOrdersCallAsync(
        RawPrivateRequests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ChildOrderAcceptanceId is { IsEmpty: false })
        {
            var filtered = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == request.ChildOrderAcceptanceId.Value.Value)
                .ToArray();
            var response = new RawPrivateDtos.GetChildOrdersResponse();
            response.AddRange(filtered);
            return Task.FromResult(MakeOkCall(request, response));
        }

        var all = new RawPrivateDtos.GetChildOrdersResponse();
        all.AddRange(_childOrders);
        return Task.FromResult(MakeOkCall(request, all));
    }

    public Task<Call<RawPrivateRequests.GetParentOrdersRequest, RawPrivateDtos.GetParentOrdersResponse>> GetParentOrdersCallAsync(
        RawPrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetParentOrdersResponse();
        response.AddRange(_parentOrders);
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetParentOrderRequest, RawPrivateDtos.GetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetParentOrderResponse()));

    public Task<Call<RawPrivateRequests.GetBalanceHistoryRequest, RawPrivateDtos.GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        RawPrivateRequests.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetBalanceHistoryResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetCollateralHistoryRequest, RawPrivateDtos.GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        RawPrivateRequests.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetCollateralHistoryResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetTradingCommissionRequest, RawPrivateDtos.GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        RawPrivateRequests.GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetTradingCommissionResponse(_tradingCommission.RawJson)));

    public Task<Call<RawPrivateRequests.GetAddressesRequest, RawPrivateDtos.GetAddressesResponse>> GetAddressesCallAsync(
        RawPrivateRequests.GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetAddressesResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetCoinInsRequest, RawPrivateDtos.GetCoinInsResponse>> GetCoinInsCallAsync(
        RawPrivateRequests.GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetCoinInsResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetCoinOutsRequest, RawPrivateDtos.GetCoinOutsResponse>> GetCoinOutsCallAsync(
        RawPrivateRequests.GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetCoinOutsResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetDepositsRequest, RawPrivateDtos.GetDepositsResponse>> GetDepositsCallAsync(
        RawPrivateRequests.GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetDepositsResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetWithdrawalsRequest, RawPrivateDtos.GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        RawPrivateRequests.GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetWithdrawalsResponse(_rawJsonList.RawJson)));

    public Task<Call<RawPrivateRequests.GetBankAccountsRequest, RawPrivateDtos.GetBankAccountsResponse>> GetBankAccountsCallAsync(
        RawPrivateRequests.GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.GetBankAccountsResponse(_rawJsonList.RawJson)));

    private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
    {
        var meta = CallMeta.CreateInternal("Raw", "FakeBitflyerPrivateApi");
        return new Call<TReq, TResponse>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Ok(response),
            Meta: meta);
    }
}
