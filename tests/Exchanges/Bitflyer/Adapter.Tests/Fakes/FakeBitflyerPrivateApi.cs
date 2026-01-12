using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IReadOnlyList<BalanceResponse> _response;
    private readonly IReadOnlyList<PositionResponse> _positions;
    private readonly IReadOnlyList<ExecutionPrivateResponse> _executions;
    private readonly CollateralResponse _collateral;
    private readonly IReadOnlyList<ChildOrderResponse> _childOrders;
    private readonly IReadOnlyList<CollateralAccount> _collateralAccounts;
    private readonly RawJsonResponse _rawJsonList = new("[]");
    private readonly RawJsonResponse _rawJsonObject = new("{}");

    public FakeBitflyerPrivateApi(
        IReadOnlyList<BalanceResponse> response,
        IReadOnlyList<PositionResponse>? positions = null,
        IReadOnlyList<ExecutionPrivateResponse>? executions = null,
        CollateralResponse? collateral = null,
        IReadOnlyList<ChildOrderResponse>? childOrders = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<PositionResponse>();
        _executions = executions ?? Array.Empty<ExecutionPrivateResponse>();
        _collateral = collateral ?? new CollateralResponse();
        _childOrders = childOrders ?? Array.Empty<ChildOrderResponse>();
        _collateralAccounts = Array.Empty<CollateralAccount>();
    }

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<string>)Array.Empty<string>()));

    public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _positions));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _executions));

    public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateral));

    public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _collateralAccounts));

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(request.ChildOrderAcceptanceId))
        {
            var filtered = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == request.ChildOrderAcceptanceId)
                .ToArray();
            return Task.FromResult(MakeOkCall(request, (IReadOnlyList<ChildOrderResponse>)filtered));
        }

        return Task.FromResult(MakeOkCall(request, _childOrders));
    }

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<ParentOrderResponse>)Array.Empty<ParentOrderResponse>()));

    public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new ParentOrderDetailResponse()));

    public Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonObject));

    public Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    public Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _rawJsonList));

    private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: "FakeBitflyerPrivateApi",
            Tags: null,
            Children: null);
        return new Call<TReq, TResponse>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Ok(response),
            Meta: meta);
    }
}
