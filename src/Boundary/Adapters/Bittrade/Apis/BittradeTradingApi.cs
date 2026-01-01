using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Spec.CallCommon;
using System.Text.Json;
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

    public async Task<ApiCall<PlaceLimitOrderRequest, OrderResult, ApiError>> PlaceLimitOrderCallAsync(
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
            return call.Result switch
            {
                Ok<OrderResult, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<OrderResult, JsonElement> err => ApiCallMapper.Err<PlaceLimitOrderRequest, OrderResult>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceLimitOrderRequest, OrderResult>(Exchange, request, startedAt, ex);
        }
    }

    public async Task<ApiCall<PlaceMarketOrderRequest, OrderResult, ApiError>> PlaceMarketOrderCallAsync(
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
            return call.Result switch
            {
                Ok<OrderResult, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<OrderResult, JsonElement> err => ApiCallMapper.Err<PlaceMarketOrderRequest, OrderResult>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceMarketOrderRequest, OrderResult>(Exchange, request, startedAt, ex);
        }
    }

    public Task<ApiCall<PlaceStopOrderRequest, OrderResult, ApiError>> PlaceStopOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceStopOrderRequest(symbol, side, size, triggerPrice);
        var meta = ApiCallMapper.ToMeta(DateTimeOffset.UtcNow);
        return Task.FromResult(ApiCallMapper.Err<PlaceStopOrderRequest, OrderResult>(
            Exchange,
            request,
            meta,
            0,
            "Feature not supported."));
    }

    public async Task<ApiCall<CancelOrderRequest, CancelResult, ApiError>> CancelOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new CancelOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.CancelOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<CancelResult, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<CancelResult, JsonElement> err => ApiCallMapper.Err<CancelOrderRequest, CancelResult>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelResult>(Exchange, request, startedAt, ex);
        }
    }

    public async Task<ApiCall<GetOrdersRequest, IReadOnlyList<OpenOrder>, ApiError>> GetOrdersCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrdersRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<OpenOrder>, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<IReadOnlyList<OpenOrder>, JsonElement> err => ApiCallMapper.Err<GetOrdersRequest, IReadOnlyList<OpenOrder>>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrdersRequest, IReadOnlyList<OpenOrder>>(
                Exchange,
                request,
                startedAt,
                ex);
        }
    }

    public async Task<ApiCall<GetOrderRequest, OrderStatus, ApiError>> GetOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading.GetOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<OrderStatus, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<OrderStatus, JsonElement> err => ApiCallMapper.Err<GetOrderRequest, OrderStatus>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderRequest, OrderStatus>(Exchange, request, startedAt, ex);
        }
    }

    private static async Task<TOk> UnwrapAsync<TReq, TOk>(
        Task<ApiCall<TReq, TOk, ApiError>> callTask,
        string operation)
    {
        var call = await callTask.ConfigureAwait(false);
        return call.Result switch
        {
            ApiOk<TOk, ApiError> ok => ok.Value,
            ApiErr<TOk, ApiError> err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: call.Exchange,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.StatusCode),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error.Kind)),
            _ => throw new InvalidOperationException("Unsupported ApiCallResult type.")
        };
    }

}
