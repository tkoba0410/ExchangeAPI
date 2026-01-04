using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
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
    private readonly IReadOnlyList<JsonElement> _genericList = Array.Empty<JsonElement>();
    private readonly IReadOnlyList<ParentOrderResponse> _parentOrders = Array.Empty<ParentOrderResponse>();

    public FakeBitflyerPrivateApi(
        IReadOnlyList<BalanceResponse> response,
        IReadOnlyList<PositionResponse>? positions = null,
        IReadOnlyList<ExecutionPrivateResponse>? executions = null,
        CollateralResponse? collateral = null,
        IReadOnlyList<ChildOrderResponse>? childOrders = null,
        IReadOnlyList<ParentOrderResponse>? parentOrders = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<PositionResponse>();
        _executions = executions ?? Array.Empty<ExecutionPrivateResponse>();
        _collateral = collateral ?? new CollateralResponse();
        _childOrders = childOrders ?? Array.Empty<ChildOrderResponse>();
        _collateralAccounts = Array.Empty<CollateralAccount>();
        _parentOrders = parentOrders ?? Array.Empty<ParentOrderResponse>();
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
        Task.FromResult(MakeOkCall(request, _parentOrders));

    public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new ParentOrderDetailResponse
        {
            Id = 1,
            ParentOrderId = request.ParentOrderId ?? "PARENT-ID",
            OrderMethod = "SIMPLE",
            ExpireDate = DateTimeOffset.UtcNow,
            TimeInForce = "GTC",
            ParentOrderAcceptanceId = request.ParentOrderAcceptanceId ?? "PARENT-ACCEPT",
            Parameters = new[]
            {
                new ParentOrderDetailParameter
                {
                    ProductCode = request.ProductCode,
                    ConditionType = "LIMIT",
                    Side = "BUY",
                    Size = 0.1m,
                    Price = 30000m,
                    TriggerPrice = 0m,
                    Offset = 0m,
                }
            }
        };
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<GetBalanceHistoryRequest, IReadOnlyList<JsonElement>>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetCollateralHistoryRequest, IReadOnlyList<JsonElement>>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetTradingCommissionRequest, JsonElement>> GetTradingCommissionAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, JsonDocument.Parse("{}").RootElement));

    public Task<Call<GetAddressesRequest, IReadOnlyList<JsonElement>>> GetAddressesAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetCoinInsRequest, IReadOnlyList<JsonElement>>> GetCoinInsAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetCoinOutsRequest, IReadOnlyList<JsonElement>>> GetCoinOutsAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetDepositsRequest, IReadOnlyList<JsonElement>>> GetDepositsAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetWithdrawalsRequest, IReadOnlyList<JsonElement>>> GetWithdrawalsAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

    public Task<Call<GetBankAccountsRequest, IReadOnlyList<JsonElement>>> GetBankAccountsAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _genericList));

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
