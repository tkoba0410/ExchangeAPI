using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Operations;
using ExchangeApi.Utilities.Operations;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Map;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Execute;
using ExchangeApi.Exchanges.Common.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using OrderRequest = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests.OrderRequest;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Orchestration;

internal sealed class PrivateFlow
{
    private static readonly string OpPlaceOrder = OperationNameBuilder.WithExchange("Bittrade", ContractOperations.Trading.PlaceOrder);
    private static readonly string OpCancelOrder = OperationNameBuilder.WithExchange("Bittrade", ContractOperations.Trading.CancelOrder);
    private static readonly string OpGetBalance = OperationNameBuilder.WithExchange("Bittrade", ContractOperations.Account.GetBalance);
    private static readonly string OpGetOrders = OperationNameBuilder.WithExchange("Bittrade", ContractOperations.History.GetOrders);
    private static readonly string OpGetExecutions = OperationNameBuilder.WithExchange("Bittrade", ContractOperations.History.GetExecutions);

    private readonly NormalizedPrivateApi _normalized;

    public PrivateFlow(NormalizedPrivateApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return await NormalizedExecutor.ExecuteMapCallAsync(
                request,
                OpPlaceOrder,
                ct => _normalized.PostOrdersPlaceCallAsync(
                    new NormalizedRequests.PostOrdersPlaceRequest(
                        new OrderRequest(
                            Symbol: request.Symbol,
                            Side: request.Side,
                            OrderType: OrderType.Limit,
                            Size: request.Size,
                            Price: request.Price)),
                    ct),
                ok => new OrderLimitResponse(
                    Key: ok.Key,
                    ExchangeOrderId: ok.ExchangeOrderId,
                    AcceptanceId: ok.AcceptanceId),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return await NormalizedExecutor.ExecuteMapCallAsync(
                request,
                OpCancelOrder,
                ct => _normalized.PostOrdersSubmitCancelByOrderIdCallAsync(
                    new NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest(request.Symbol, request.OrderKey),
                    ct),
                ok => new CancelOrderResponse(ok.IsSuccess),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default) =>
        GetAccountsBalanceByAccountIdCallAsync(request, cancellationToken);

    public async Task<Call<BalanceRequest, BalanceResponse>> GetAccountsBalanceByAccountIdCallAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return await NormalizedExecutor.ExecuteMapCallAsync(
                request,
                OpGetBalance,
                ct => _normalized.GetAccountsBalanceByAccountIdCallAsync(ct),
                ok => new BalanceResponse(ContractMapper.MapBalances(ok.Items)),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return await NormalizedExecutor.ExecuteMapCallAsync(
                request,
                OpGetOrders,
                ct => _normalized.GetOpenOrdersCallAsync(
                    new ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests.GetOpenOrdersRequest(request.Symbol),
                    ct),
                ok => BuildOrderResponse(request, ok.Items),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        ExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default)
    {
        return await NormalizedExecutor.ExecuteMapCallAsync(
                request,
                OpGetExecutions,
                ct => _normalized.GetMatchResultsCallAsync(request.Symbol, request.Limit, ct),
                ok => BuildExecutionResponse(request, ok.Items),
                cancellationToken)
            .ConfigureAwait(false);
    }

    private static OrdersResponse BuildOrderResponse(
        OrdersRequest request,
        IReadOnlyList<ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.OpenOrder> orders)
    {
        var items = orders.Select(MapSnapshot).ToList();
        var (requestedLimit, appliedLimit) = GetLimits(request);
        items = items.Take(appliedLimit).ToList();
        var (returnedCount, limitClamped, completeness, reason, asOf) = BuildMeta(
            requestedLimit,
            appliedLimit,
            items.Count,
            Completeness.MayBePartial,
            PartialReason.Unknown);
        return new OrdersResponse(
            Items: items,
            HasMore: false,
            NextCursor: null,
            RequestedLimit: requestedLimit,
            AppliedLimit: appliedLimit,
            ReturnedCount: returnedCount,
            LimitClamped: limitClamped,
            Completeness: completeness,
            PartialReason: reason,
            AsOf: asOf);
    }

    private static ExecutionsPrivateResponse BuildExecutionResponse(
        ExecutionsPrivateRequest request,
        IReadOnlyList<ExecutionNormalized> executions)
    {
        var items = executions.Select(e => new ExecutionsPrivateItem(
            Timestamp: e.Timestamp,
            ExecutionId: ExecutionId.ParseOrThrow(e.OrderId.Value),
            Market: request.Symbol,
            Side: MapSide(e.Side),
            Price: new Price(e.Price),
            Size: new Size(e.Size))).ToList();

        var (requestedLimit, appliedLimit) = GetLimits(request);
        items = items.Take(appliedLimit).ToList();
        var (returnedCount, limitClamped, completeness, reason, asOf) = BuildMeta(
            requestedLimit,
            appliedLimit,
            items.Count,
            Completeness.MayBePartial,
            PartialReason.Unknown);
        return new ExecutionsPrivateResponse(
            Items: items,
            HasMore: false,
            NextCursor: null,
            RequestedLimit: requestedLimit,
            AppliedLimit: appliedLimit,
            ReturnedCount: returnedCount,
            LimitClamped: limitClamped,
            Completeness: completeness,
            PartialReason: reason,
            AsOf: asOf);
    }

    private static OrdersItem MapSnapshot(ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.OpenOrder order)
    {
        var createdAt = order.OrderedAt ?? DateTimeOffset.UtcNow;
        var orderType = order.OrderType switch
        {
            OrderType.Limit => OrdersOrderType.Limit,
            OrderType.Market => OrdersOrderType.Market,
            _ => OrdersOrderType.Unknown,
        };

        return new OrdersItem(
            CreatedAt: createdAt,
            OrderId: OrderId.ParseOrThrow(order.Key.Value),
            Market: order.Symbol,
            Side: order.Side,
            OrderType: orderType,
            Price: order.Price,
            Size: order.Size,
            Status: OrdersOrderStatus.Open);
    }

    private static (int RequestedLimit, int AppliedLimit) GetLimits(OrdersRequest request)
    {
        var requestedLimit = request.Limit ?? 1000;
        var appliedLimit = Math.Min(requestedLimit, 1000);
        return (requestedLimit, appliedLimit);
    }

    private static (int RequestedLimit, int AppliedLimit) GetLimits(ExecutionsPrivateRequest request)
    {
        var requestedLimit = request.Limit ?? 1000;
        var appliedLimit = Math.Min(requestedLimit, 1000);
        return (requestedLimit, appliedLimit);
    }

    private static (int ReturnedCount, bool LimitClamped, Completeness Completeness, PartialReason? PartialReason, DateTimeOffset AsOf) BuildMeta(
        int requestedLimit,
        int appliedLimit,
        int returnedCount,
        Completeness completeness,
        PartialReason? reason)
    {
        var clamped = appliedLimit != requestedLimit;
        return (returnedCount, clamped, completeness, reason, DateTimeOffset.UtcNow);
    }

    private static Side MapSide(object side)
    {
        var sideText = side.ToString();
        if (string.Equals(sideText, "Buy", StringComparison.OrdinalIgnoreCase))
        {
            return Side.Buy;
        }

        if (string.Equals(sideText, "Sell", StringComparison.OrdinalIgnoreCase))
        {
            return Side.Sell;
        }

        throw new InvalidOperationException($"Unsupported side: {sideText}.");
    }
}
