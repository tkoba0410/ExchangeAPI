using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Boundary.Adapters.Common.ApiCallMapping;

namespace ExchangeApi.Boundary.Adapters.Common.NotSupported;

internal sealed class NotSupportedTradingApi : ITradingApi
{
    private readonly ExchangeCode _exchange;

    public NotSupportedTradingApi(ExchangeCode exchange) => _exchange = exchange;

    private ExchangeFeatureNotSupportedException NotSupported(string feature) => new(_exchange, feature);

    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<ApiCall<PlaceLimitOrderRequest, OrderResult, ApiError>> PlaceLimitOrderCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<PlaceLimitOrderRequest, OrderResult>(new PlaceLimitOrderRequest(symbol, side, size, price)));

    public Task<ApiCall<PlaceMarketOrderRequest, OrderResult, ApiError>> PlaceMarketOrderCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<PlaceMarketOrderRequest, OrderResult>(new PlaceMarketOrderRequest(symbol, side, size)));

    public Task<ApiCall<PlaceStopOrderRequest, OrderResult, ApiError>> PlaceStopOrderCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<PlaceStopOrderRequest, OrderResult>(new PlaceStopOrderRequest(symbol, side, size, triggerPrice)));

    public Task<ApiCall<CancelOrderRequest, CancelResult, ApiError>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<CancelOrderRequest, CancelResult>(new CancelOrderRequest(symbol, orderKey)));

    public Task<ApiCall<GetOrdersRequest, IReadOnlyList<OpenOrder>, ApiError>> GetOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOrdersRequest, IReadOnlyList<OpenOrder>>(new GetOrdersRequest(symbol)));

    public Task<ApiCall<GetOrderRequest, OrderStatus, ApiError>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOrderRequest, OrderStatus>(new GetOrderRequest(symbol, orderKey)));

    private ApiCall<TReq, TOk, ApiError> NotSupportedCall<TReq, TOk>(TReq request)
    {
        var meta = ApiCallMapperBase.ToMeta(System.DateTimeOffset.UtcNow);
        return ApiCallMapperBase.Err<TReq, TOk>(_exchange, request, meta, 0, "Feature not supported.");
    }
}

internal sealed class NotSupportedAccountApi : IAccountApi
{
    private readonly ExchangeCode _exchange;

    public NotSupportedAccountApi(ExchangeCode exchange) => _exchange = exchange;

    private ExchangeFeatureNotSupportedException NotSupported(string feature) => new(_exchange, feature);

    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        throw NotSupported("Account");

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Account");

    public Task<ApiCall<GetBalancesRequest, IReadOnlyList<Balance>, ApiError>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetBalancesRequest, IReadOnlyList<Balance>>(new GetBalancesRequest()));

    public Task<ApiCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>, ApiError>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(new GetAccountExecutionsRequest(symbol)));

    private ApiCall<TReq, TOk, ApiError> NotSupportedCall<TReq, TOk>(TReq request)
    {
        var meta = ApiCallMapperBase.ToMeta(System.DateTimeOffset.UtcNow);
        return ApiCallMapperBase.Err<TReq, TOk>(_exchange, request, meta, 0, "Feature not supported.");
    }
}
