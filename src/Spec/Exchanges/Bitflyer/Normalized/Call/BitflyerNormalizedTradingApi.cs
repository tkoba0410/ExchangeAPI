using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Spec.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Call;

internal sealed class BitflyerNormalizedTradingApi : IBitflyerNormalizedTradingApi
{
    private readonly IBitflyerRawPrivateTradingApi _tradingApi;
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly IExchangeMarketResolver _markets;

    public BitflyerNormalizedTradingApi(
        IBitflyerRawPrivateTradingApi tradingApi,
        IBitflyerPrivateApi privateApi,
        IExchangeMarketResolver markets)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        var call = await PlaceOrderCallAsync(request, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.CreateChildOrder");
    }

    public async Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var call = await CancelOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.CancelChildOrder");
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var call = await GetOpenOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetOpenOrders");
    }

    public async Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var call = await GetOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetOrder");
    }

    public async Task<IReadOnlyList<BitflyerParentOrderNormalized>> GetParentOrdersAsync(
        Symbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var call = await GetParentOrdersCallAsync(
                symbol,
                parentOrderId,
                parentOrderAcceptanceId,
                cancellationToken)
            .ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetParentOrders");
    }

    public async Task<BitflyerParentOrderDetailNormalized> GetParentOrderAsync(
        Symbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var call = await GetParentOrderCallAsync(
                symbol,
                parentOrderId,
                parentOrderAcceptanceId,
                cancellationToken)
            .ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetParentOrder");
    }

    public async Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BitflyerTradingMapper.ValidateOrderRequest(request);

        var childOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price);
        var timeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce);
        var dto = new CreateChildOrderRequest
        {
            ProductCode = await ToProductCodeAsync(request.Symbol, cancellationToken).ConfigureAwait(false),
            Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
            ChildOrderType = BitflyerTradingMapper.ToApiChildOrderType(childOrderType),
            Size = request.Size.Value,
            Price = request.Price?.Value,
            TriggerPrice = request.TriggerPrice?.Value,
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = BitflyerTradingMapper.ToApiTimeInForce(timeInForce),
        };

        var rawCall = await _tradingApi
            .CreateChildOrderAsync(new RawRequests.CreateChildOrderRequest(dto), cancellationToken)
            .ConfigureAwait(false);
        var callRequest = new PlaceOrderRequest(request);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CreateChildOrder",
            ok =>
            {
                var acceptanceId = ok.ChildOrderAcceptanceId;
                var key = new OrderKey(OrderIdKind.AcceptanceId, acceptanceId);
                return new OrderResult(key, AcceptanceId: acceptanceId);
            });
    }

    public async Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        CancelChildOrderRequest dto;
        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        switch (orderKey.Kind)
        {
            case OrderIdKind.AcceptanceId:
                dto = new CancelChildOrderRequest
                {
                    ProductCode = productCode,
                    ChildOrderAcceptanceId = orderKey.Value,
                };
                break;
            case OrderIdKind.ExchangeOrderId:
                dto = new CancelChildOrderRequest
                {
                    ProductCode = productCode,
                    ChildOrderId = orderKey.Value,
                };
                break;
            default:
                throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, $"CancelOrderBy{orderKey.Kind}");
        }

        var rawCall = await _tradingApi
            .CancelChildOrderAsync(new RawRequests.CancelChildOrderRequest(dto), cancellationToken)
            .ConfigureAwait(false);
        var callRequest = new CancelOrderRequest(symbol, orderKey);

        return CreateCall(rawCall, callRequest, "Bitflyer.CancelChildOrder", _ => new CancelResult(true));
    }

    public async Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _privateApi
            .GetChildOrdersAsync(
                new RawRequests.GetChildOrdersRequest(
                    productCode,
                    ChildOrderStatusState: "ACTIVE",
                    ChildOrderAcceptanceId: null),
                cancellationToken)
            .ConfigureAwait(false);
        var callRequest = new GetOpenOrdersRequest(symbol);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetOpenOrders",
            rawOrders =>
            {
                IReadOnlyList<OpenOrder> mapped = rawOrders.Select(o =>
                {
                    var acceptanceId = string.IsNullOrWhiteSpace(o.ChildOrderAcceptanceId) ? null : o.ChildOrderAcceptanceId;
                    var exchangeOrderId = string.IsNullOrWhiteSpace(o.ChildOrderId) ? null : o.ChildOrderId;
                    var key = acceptanceId is not null
                        ? new OrderKey(OrderIdKind.AcceptanceId, acceptanceId)
                        : exchangeOrderId is not null
                            ? new OrderKey(OrderIdKind.ExchangeOrderId, exchangeOrderId)
                            : throw new ExchangeApiException(
                                message: "bitFlyer order is missing both acceptanceId and exchangeOrderId.",
                                exchange: ExchangeCode.Bitflyer,
                                operation: "Bitflyer.GetOpenOrders");

                    return new OpenOrder(
                        ExchangeCode: ExchangeCode.Bitflyer,
                        Symbol: symbol,
                        Key: key,
                        Side: BitflyerCommonMapper.MapSide(o.Side),
                        OrderType: BitflyerTradingMapper.ToOrderType(BitflyerTradingMapper.ParseChildOrderType(o.ChildOrderType)),
                        Size: new Size(o.Size),
                        OutstandingSize: new Size(o.OutstandingSize),
                        ExecutedSize: new Size(o.ExecutedSize),
                        Price: o.Price == 0 ? null : new Price(o.Price),
                        OrderedAt: o.ChildOrderDate,
                        UpdatedAt: null,
                        StopPrice: null,
                        Status: o.ChildOrderStatusState,
                        ExchangeOrderId: exchangeOrderId,
                        AcceptanceId: acceptanceId);
                }).ToArray();
                return mapped;
            });
    }

    public async Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = orderKey.Kind switch
        {
            OrderIdKind.AcceptanceId => await _privateApi
                .GetChildOrdersAsync(
                    new RawRequests.GetChildOrdersRequest(
                        productCode,
                        ChildOrderStatusState: null,
                        ChildOrderAcceptanceId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false),
            OrderIdKind.ExchangeOrderId => await _privateApi
                .GetChildOrdersAsync(
                    new RawRequests.GetChildOrdersRequest(
                        productCode,
                        ChildOrderStatusState: null,
                        ChildOrderId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false),
            _ => throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, $"GetOrderBy{orderKey.Kind}")
        };

        var callRequest = new GetOrderRequest(symbol, orderKey);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetOrder",
            orders =>
            {
                var order = orders.FirstOrDefault();
                if (order is null)
                {
                    throw new ExchangeOrderNotFoundException(ExchangeCode.Bitflyer, "Bitflyer.GetOrder", symbol.ToString(), orderKey.ToString());
                }

                var status = BitflyerCommonMapper.MapOrderStatus(order.ChildOrderStatusState);
                var resolvedKey = !string.IsNullOrWhiteSpace(order.ChildOrderAcceptanceId)
                    ? new OrderKey(OrderIdKind.AcceptanceId, order.ChildOrderAcceptanceId)
                    : new OrderKey(OrderIdKind.ExchangeOrderId, order.ChildOrderId);
                return new OrderStatus(
                    ProductCode: productCode,
                    Key: resolvedKey,
                    Status: status,
                    ExecutedSize: new Size(order.ExecutedSize),
                    OutstandingSize: new Size(order.OutstandingSize),
                    Price: order.Price == 0 ? null : new Price(order.Price),
                    AveragePrice: order.AveragePrice == 0 ? null : new Price(order.AveragePrice));
            });
    }

    public async Task<Call<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        Symbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _privateApi
            .GetParentOrdersAsync(
                new RawRequests.GetParentOrdersRequest(
                    productCode,
                    parentOrderId,
                    parentOrderAcceptanceId),
                cancellationToken)
            .ConfigureAwait(false);
        var callRequest = new GetParentOrdersRequest(symbol, parentOrderId, parentOrderAcceptanceId);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetParentOrders",
            raw => (IReadOnlyList<BitflyerParentOrderNormalized>)raw
                .Select(item => BitflyerParentOrderNormalizer.Normalize(item, rawCall.Meta.RawJson))
                .ToArray());
    }

    public async Task<Call<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        Symbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _privateApi
            .GetParentOrderAsync(
                new RawRequests.GetParentOrderRequest(
                    productCode,
                    parentOrderId,
                    parentOrderAcceptanceId),
                cancellationToken)
            .ConfigureAwait(false);
        var callRequest = new GetParentOrderRequest(symbol, parentOrderId, parentOrderAcceptanceId);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetParentOrder",
            raw => BitflyerParentOrderNormalizer.NormalizeDetail(raw, rawCall.Meta.RawJson));
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
            Children: new[] { rawCall.Id })
        {
            RawJson = rawCall.Meta.RawJson
        };

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
        catch (ExchangeOrderNotFoundException ex)
        {
            var error = new CallError(CallErrorKind.Semantic, ex.Message, ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
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

    private async Task<string> ToProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode;
    }

    private static TRes Unwrap<TReq, TRes>(Call<TReq, TRes> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TRes>.Ok ok => ok.Response,
            CallResult<TRes>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: err.Error.HttpStatus is int status ? (HttpStatusCode?)status : null,
                innerException: err.Error.Exception),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}
