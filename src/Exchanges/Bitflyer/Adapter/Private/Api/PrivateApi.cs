using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrderRequest = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests.OrderRequest;
using NormalizedOpenOrder = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.OpenOrder;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Utilities.Account;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

internal sealed class PrivateApi
{
    private readonly INormalizedApi _normalized;

    public PrivateApi(INormalizedApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetBalanceCallAsync(cancellationToken).ConfigureAwait(false);
            return AdapterCallMapper.MapCall(
                request,
                call,
                Operations.Account.GetBalance,
                ok => new BalanceResponse(MapBalances(ok)));
        }
        catch (Exception ex)
        {
            return AdapterCallMapper.FromException<BalanceRequest, BalanceResponse>(
                request,
                startedAt,
                Operations.Account.GetBalance,
                ex);
        }
    }

    public async Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetChildOrdersCallAsync(request.Symbol, cancellationToken).ConfigureAwait(false);
            return AdapterCallMapper.MapCall(
                request,
                call,
                Operations.History.GetOrders,
                ok => BuildOrderResponse(request, ok));
        }
        catch (Exception ex)
        {
            return AdapterCallMapper.FromException<OrdersRequest, OrdersResponse>(
                request,
                startedAt,
                Operations.History.GetOrders,
                ex);
        }
    }

    public async Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        ExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetExecutionsPrivateCallAsync(request.Symbol, cancellationToken)
                .ConfigureAwait(false);
            return AdapterCallMapper.MapCall(
                request,
                call,
                Operations.History.GetExecutions,
                ok => BuildExecutionResponse(request, ok));
        }
        catch (Exception ex)
        {
            return AdapterCallMapper.FromException<ExecutionsPrivateRequest, ExecutionsPrivateResponse>(
                request,
                startedAt,
                Operations.History.GetExecutions,
                ex);
        }
    }

    public async Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var normalizedRequest = new OrderRequest(
                Symbol: request.Symbol,
                Side: request.Side,
                OrderType: OrderType.Limit,
                Size: request.Size,
                Price: request.Price);
            var call = await _normalized
                .SendChildOrderCallAsync(normalizedRequest, cancellationToken)
                .ConfigureAwait(false);
            return AdapterCallMapper.MapCall(
                request,
                call,
                Operations.Trading.PlaceOrder,
                ok => new OrderLimitResponse(
                    Key: ok.Key,
                    ExchangeOrderId: ok.ExchangeOrderId,
                    AcceptanceId: ok.AcceptanceId));
        }
        catch (Exception ex)
        {
            return AdapterCallMapper.FromException<OrderLimitRequest, OrderLimitResponse>(
                request,
                startedAt,
                Operations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.CancelChildOrderCallAsync(request.Symbol, request.OrderKey, cancellationToken).ConfigureAwait(false);
            return AdapterCallMapper.MapCall(
                request,
                call,
                Operations.Trading.CancelOrder,
                ok => new CancelOrderResponse(ok.IsSuccess));
        }
        catch (Exception ex)
        {
            return AdapterCallMapper.FromException<CancelOrderRequest, CancelOrderResponse>(
                request,
                startedAt,
                Operations.Trading.CancelOrder,
                ex);
        }
    }

    private static IReadOnlyList<BalanceEntry> MapBalances(GetBalanceResponse balances) =>
        balances.Items
            .Select(b => BalanceFactory.Create(
                currency: b.Value.CurrencyCode,
                amount: b.Value.Amount,
                available: b.Value.Available))
            .ToArray();

    private static OrdersResponse BuildOrderResponse(
        OrdersRequest request,
        GetChildOrdersResponse orders)
    {
        var items = orders.Items.Select(x => MapSnapshot(x.Value)).ToList();
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
        GetExecutionsPrivateResponse executions)
    {
        var items = executions.Items.Select(e => new ExecutionsPrivateItem(
            Timestamp: e.Value.ExecutedAt,
            ExecutionId: ExecutionId.ParseOrThrow(e.Value.OrderId.ToString()),
            Market: e.Value.Symbol,
            Side: e.Value.Side,
            Price: e.Value.Price,
            Size: e.Value.Size)).ToList();

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

    private static OrdersItem MapSnapshot(NormalizedOpenOrder order)
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
}
