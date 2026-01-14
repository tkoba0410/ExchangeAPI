using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Contracts.Common.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Contracts.Common.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Trading;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class BittradeTradingApi : ITradingApi
{
    private readonly IBittradeNormalizedTradingApi _trading;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeTradingApi(IBittradeNormalizedTradingApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

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
                .PlaceOrderCallAsync(OrderRequest.Limit(symbol, side, size, price), cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BittradeOperations.Trading.PlaceOrder);
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
                .PlaceOrderCallAsync(OrderRequest.Market(symbol, side, size), cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BittradeOperations.Trading.PlaceOrder);
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
            var call = await _trading.CancelOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BittradeOperations.Trading.CancelOrder);
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

    public async Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BittradeOperations.Trading.GetOrder);
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

}
