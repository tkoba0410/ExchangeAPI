using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Spec.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

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

    public Task<OrderResult> PlaceLimitOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        UnwrapAsync(
            PlaceLimitOrderCallAsync(symbol, side, size, price, cancellationToken),
            BittradeOperations.Trading.PlaceOrder);

    public Task<OrderResult> PlaceMarketOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        UnwrapAsync(
            PlaceMarketOrderCallAsync(symbol, side, size, cancellationToken),
            BittradeOperations.Trading.PlaceOrder);

    public Task<OrderResult> PlaceStopOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(Exchange, "StopOrder");

    public async Task<CancelResult> CancelOrderAsync(CommonSymbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default)
    {
        return await UnwrapAsync(
                CancelOrderCallAsync(symbol, orderKey, cancellationToken),
                BittradeOperations.Trading.CancelOrder)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        return await UnwrapAsync(
                GetOrdersCallAsync(symbol, cancellationToken),
                BittradeOperations.Trading.GetOpenOrders)
            .ConfigureAwait(false);
    }

    public async Task<OrderStatus> GetOrderAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        return await UnwrapAsync(
                GetOrderCallAsync(symbol, orderKey, cancellationToken),
                BittradeOperations.Trading.GetOrder)
            .ConfigureAwait(false);
    }

    public Task<IReadOnlyList<ParentOrder>> GetParentOrdersAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        UnwrapAsync(
            GetParentOrdersCallAsync(symbol, parentOrderId, parentOrderAcceptanceId, cancellationToken),
            BittradeOperations.Trading.GetParentOrders);

    public Task<ParentOrderDetail> GetParentOrderAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        UnwrapAsync(
            GetParentOrderCallAsync(symbol, parentOrderId, parentOrderAcceptanceId, cancellationToken),
            BittradeOperations.Trading.GetParentOrder);

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

    public Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceStopOrderRequest(symbol, side, size, triggerPrice);
        var now = DateTimeOffset.UtcNow;
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: BittradeOperations.Trading.PlaceOrder,
            Tags: null,
            Children: null);
        return Task.FromResult(new Call<PlaceStopOrderRequest, OrderResult>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<OrderResult>.Err(new CallError(CallErrorKind.Semantic, "Feature not supported.")),
            Meta: meta));
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

    public async Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrdersRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BittradeOperations.Trading.GetOpenOrders);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrdersRequest, IReadOnlyList<OpenOrder>>(
                request,
                startedAt,
                BittradeOperations.Trading.GetOpenOrders,
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

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>> GetParentOrdersCallAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GetParentOrdersRequest(symbol, parentOrderId, parentOrderAcceptanceId);
        return Task.FromResult(NotSupported<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>(
            request,
            BittradeOperations.Trading.GetParentOrders));
    }

    public Task<Call<GetParentOrderRequest, ParentOrderDetail>> GetParentOrderCallAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GetParentOrderRequest(symbol, parentOrderId, parentOrderAcceptanceId);
        return Task.FromResult(NotSupported<GetParentOrderRequest, ParentOrderDetail>(
            request,
            BittradeOperations.Trading.GetParentOrder));
    }

    private static async Task<TOk> UnwrapAsync<TReq, TOk>(
        Task<Call<TReq, TOk>> callTask,
        string operation)
    {
        var call = await callTask.ConfigureAwait(false);
        return call.Result switch
        {
            CallResult<TOk>.Ok ok => ok.Response,
            CallResult<TOk>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: Exchange,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.Error.HttpStatus),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error)),
            _ => throw new ExchangeApiException(
                message: "Unknown call result.",
                exchange: Exchange,
                operation: operation,
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(new CallError(CallErrorKind.Unknown, "Unknown call result.")))
        };
    }

    private static Call<TReq, TOk> NotSupported<TReq, TOk>(TReq request, string operation)
    {
        var now = DateTimeOffset.UtcNow;
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: operation,
            Tags: null,
            Children: null);
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Semantic, "Feature not supported.")),
            Meta: meta);
    }

}
