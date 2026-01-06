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
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
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

    public async Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = new PlaceOrderRequest(request);
        var marketCall = await _markets.ResolveCallAsync(request.Symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<PlaceOrderRequest, OrderResult>(
                marketCall,
                callRequest,
                "Bittrade.PlaceOrder",
                marketError!);
        }

        var rawRequest = BittradeTradingMapper.ToRaw(_accountId, apiSymbol!, request);
        var rawCall = await _trading
            .CreateOrderAsync(new RawRequests.CreateOrderRequest(rawRequest), ct)
            .ConfigureAwait(false);

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
        var callRequest = new GetOpenOrdersRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>(
                marketCall,
                callRequest,
                "Bittrade.GetOpenOrders",
                marketError!);
        }

        var rawCall = await _trading
            .GetOpenOrdersAsync(new RawRequests.GetOpenOrdersRequest(apiSymbol!, _accountId), ct)
            .ConfigureAwait(false);

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

        var callRequest = new GetOrderRequest(symbol, orderKey);
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err marketError)
        {
            return CreateCallError<GetOrderRequest, OrderStatus>(
                marketCall,
                callRequest,
                "Bittrade.GetOrder",
                marketError.Error);
        }

        var market = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response;
        var key = orderKey.Kind == OrderIdKind.AcceptanceId
            ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
            : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

        var rawCall = await _trading
            .GetOrderAsync(new RawRequests.GetOrderRequest(orderKey.Value), ct)
            .ConfigureAwait(false);

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

    private static bool TryGetApiSymbol(
        Call<ExchangeApi.Contracts.Requests.ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
        out string? apiSymbol,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            apiSymbol = null;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Ok ok)
        {
            apiSymbol = ok.Response.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
            error = null;
            return true;
        }

        apiSymbol = null;
        error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
        return false;
    }

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        Call<ExchangeApi.Contracts.Requests.ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
        TReq request,
        string component,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Normalized",
            Component: component,
            Tags: null,
            Children: new[] { marketCall.Id });

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

}
