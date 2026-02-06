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
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;

internal sealed class BittradeSpotHistoryApi
{
    private readonly BittradeNormalizedPrivateApi _trading;

    public BittradeSpotHistoryApi(
        BittradeNormalizedPrivateApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

    public async Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
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
            return ApiCallMapper.FromException<GetOrdersRequest, GetOrdersResponse>(
                request,
                startedAt,
                BittradeOperations.History.GetOrders,
                ex);
        }
    }

    public async Task<Call<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        GetExecutionsPrivateRequest request,
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
            return ApiCallMapper.FromException<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>(
                request,
                startedAt,
                BittradeOperations.History.GetExecutions,
                ex);
        }
    }

    private static GetOrdersResponse BuildOrderResponse(
        GetOrdersRequest request,
        IReadOnlyList<ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos.BittradeOpenOrder> orders)
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
        return new GetOrdersResponse(
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

    private static GetExecutionsPrivateResponse BuildExecutionResponse(
        GetExecutionsPrivateRequest request,
        IReadOnlyList<BittradeExecutionNormalized> executions)
    {
        var items = executions.Select(e => new GetExecutionsPrivateItem(
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
        return new GetExecutionsPrivateResponse(
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

    private static GetOrdersItem MapSnapshot(ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos.BittradeOpenOrder order)
    {
        var createdAt = order.OrderedAt ?? DateTimeOffset.UtcNow;
        var orderType = order.OrderType switch
        {
            OrderType.Limit => GetOrdersOrderType.Limit,
            OrderType.Market => GetOrdersOrderType.Market,
            _ => GetOrdersOrderType.Unknown,
        };

        return new GetOrdersItem(
            CreatedAt: createdAt,
            OrderId: OrderId.ParseOrThrow(order.Key.Value),
            Market: order.Symbol,
            Side: order.Side,
            OrderType: orderType,
            Price: order.Price,
            Size: order.Size,
            Status: GetOrdersOrderStatus.Open);
    }

    private static (int RequestedLimit, int AppliedLimit) GetLimits(GetOrdersRequest request)
    {
        var requestedLimit = request.Limit ?? 1000;
        var appliedLimit = Math.Min(requestedLimit, 1000);
        return (requestedLimit, appliedLimit);
    }

    private static (int RequestedLimit, int AppliedLimit) GetLimits(GetExecutionsPrivateRequest request)
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
