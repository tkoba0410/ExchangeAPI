using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

internal sealed class BitflyerSpotHistoryApi
{
    private readonly IBitflyerNormalizedApi _normalized;

    public BitflyerSpotHistoryApi(
        IBitflyerNormalizedApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetChildOrdersCallAsync(request.Market, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.History.GetOrders,
                ok => BuildOrderResponse(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrdersRequest, GetOrdersResponse>(
                request,
                startedAt,
                BitflyerOperations.History.GetOrders,
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
            var call = await _normalized.GetExecutionsPrivateCallAsync(request.Market, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.History.GetExecutions,
                ok => BuildExecutionResponse(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>(
                request,
                startedAt,
                BitflyerOperations.History.GetExecutions,
                ex);
        }
    }

    private static GetOrdersResponse BuildOrderResponse(
        GetOrdersRequest request,
        IReadOnlyList<BitflyerOpenOrder> orders)
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
        IReadOnlyList<BitflyerExecutionAccountNormalized> executions)
    {
        var items = executions.Select(e => new GetExecutionsPrivateItem(
            Timestamp: e.ExecutedAt,
            ExecutionId: ExecutionId.ParseOrThrow(e.OrderId.ToString()),
            Market: e.Symbol,
            Side: e.Side,
            Price: e.Price,
            Size: e.Size)).ToList();

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

    private static GetOrdersItem MapSnapshot(BitflyerOpenOrder order)
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
