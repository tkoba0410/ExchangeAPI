using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
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

    public Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default)
    {
        var request = new PlaceStopOrderRequest(symbol, side, size, triggerPrice);
        var now = DateTimeOffset.UtcNow;
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: BitflyerOperations.Trading.PlaceOrder,
            Tags: null,
            Children: null);
        var call = new Call<PlaceStopOrderRequest, OrderResult>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<OrderResult>.Err(new CallError(CallErrorKind.Semantic, "Feature not supported.")),
            Meta: meta);
        return Task.FromResult(call);
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

    public async Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrdersRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _tradingApi.GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Trading.GetOpenOrders);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrdersRequest, IReadOnlyList<OpenOrder>>(
                request,
                startedAt,
                BitflyerOperations.Trading.GetOpenOrders,
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
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.Error.HttpStatus),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error)),
            _ => throw new ExchangeApiException(
                message: "Unknown call result.",
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(new CallError(CallErrorKind.Unknown, "Unknown call result.")))
        };
    }

}
