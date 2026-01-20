using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.Trading;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.History;

internal sealed class BitflyerSpotHistoryApi : ISpotHistoryApi
{
    private readonly IBitflyerNormalizedTradingApi _trading;
    private readonly IBitflyerNormalizedAccountApi _account;

    public BitflyerSpotHistoryApi(
        IBitflyerNormalizedTradingApi trading,
        IBitflyerNormalizedAccountApi account)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _account = account ?? throw new ArgumentNullException(nameof(account));
    }

    public async Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetChildOrdersCallAsync(request.Market, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.History.GetOrders,
                ok => BuildOrderPage(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<MarketLimitCursorRequest, Page<OrderSnapshotItem>>(
                request,
                startedAt,
                BitflyerOperations.History.GetOrders,
                ex);
        }
    }

    public async Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _account.GetExecutionsPrivateCallAsync(request.Market, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.History.GetExecutions,
                ok => BuildExecutionPage(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<MarketLimitCursorRequest, Page<ExecutionItem>>(
                request,
                startedAt,
                BitflyerOperations.History.GetExecutions,
                ex);
        }
    }

    private static Page<OrderSnapshotItem> BuildOrderPage(
        MarketLimitCursorRequest request,
        IReadOnlyList<BitflyerOpenOrder> orders)
    {
        var items = orders.Select(MapSnapshot).ToList();
        var (requestedLimit, appliedLimit) = GetLimits(request);
        items = items.Take(appliedLimit).ToList();
        var meta = BuildMeta(requestedLimit, appliedLimit, items.Count, Completeness.MayBePartial, PartialReason.Unknown);
        return new Page<OrderSnapshotItem>(items, HasMore: false, NextCursor: null, Meta: meta);
    }

    private static Page<ExecutionItem> BuildExecutionPage(
        MarketLimitCursorRequest request,
        IReadOnlyList<BitflyerExecutionAccountNormalized> executions)
    {
        var items = executions.Select(e => new ExecutionItem(
            Timestamp: e.ExecutedAt,
            ExecutionId: e.OrderId,
            Market: e.Symbol,
            Side: e.Side,
            Price: e.Price,
            Size: e.Size)).ToList();

        var (requestedLimit, appliedLimit) = GetLimits(request);
        items = items.Take(appliedLimit).ToList();
        var meta = BuildMeta(requestedLimit, appliedLimit, items.Count, Completeness.MayBePartial, PartialReason.Unknown);
        return new Page<ExecutionItem>(items, HasMore: false, NextCursor: null, Meta: meta);
    }

    private static OrderSnapshotItem MapSnapshot(BitflyerOpenOrder order)
    {
        var createdAt = order.OrderedAt ?? DateTimeOffset.UtcNow;
        var orderType = order.OrderType switch
        {
            OrderType.Limit => OrderSnapshotType.Limit,
            OrderType.Market => OrderSnapshotType.Market,
            _ => OrderSnapshotType.Unknown,
        };

        return new OrderSnapshotItem(
            CreatedAt: createdAt,
            OrderId: order.Key.Value,
            Market: order.Symbol,
            Side: order.Side,
            OrderType: orderType,
            Price: order.Price,
            Size: order.Size,
            Status: OrderSnapshotStatus.Open);
    }

    private static (int RequestedLimit, int AppliedLimit) GetLimits(MarketLimitCursorRequest request)
    {
        var requestedLimit = request.Limit ?? 1000;
        var appliedLimit = Math.Min(requestedLimit, 1000);
        return (requestedLimit, appliedLimit);
    }

    private static PageMeta BuildMeta(
        int requestedLimit,
        int appliedLimit,
        int returnedCount,
        Completeness completeness,
        PartialReason? reason)
    {
        var clamped = appliedLimit != requestedLimit;

        return new PageMeta(
            RequestedLimit: requestedLimit,
            AppliedLimit: appliedLimit,
            ReturnedCount: returnedCount,
            LimitClamped: clamped,
            Completeness: completeness,
            PartialReason: reason,
            AsOf: DateTimeOffset.UtcNow);
    }
}
