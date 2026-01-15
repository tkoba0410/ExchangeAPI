using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;

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

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<CancelOrderRequest, CancelResult>(new CancelOrderRequest(symbol, orderKey)));

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOrderRequest, OrderStatus>(new GetOrderRequest(symbol, orderKey)));

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>(
            new GetOpenOrdersRequest(symbol)));

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

internal sealed class NotSupportedSpotHistoryApi : ISpotHistoryApi
{
    public Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<MarketLimitCursorRequest, Page<OrderSnapshotItem>>(request));

    public Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<MarketLimitCursorRequest, Page<ExecutionItem>>(request));

    private static Call<TReq, TOk> NotSupportedCall<TReq, TOk>(TReq request)
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
