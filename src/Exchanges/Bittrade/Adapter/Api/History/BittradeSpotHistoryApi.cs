using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Operations;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.History;

internal sealed class BittradeSpotHistoryApi : ISpotHistoryApi
{
    private readonly IBittradeNormalizedTradingApi _trading;
    private readonly string? _accountId;

    public BittradeSpotHistoryApi(
        IBittradeNormalizedTradingApi trading,
        string? accountId)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _accountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }

    public async Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
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
                ok => BuildOrderPage(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<MarketLimitCursorRequest, Page<OrderSnapshotItem>>(
                request,
                startedAt,
                BittradeOperations.History.GetOrders,
                ex);
        }
    }

    public async Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        if (string.IsNullOrWhiteSpace(_accountId))
        {
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: BittradeOperations.History.GetExecutions,
                Tags: null,
                Children: null);
            return new Call<MarketLimitCursorRequest, Page<ExecutionItem>>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<Page<ExecutionItem>>.Err(new CallError(
                    CallErrorKind.Semantic,
                    "Bittrade accountId is required to access executions.")),
                Meta: meta);
        }

        try
        {
            var call = await _trading
                .GetExecutionsCallAsync(request.Market, request.Limit, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.History.GetExecutions,
                ok => BuildExecutionPage(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<MarketLimitCursorRequest, Page<ExecutionItem>>(
                request,
                startedAt,
                BittradeOperations.History.GetExecutions,
                ex);
        }
    }

    private static Page<OrderSnapshotItem> BuildOrderPage(
        MarketLimitCursorRequest request,
        IReadOnlyList<OpenOrder> orders)
    {
        var items = orders.Select(MapSnapshot).ToList();
        var (requestedLimit, appliedLimit) = GetLimits(request);
        items = items.Take(appliedLimit).ToList();
        var meta = BuildMeta(requestedLimit, appliedLimit, items.Count, Completeness.MayBePartial, PartialReason.Unknown);
        return new Page<OrderSnapshotItem>(items, HasMore: false, NextCursor: null, Meta: meta);
    }

    private static Page<ExecutionItem> BuildExecutionPage(
        MarketLimitCursorRequest request,
        IReadOnlyList<BittradeExecutionNormalized> executions)
    {
        var items = executions.Select(e => new ExecutionItem(
            Timestamp: e.Timestamp,
            ExecutionId: e.Id,
            Market: request.Market,
            Side: string.Equals(e.Side, "buy", StringComparison.OrdinalIgnoreCase)
                ? Side.Buy
                : Side.Sell,
            Price: new Price(e.Price),
            Size: new Size(e.Size))).ToList();

        var (requestedLimit, appliedLimit) = GetLimits(request);
        items = items.Take(appliedLimit).ToList();
        var meta = BuildMeta(requestedLimit, appliedLimit, items.Count, Completeness.MayBePartial, PartialReason.Unknown);
        return new Page<ExecutionItem>(items, HasMore: false, NextCursor: null, Meta: meta);
    }

    private static OrderSnapshotItem MapSnapshot(OpenOrder order)
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
