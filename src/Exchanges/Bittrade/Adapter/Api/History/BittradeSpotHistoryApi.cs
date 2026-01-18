using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Operations;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Types;
using ExchangeApi.Primitives.CallCommon;

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
            return NotSupportedCall.Create<MarketLimitCursorRequest, Page<ExecutionItem>>(
                "Contracts",
                BittradeOperations.History.GetExecutions,
                request,
                "AccountIdRequired");
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
        IReadOnlyList<ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading.BittradeOpenOrder> orders)
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
        var meta = BuildMeta(requestedLimit, appliedLimit, items.Count, Completeness.MayBePartial, PartialReason.Unknown);
        return new Page<ExecutionItem>(items, HasMore: false, NextCursor: null, Meta: meta);
    }

    private static OrderSnapshotItem MapSnapshot(ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading.BittradeOpenOrder order)
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
