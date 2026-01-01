using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ContractSide = ExchangeApi.Common.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Spec.CallCommon;
using System.Text.Json;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
internal sealed class BitflyerTradingApi : ITradingApi
{
    private readonly IBitflyerNormalizedTradingApi _tradingApi;
    private readonly ExchangeCode _exchange;

    public BitflyerTradingApi(
        IBitflyerNormalizedTradingApi tradingApi,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _exchange = exchange;
    }

    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        UnwrapAsync(
            PlaceLimitOrderCallAsync(symbol, side, size, price, cancellationToken),
            BitflyerOperations.Trading.PlaceOrder);

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        CancellationToken cancellationToken = default) =>
        UnwrapAsync(
            PlaceMarketOrderCallAsync(symbol, side, size, cancellationToken),
            BitflyerOperations.Trading.PlaceOrder);

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, "StopOrder");

    public async Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        return await UnwrapAsync(
                CancelOrderCallAsync(symbol, orderKey, cancellationToken),
                BitflyerOperations.Trading.CancelOrder)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        return await UnwrapAsync(
                GetOrdersCallAsync(symbol, cancellationToken),
                BitflyerOperations.Trading.GetOpenOrders)
            .ConfigureAwait(false);
    }

    public async Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        return await UnwrapAsync(
                GetOrderCallAsync(symbol, orderKey, cancellationToken),
                BitflyerOperations.Trading.GetOrder)
            .ConfigureAwait(false);
    }

    public async Task<ApiCall<PlaceLimitOrderRequest, OrderResult, ApiError>> PlaceLimitOrderCallAsync(
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
            return call.Result switch
            {
                Ok<OrderResult, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<OrderResult, JsonElement> err => ApiCallMapper.Err<PlaceLimitOrderRequest, OrderResult>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceLimitOrderRequest, OrderResult>(_exchange, request, startedAt, ex);
        }
    }

    public async Task<ApiCall<PlaceMarketOrderRequest, OrderResult, ApiError>> PlaceMarketOrderCallAsync(
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
            return call.Result switch
            {
                Ok<OrderResult, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<OrderResult, JsonElement> err => ApiCallMapper.Err<PlaceMarketOrderRequest, OrderResult>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<PlaceMarketOrderRequest, OrderResult>(_exchange, request, startedAt, ex);
        }
    }

    public Task<ApiCall<PlaceStopOrderRequest, OrderResult, ApiError>> PlaceStopOrderCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceStopOrderRequest(symbol, side, size, triggerPrice);
        var meta = ApiCallMapper.ToMeta(DateTimeOffset.UtcNow);
        return Task.FromResult(ApiCallMapper.Err<PlaceStopOrderRequest, OrderResult>(
            _exchange,
            request,
            meta,
            0,
            "Feature not supported."));
    }

    public async Task<ApiCall<CancelOrderRequest, CancelResult, ApiError>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new CancelOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _tradingApi.CancelOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<CancelResult, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<CancelResult, JsonElement> err => ApiCallMapper.Err<CancelOrderRequest, CancelResult>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelResult>(_exchange, request, startedAt, ex);
        }
    }

    public async Task<ApiCall<GetOrdersRequest, IReadOnlyList<OpenOrder>, ApiError>> GetOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrdersRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _tradingApi.GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<OpenOrder>, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<IReadOnlyList<OpenOrder>, JsonElement> err => ApiCallMapper.Err<GetOrdersRequest, IReadOnlyList<OpenOrder>>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrdersRequest, IReadOnlyList<OpenOrder>>(
                _exchange,
                request,
                startedAt,
                ex);
        }
    }

    public async Task<ApiCall<GetOrderRequest, OrderStatus, ApiError>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _tradingApi.GetOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<OrderStatus, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<OrderStatus, JsonElement> err => ApiCallMapper.Err<GetOrderRequest, OrderStatus>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderRequest, OrderStatus>(_exchange, request, startedAt, ex);
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
