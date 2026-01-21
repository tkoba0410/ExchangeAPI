using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

public sealed class FakeBitflyerPrivateApi
{
    private readonly IReadOnlyList<RawPrivateModels.BalanceResponse> _response;
    private readonly IReadOnlyList<RawPrivateModels.PositionResponse> _positions;
    private readonly IReadOnlyList<RawPrivateModels.ExecutionPrivateResponse> _executions;
    private readonly RawPrivateModels.CollateralResponse _collateral;
    private readonly IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse> _childOrders;
    private readonly IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse> _parentOrders;
    private readonly IReadOnlyList<RawPrivateModels.CollateralAccount> _collateralAccounts;
    private readonly RawPrivateModels.RawJsonResponse _rawJsonList = new("[]");
    private readonly RawPrivateModels.RawJsonResponse _rawJsonObject = new("{}");
    private readonly RawPrivateModels.RawJsonResponse _tradingCommission;

    public FakeBitflyerPrivateApi(
        IReadOnlyList<RawPrivateModels.BalanceResponse> response,
        IReadOnlyList<RawPrivateModels.PositionResponse>? positions = null,
        IReadOnlyList<RawPrivateModels.ExecutionPrivateResponse>? executions = null,
        RawPrivateModels.CollateralResponse? collateral = null,
        IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>? childOrders = null,
        IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>? parentOrders = null,
        string? tradingCommissionJson = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<RawPrivateModels.PositionResponse>();
        _executions = executions ?? Array.Empty<RawPrivateModels.ExecutionPrivateResponse>();
        _collateral = collateral ?? new RawPrivateModels.CollateralResponse();
        _childOrders = childOrders ?? Array.Empty<RawPrivateModels.RawGetChildOrdersResponse>();
        _parentOrders = parentOrders ?? Array.Empty<RawPrivateModels.RawGetParentOrdersResponse>();
        _collateralAccounts = Array.Empty<RawPrivateModels.CollateralAccount>();
        _tradingCommission = new RawPrivateModels.RawJsonResponse(tradingCommissionJson ?? "{}");
    }

    public Task<Call<RawPrivateModels.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        RawPrivateModels.GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<string>)Array.Empty<string>()));

    public Task<Call<RawPrivateModels.GetBalancesRequest, IReadOnlyList<RawPrivateModels.BalanceResponse>>> GetBalanceCallAsync(
        RawPrivateModels.GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<RawPrivateModels.GetPositionsRequest, IReadOnlyList<RawPrivateModels.PositionResponse>>> GetPositionsCallAsync(
        RawPrivateModels.GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _positions));

    public Task<Call<RawPrivateModels.GetAccountExecutionsRequest, IReadOnlyList<RawPrivateModels.ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        RawPrivateModels.GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _executions));

    public Task<Call<RawPrivateModels.GetCollateralRequest, RawPrivateModels.CollateralResponse>> GetCollateralCallAsync(
        RawPrivateModels.GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateral));

    public Task<Call<RawPrivateModels.GetCollateralAccountsRequest, IReadOnlyList<RawPrivateModels.CollateralAccount>>> GetCollateralAccountsCallAsync(
        RawPrivateModels.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateralAccounts));

    public Task<Call<RawPrivateModels.GetChildOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        RawPrivateModels.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(request.ChildOrderAcceptanceId))
        {
            var filtered = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == request.ChildOrderAcceptanceId)
                .ToArray();
            return Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>)filtered));
        }

        return Task.FromResult(MakeOkCall(request, _childOrders));
    }

    public Task<Call<RawPrivateModels.GetParentOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        RawPrivateModels.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _parentOrders));

    public Task<Call<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.RawGetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateModels.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPrivateModels.RawGetParentOrderResponse()));

    public Task<Call<RawPrivateModels.GetBalanceHistoryRequest, RawPrivateModels.RawJsonResponse>> GetBalanceHistoryCallAsync(
        RawPrivateModels.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetCollateralHistoryRequest, RawPrivateModels.RawJsonResponse>> GetCollateralHistoryCallAsync(
        RawPrivateModels.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetTradingCommissionRequest, RawPrivateModels.RawJsonResponse>> GetTradingCommissionCallAsync(
        RawPrivateModels.GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _tradingCommission));

    public Task<Call<RawPrivateModels.GetAddressesRequest, RawPrivateModels.RawJsonResponse>> GetAddressesCallAsync(
        RawPrivateModels.GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetCoinInsRequest, RawPrivateModels.RawJsonResponse>> GetCoinInsCallAsync(
        RawPrivateModels.GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetCoinOutsRequest, RawPrivateModels.RawJsonResponse>> GetCoinOutsCallAsync(
        RawPrivateModels.GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetDepositsRequest, RawPrivateModels.RawJsonResponse>> GetDepositsCallAsync(
        RawPrivateModels.GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetWithdrawalsRequest, RawPrivateModels.RawJsonResponse>> GetWithdrawalsCallAsync(
        RawPrivateModels.GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<RawPrivateModels.GetBankAccountsRequest, RawPrivateModels.RawJsonResponse>> GetBankAccountsCallAsync(
        RawPrivateModels.GetBankAccountsRequest request,
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
