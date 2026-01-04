using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Spec.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Call;

internal sealed class BittradeNormalizedTradingApi : IBittradeNormalizedTradingApi
{
    private readonly IBittradeRawTradingApi _trading;
    private readonly IExchangeMarketResolver _markets;
    private readonly string _accountId;

    public BittradeNormalizedTradingApi(
        IBittradeRawTradingApi trading,
        IExchangeMarketResolver markets,
        string accountId)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
        _accountId = string.IsNullOrWhiteSpace(accountId)
            ? throw new ArgumentException("accountId is required.", nameof(accountId))
            : accountId;
    }

    public async Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken ct = default)
    {
        var call = await PlaceOrderCallAsync(request, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.PlaceOrder");
    }

    public async Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default)
    {
        var call = await CancelOrderCallAsync(symbol, orderKey, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.CancelOrder");
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken ct = default)
    {
        var call = await GetOpenOrdersCallAsync(symbol, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.GetOpenOrders");
    }

    public async Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default)
    {
        var call = await GetOrderCallAsync(symbol, orderKey, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.GetOrder");
    }

    public async Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var apiSymbol = await ToApiSymbolAsync(request.Symbol, ct).ConfigureAwait(false);
        var rawRequest = BittradeTradingMapper.ToRaw(_accountId, apiSymbol, request);
        var rawCall = await _trading
            .CreateOrderAsync(new RawRequests.CreateOrderRequest(rawRequest), ct)
            .ConfigureAwait(false);
        var callRequest = new PlaceOrderRequest(request);

        return CreateCall(rawCall, callRequest, "Bittrade.PlaceOrder", BittradeTradingMapper.ToOrderResult);
    }

    public async Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"CancelOrderBy{orderKey.Kind}");
        }

        var rawCall = await _trading
            .CancelOrderAsync(new RawRequests.CancelOrderRequest(orderKey.Value), ct)
            .ConfigureAwait(false);
        var callRequest = new CancelOrderRequest(symbol, orderKey);

        return CreateCall(rawCall, callRequest, "Bittrade.CancelOrder", _ => new CancelResult(true));
    }

    public async Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default)
    {
        var apiSymbol = await ToApiSymbolAsync(symbol, ct).ConfigureAwait(false);
        var rawCall = await _trading
            .GetOpenOrdersAsync(new RawRequests.GetOpenOrdersRequest(apiSymbol, _accountId), ct)
            .ConfigureAwait(false);
        var callRequest = new GetOpenOrdersRequest(symbol);

        return CreateCall(rawCall, callRequest, "Bittrade.GetOpenOrders", raw => BittradeTradingMapper.ToOpenOrders(symbol, raw));
    }

    public async Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"GetOrderBy{orderKey.Kind}");
        }

        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        var key = orderKey.Kind == OrderIdKind.AcceptanceId
            ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
            : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

        var rawCall = await _trading
            .GetOrderAsync(new RawRequests.GetOrderRequest(orderKey.Value), ct)
            .ConfigureAwait(false);
        var callRequest = new GetOrderRequest(symbol, orderKey);

        return CreateCall(rawCall, callRequest, "Bittrade.GetOrder", raw => BittradeTradingMapper.ToOrderStatus(market.ProductCode, raw, key));
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        var meta = new CallMeta(
            Layer: "Normalized",
            Component: component,
            Tags: null,
            Children: new[] { rawCall.Id });

        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper, meta),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: meta)
        };
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, TOk> mapper,
        CallMeta meta)
    {
        try
        {
            var mapped = mapper(raw);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Ok(mapped),
                Meta: meta);
        }
        catch (Exception ex)
        {
            var error = new CallError(CallErrorKind.Mapping, $"{component} failed to map normalized response.", ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: meta);
        }
    }

    private async Task<string> ToApiSymbolAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }

    private static TRes Unwrap<TReq, TRes>(Call<TReq, TRes> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TRes>.Ok ok => ok.Response,
            CallResult<TRes>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                statusCode: err.Error.HttpStatus is int status ? (HttpStatusCode?)status : null,
                innerException: err.Error.Exception),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}
