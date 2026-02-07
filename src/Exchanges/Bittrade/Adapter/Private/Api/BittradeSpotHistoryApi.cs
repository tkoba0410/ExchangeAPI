using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

internal sealed class BittradeSpotHistoryApi
{
    private readonly BittradeNormalizedPrivateApi _trading;

    public BittradeSpotHistoryApi(
        BittradeNormalizedPrivateApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

    public async Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOpenOrdersCallAsync(request.Market, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.History.GetOrders,
                ok => BuildOrderResponse(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<OrdersRequest, OrdersResponse>(
                request,
                startedAt,
                BittradeOperations.History.GetOrders,
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
            var call = await _trading
                .GetMatchResultsCallAsync(request.Market, request.Limit, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.History.GetExecutions,
                ok => BuildExecutionResponse(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<ExecutionsPrivateRequest, ExecutionsPrivateResponse>(
                request,
                startedAt,
                BittradeOperations.History.GetExecutions,
                ex);
        }
    }

    private static OrdersResponse BuildOrderResponse(
        OrdersRequest request,
        IReadOnlyList<ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.BittradeOpenOrder> orders)
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
        IReadOnlyList<BittradeExecutionNormalized> executions)
    {
        var items = executions.Select(e => new ExecutionsPrivateItem(
            Timestamp: e.Timestamp,
            ExecutionId: ExecutionId.ParseOrThrow(e.Id.Value),
            Market: request.Market,
            Side: e.Side switch
            {
                BittradeOrderSide.Buy => Side.Buy,
                BittradeOrderSide.Sell => Side.Sell,
                _ => throw new InvalidOperationException($"Unsupported side: {e.Side}.")
            },
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

    private static OrdersItem MapSnapshot(ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.BittradeOpenOrder order)
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
