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
    private readonly RawPrivateDtos.CollateralResponse _collateral;
    private readonly IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse> _childOrders;
    private readonly IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse> _parentOrders;
    private readonly IReadOnlyList<RawPrivateDtos.CollateralAccount> _collateralAccounts;
    private readonly RawPrivateDtos.RawJsonResponse _rawJsonList = new("[]");
    private readonly RawPrivateDtos.RawJsonResponse _rawJsonObject = new("{}");
    private readonly RawPrivateDtos.RawJsonResponse _tradingCommission;

    public FakeBitflyerPrivateApi(
        IReadOnlyList<RawPrivateDtos.BalanceResponse> response,
        IReadOnlyList<RawPrivateDtos.PositionResponse>? positions = null,
        IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse>? executions = null,
        RawPrivateDtos.CollateralResponse? collateral = null,
        IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>? childOrders = null,
        IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse>? parentOrders = null,
        string? tradingCommissionJson = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<RawPrivateDtos.PositionResponse>();
        _executions = executions ?? Array.Empty<RawPrivateDtos.ExecutionPrivateResponse>();
        _collateral = collateral ?? new RawPrivateDtos.CollateralResponse();
        _childOrders = childOrders ?? Array.Empty<RawPrivateDtos.RawGetChildOrdersResponse>();
        _parentOrders = parentOrders ?? Array.Empty<RawPrivateDtos.RawGetParentOrdersResponse>();
        _collateralAccounts = Array.Empty<RawPrivateDtos.CollateralAccount>();
        _tradingCommission = new RawPrivateDtos.RawJsonResponse(tradingCommissionJson ?? "{}");
    }

    public Task<Call<RawPrivateRequests.GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        RawPrivateRequests.GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<FreeText>)Array.Empty<FreeText>()));

    public Task<Call<RawPrivateRequests.GetBalancesRequest, IReadOnlyList<RawPrivateDtos.BalanceResponse>>> GetBalanceCallAsync(
        RawPrivateRequests.GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<RawPrivateRequests.GetPositionsRequest, IReadOnlyList<RawPrivateDtos.PositionResponse>>> GetPositionsCallAsync(
        RawPrivateRequests.GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _positions));

    public Task<Call<RawPrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        RawPrivateRequests.GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _executions));

    public Task<Call<RawPrivateRequests.GetCollateralRequest, RawPrivateDtos.CollateralResponse>> GetCollateralCallAsync(
        RawPrivateRequests.GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateral));

    public Task<Call<RawPrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<RawPrivateDtos.CollateralAccount>>> GetCollateralAccountsCallAsync(
        RawPrivateRequests.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateralAccounts));

    public Task<Call<RawPrivateRequests.GetChildOrdersRequest, IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        RawPrivateRequests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ChildOrderAcceptanceId is { IsEmpty: false })
        {
            var filtered = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == request.ChildOrderAcceptanceId.Value.Value)
                .ToArray();
            return Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>)filtered));
        }

        return Task.FromResult(MakeOkCall(request, _childOrders));
    }

    public Task<Call<RawPrivateRequests.GetParentOrdersRequest, IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        RawPrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _parentOrders));

    public Task<Call<RawPrivateRequests.GetParentOrderRequest, RawPrivateDtos.RawGetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateDtos.RawGetParentOrderResponse()));

    public Task<Call<RawPrivateRequests.GetBalanceHistoryRequest, RawPrivateDtos.RawJsonResponse>> GetBalanceHistoryCallAsync(
        RawPrivateRequests.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetCollateralHistoryRequest, RawPrivateDtos.RawJsonResponse>> GetCollateralHistoryCallAsync(
        RawPrivateRequests.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetTradingCommissionRequest, RawPrivateDtos.RawJsonResponse>> GetTradingCommissionCallAsync(
        RawPrivateRequests.GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _tradingCommission));

    public Task<Call<RawPrivateRequests.GetAddressesRequest, RawPrivateDtos.RawJsonResponse>> GetAddressesCallAsync(
        RawPrivateRequests.GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetCoinInsRequest, RawPrivateDtos.RawJsonResponse>> GetCoinInsCallAsync(
        RawPrivateRequests.GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetCoinOutsRequest, RawPrivateDtos.RawJsonResponse>> GetCoinOutsCallAsync(
        RawPrivateRequests.GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetDepositsRequest, RawPrivateDtos.RawJsonResponse>> GetDepositsCallAsync(
        RawPrivateRequests.GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetWithdrawalsRequest, RawPrivateDtos.RawJsonResponse>> GetWithdrawalsCallAsync(
        RawPrivateRequests.GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateRequests.GetBankAccountsRequest, RawPrivateDtos.RawJsonResponse>> GetBankAccountsCallAsync(
        RawPrivateRequests.GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

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
