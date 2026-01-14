using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ContractSide = ExchangeApi.Contracts.Common.DomainCommon.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Contracts.Common.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Trading;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
internal sealed class BitflyerTradingApi : ITradingApi
{
    private readonly IBitflyerNormalizedTradingApi _tradingApi;

    public BitflyerTradingApi(
        IBitflyerNormalizedTradingApi tradingApi)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
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
            var call = await _tradingApi
                .PlaceOrderCallAsync(OrderRequest.Limit(symbol, side, size, price), cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Trading.PlaceOrder);
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
            var call = await _tradingApi
                .PlaceOrderCallAsync(OrderRequest.Market(symbol, side, size), cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Trading.PlaceOrder);
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
            var call = await _tradingApi.CancelOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Trading.CancelOrder);
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
            var call = await _tradingApi.GetOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Trading.GetOrder);
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
            var call = await _tradingApi.GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
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
