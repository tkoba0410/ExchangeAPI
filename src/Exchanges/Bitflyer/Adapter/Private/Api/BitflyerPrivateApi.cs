using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitflyerOrderRequest = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests.BitflyerOrderRequest;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Utilities.Account;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

internal sealed class BitflyerPrivateApi
{
    private readonly IBitflyerNormalizedApi _normalized;

    public BitflyerPrivateApi(IBitflyerNormalizedApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new BalanceRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetBalanceCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Account.GetBalance,
                ok => new BalanceResponse(MapBalances(ok)));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<BalanceRequest, BalanceResponse>(
                request,
                startedAt,
                BitflyerOperations.Account.GetBalance,
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
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.History.GetOrders,
                ok => BuildOrderResponse(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<OrdersRequest, OrdersResponse>(
                request,
                startedAt,
                BitflyerOperations.History.GetOrders,
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
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.History.GetExecutions,
                ok => BuildExecutionResponse(request, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<ExecutionsPrivateRequest, ExecutionsPrivateResponse>(
                request,
                startedAt,
                BitflyerOperations.History.GetExecutions,
                ex);
        }
    }

    public async Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default)
    {
        var request = new OrderLimitRequest(symbol, side, size, price);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var normalizedRequest = new BitflyerOrderRequest(
                Symbol: symbol,
                Side: side,
                OrderType: OrderType.Limit,
                Size: size,
                Price: price);
            var call = await _normalized
                .SendChildOrderCallAsync(normalizedRequest, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.PlaceOrder,
                ok => new OrderLimitResponse(
                    Key: ok.Key,
                    ExchangeOrderId: ok.ExchangeOrderId,
                    AcceptanceId: ok.AcceptanceId));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<OrderLimitRequest, OrderLimitResponse>(
                request,
                startedAt,
                BitflyerOperations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new CancelOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.CancelOrder,
                ok => new CancelOrderResponse(ok.IsSuccess));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelOrderResponse>(
                request,
                startedAt,
                BitflyerOperations.Trading.CancelOrder,
                ex);
        }
    }

    private static IReadOnlyList<BalanceEntry> MapBalances(IReadOnlyList<BitflyerBalanceEntryNormalized> balances) =>
        balances
            .Select(b => BalanceFactory.Create(
                currency: b.CurrencyCode,
                amount: b.Amount,
                available: b.Available))
            .ToArray();

    private static OrdersResponse BuildOrderResponse(
        OrdersRequest request,
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
        IReadOnlyList<BitflyerExecutionAccountNormalized> executions)
    {
        var items = executions.Select(e => new ExecutionsPrivateItem(
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

    private static OrdersItem MapSnapshot(BitflyerOpenOrder order)
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
