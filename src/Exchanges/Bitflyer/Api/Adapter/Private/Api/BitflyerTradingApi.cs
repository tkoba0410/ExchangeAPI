using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using BitflyerOrderRequest = ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests.BitflyerOrderRequest;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
internal sealed class BitflyerTradingApi
{
    private readonly IBitflyerNormalizedApi _normalized;

    public BitflyerTradingApi(
        IBitflyerNormalizedApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceLimitOrderRequest(symbol, side, size, price);
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
                MapOrderResult);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceLimitOrderRequest, OrderResult>(
                request,
                startedAt,
                BitflyerOperations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceMarketOrderRequest(symbol, side, size);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var normalizedRequest = new BitflyerOrderRequest(
                Symbol: symbol,
                Side: side,
                OrderType: OrderType.Market,
                Size: size,
                Price: null);
            var call = await _normalized
                .SendChildOrderCallAsync(normalizedRequest, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.PlaceOrder,
                MapOrderResult);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceMarketOrderRequest, OrderResult>(
                request,
                startedAt,
                BitflyerOperations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
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
                ok => new CancelResult(ok.IsSuccess));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelResult>(
                request,
                startedAt,
                BitflyerOperations.Trading.CancelOrder,
                ex);
        }
    }

    public async Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetChildOrdersCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.GetOrder,
                MapOrderStatus);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderRequest, OrderStatus>(
                request,
                startedAt,
                BitflyerOperations.Trading.GetOrder,
                ex);
        }
    }

    public async Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOpenOrdersRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetChildOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.GetOpenOrders,
                ok => (IReadOnlyList<OrderSnapshotItem>)ok.Select(MapSnapshot).ToArray());
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>(
                request,
                startedAt,
                BitflyerOperations.Trading.GetOpenOrders,
                ex);
        }
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

    private static OrderResult MapOrderResult(BitflyerOrderResult result) =>
        new(
            Key: result.Key,
            ExchangeOrderId: result.ExchangeOrderId,
            AcceptanceId: result.AcceptanceId);

    private static OrderStatus MapOrderStatus(BitflyerOrderStatus status) =>
        new(
            ProductCode: status.ProductCode,
            Key: status.Key,
            Status: status.Status,
            ExecutedSize: status.ExecutedSize,
            OutstandingSize: status.OutstandingSize,
            Price: status.Price,
            AveragePrice: status.AveragePrice);

}
