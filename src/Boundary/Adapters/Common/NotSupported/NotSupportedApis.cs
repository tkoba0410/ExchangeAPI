using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Boundary.Adapters.Common.NotSupported;

internal sealed class NotSupportedTradingApi : ITradingApi
{
    private readonly ExchangeCode _exchange;

    public NotSupportedTradingApi(ExchangeCode exchange) => _exchange = exchange;

    public Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<PlaceLimitOrderRequest, OrderResult>(new PlaceLimitOrderRequest(symbol, side, size, price)));

    public Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<PlaceMarketOrderRequest, OrderResult>(new PlaceMarketOrderRequest(symbol, side, size)));

    public Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<PlaceStopOrderRequest, OrderResult>(new PlaceStopOrderRequest(symbol, side, size, triggerPrice)));

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<CancelOrderRequest, CancelResult>(new CancelOrderRequest(symbol, orderKey)));

    public Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOrdersRequest, IReadOnlyList<OpenOrder>>(new GetOrdersRequest(symbol)));

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOrderRequest, OrderStatus>(new GetOrderRequest(symbol, orderKey)));

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>> GetParentOrdersCallAsync(
        Symbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>(
            new GetParentOrdersRequest(symbol, parentOrderId, parentOrderAcceptanceId)));

    public Task<Call<GetParentOrderRequest, ParentOrderDetail>> GetParentOrderCallAsync(
        Symbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetParentOrderRequest, ParentOrderDetail>(
            new GetParentOrderRequest(symbol, parentOrderId, parentOrderAcceptanceId)));

    private Call<TReq, TOk> NotSupportedCall<TReq, TOk>(TReq request)
    {
        var now = System.DateTimeOffset.UtcNow;
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: "NotSupported",
            Tags: null,
            Children: null);
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: System.TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Semantic, "Feature not supported.")),
            Meta: meta);
    }
}

internal sealed class NotSupportedAccountApi : IAccountApi
{
    private readonly ExchangeCode _exchange;

    public NotSupportedAccountApi(ExchangeCode exchange) => _exchange = exchange;

    public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetBalancesRequest, IReadOnlyList<Balance>>(new GetBalancesRequest()));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(new GetAccountExecutionsRequest(symbol)));

    private Call<TReq, TOk> NotSupportedCall<TReq, TOk>(TReq request)
    {
        var now = System.DateTimeOffset.UtcNow;
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: "NotSupported",
            Tags: null,
            Children: null);
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: System.TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Semantic, "Feature not supported.")),
            Meta: meta);
    }
}
