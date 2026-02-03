using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using BittradeOrderRequest = ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests.BittradeOrderRequest;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Operations;
namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class BittradeTradingApi
{
    private readonly BittradeNormalizedPrivateApi _trading;

    public BittradeTradingApi(BittradeNormalizedPrivateApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        GetOrdersByOrderIdCallAsync(symbol, orderKey, cancellationToken);

    public async Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceLimitOrderRequest(symbol, side, size, price);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading
                .PostOrdersPlaceCallAsync(
                    new NormalizedRequests.PostOrdersPlaceRequest(
                        new BittradeOrderRequest(
                            Symbol: symbol,
                            Side: side,
                            OrderType: OrderType.Limit,
                            Size: size,
                            Price: price)),
                    cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.Trading.PlaceOrder,
                MapOrderResult);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceLimitOrderRequest, OrderResult>(
                request,
                startedAt,
                BittradeOperations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceMarketOrderRequest(symbol, side, size);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading
                .PostOrdersPlaceCallAsync(
                    new NormalizedRequests.PostOrdersPlaceRequest(
                        new BittradeOrderRequest(
                            Symbol: symbol,
                            Side: side,
                            OrderType: OrderType.Market,
                            Size: size,
                            Price: null)),
                    cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.Trading.PlaceOrder,
                MapOrderResult);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceMarketOrderRequest, OrderResult>(
                request,
                startedAt,
                BittradeOperations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new CancelOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading
                .PostOrdersSubmitCancelByOrderIdCallAsync(symbol, orderKey, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.Trading.CancelOrder,
                ok => new CancelResult(ok.IsSuccess));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelResult>(
                request,
                startedAt,
                BittradeOperations.Trading.CancelOrder,
                ex);
        }
    }

    public async Task<Call<GetOrderRequest, OrderStatus>> GetOrdersByOrderIdCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOrdersByOrderIdCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.Trading.GetOrder,
                MapOrderStatus);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderRequest, OrderStatus>(
                request,
                startedAt,
                BittradeOperations.Trading.GetOrder,
                ex);
        }
    }

    public async Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOpenOrdersRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.Trading.GetOpenOrders,
                ok => (IReadOnlyList<OrderSnapshotItem>)ok.Select(MapSnapshot).ToArray());
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>(
                request,
                startedAt,
                BittradeOperations.Trading.GetOpenOrders,
                ex);
        }
    }

    private static OrderSnapshotItem MapSnapshot(BittradeOpenOrder order)
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
            OrderId: OrderId.ParseOrThrow(order.Key.Value),
            Market: order.Symbol,
            Side: order.Side,
            OrderType: orderType,
            Price: order.Price,
            Size: order.Size,
            Status: OrderSnapshotStatus.Open);
    }

    private static OrderResult MapOrderResult(BittradeOrderResult result) =>
        new(
            Key: result.Key,
            ExchangeOrderId: result.ExchangeOrderId,
            AcceptanceId: result.AcceptanceId);

    private static OrderStatus MapOrderStatus(BittradeOrderStatus status) =>
        new(
            ProductCode: status.ProductCode,
            Key: status.Key,
            Status: status.Status,
            ExecutedSize: status.ExecutedSize,
            OutstandingSize: status.OutstandingSize,
            Price: status.Price,
            AveragePrice: status.AveragePrice);
}
