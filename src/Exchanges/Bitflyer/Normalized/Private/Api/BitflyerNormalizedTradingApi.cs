using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using RawPrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Api;

internal sealed class BitflyerNormalizedTradingApi
{
    private readonly IBitflyerRawApi _raw;
    private readonly IBitflyerMarketResolver _markets;

    public BitflyerNormalizedTradingApi(
        IBitflyerRawApi raw,
        IBitflyerMarketResolver markets)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>> SendChildOrderCallAsync(
        BitflyerOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BitflyerTradingMapper.ValidateOrderRequest(request);

        var callRequest = new PrivateRequests.PlaceOrderRequest(request);
        var marketCall = await _markets.ResolveCallAsync(request.Symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>(
                marketCall,
                callRequest,
                "Bitflyer.CreateChildOrder",
                marketError!);
        }

        var childOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price);
        var dto = new RawPrivateModels.CreateChildOrderRequest
        {
            ProductCode = productCode!,
            Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
            ChildOrderType = BitflyerTradingMapper.ToApiChildOrderType(childOrderType),
            Size = request.Size.Value,
            Price = request.Price?.Value,
        };

        var bodyJson = BitflyerOrderEncoder.BuildChildOrderBodyJson(dto);
        var rawCall = await _raw
            .SendChildOrderCallAsync(bodyJson, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CreateChildOrder",
            ok =>
            {
                var acceptanceId = ok.ChildOrderAcceptanceId;
                var key = new OrderKey(OrderIdKind.AcceptanceId, acceptanceId);
                return new BitflyerOrderResult(key, AcceptanceId: acceptanceId);
            });
    }

    public async Task<Call<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var callRequest = new PrivateRequests.CancelOrderRequest(symbol, orderKey);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>(
                marketCall,
                callRequest,
                "Bitflyer.CancelChildOrder",
                marketError!);
        }

        RawPrivateModels.CancelChildOrderRequest dto;
        switch (orderKey.Kind)
        {
            case OrderIdKind.AcceptanceId:
                dto = new RawPrivateModels.CancelChildOrderRequest
                {
                    ProductCode = productCode!,
                    ChildOrderAcceptanceId = orderKey.Value,
                };
                break;
            case OrderIdKind.ExchangeOrderId:
                dto = new RawPrivateModels.CancelChildOrderRequest
                {
                    ProductCode = productCode!,
                    ChildOrderId = orderKey.Value,
                };
                break;
            default:
                return CreateNotSupported<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>(
                    callRequest,
                    component: "Bitflyer.Trading",
                    feature: "CancelOrder",
                    reason: $"orderKey.Kind={orderKey.Kind}",
                    meta: marketCall.Meta);
        }

        var rawCall = await _raw
            .CancelChildOrderCallAsync(dto, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(rawCall, callRequest, "Bitflyer.CancelChildOrder", _ => new BitflyerCancelResult(true));
    }

    public async Task<Call<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var callRequest = new PrivateRequests.CancelAllChildOrdersRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>(
                marketCall,
                callRequest,
                "Bitflyer.CancelAllChildOrders",
                marketError!);
        }

        var rawCall = await _raw
            .CancelAllChildOrdersCallAsync(new RawPrivateModels.CancelAllChildOrdersRequest { ProductCode = productCode! }, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(rawCall, callRequest, "Bitflyer.CancelAllChildOrders", _ => new BitflyerCancelResult(true));
    }

    public async Task<Call<PrivateRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var callRequest = new PrivateRequests.GetOpenOrdersRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>(
                marketCall,
                callRequest,
                "Bitflyer.GetOpenOrders",
                marketError!);
        }

        var rawCall = await _raw
            .GetChildOrdersCallAsync(
                new RawPrivateModels.GetChildOrdersRequest(
                    productCode!,
                    ChildOrderStatusState: "ACTIVE",
                    ChildOrderAcceptanceId: null),
                cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetOpenOrders",
            rawOrders =>
            {
                IReadOnlyList<BitflyerOpenOrder> mapped = rawOrders.Select(o =>
                {
                    var acceptanceId = string.IsNullOrWhiteSpace(o.ChildOrderAcceptanceId) ? null : o.ChildOrderAcceptanceId;
                    var exchangeOrderId = string.IsNullOrWhiteSpace(o.ChildOrderId) ? null : o.ChildOrderId;
                    var key = acceptanceId is not null
                        ? new OrderKey(OrderIdKind.AcceptanceId, acceptanceId)
                        : exchangeOrderId is not null
                            ? new OrderKey(OrderIdKind.ExchangeOrderId, exchangeOrderId)
                            : throw new InvalidOperationException(
                                "bitFlyer order is missing both acceptanceId and exchangeOrderId.");

                    return new BitflyerOpenOrder(
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

    public async Task<Call<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var callRequest = new PrivateRequests.GetOrderRequest(symbol, orderKey);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>(
                marketCall,
                callRequest,
                "Bitflyer.GetOrder",
                marketError!);
        }

        if (orderKey.Kind is not (OrderIdKind.AcceptanceId or OrderIdKind.ExchangeOrderId))
        {
            return CreateNotSupported<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>(
                callRequest,
                component: "Bitflyer.Trading",
                feature: "GetOrder",
                reason: $"orderKey.Kind={orderKey.Kind}",
                meta: marketCall.Meta);
        }

        var rawCall = orderKey.Kind == OrderIdKind.AcceptanceId
            ? await _raw
                .GetChildOrdersCallAsync(
                    new RawPrivateModels.GetChildOrdersRequest(
                        productCode!,
                        ChildOrderStatusState: null,
                        ChildOrderAcceptanceId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false)
            : await _raw
                .GetChildOrdersCallAsync(
                    new RawPrivateModels.GetChildOrdersRequest(
                        productCode!,
                        ChildOrderStatusState: null,
                        ChildOrderId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetOrder",
            orders =>
            {
                var order = orders.FirstOrDefault();
                if (order is null)
                {
                    throw new InvalidOperationException(
                        $"Order not found. symbol={symbol} orderKey={orderKey}");
                }

                var status = BitflyerCommonMapper.MapOrderStatus(order.ChildOrderStatusState);
                var resolvedKey = !string.IsNullOrWhiteSpace(order.ChildOrderAcceptanceId)
                    ? new OrderKey(OrderIdKind.AcceptanceId, order.ChildOrderAcceptanceId)
                    : new OrderKey(OrderIdKind.ExchangeOrderId, order.ChildOrderId);
                return new BitflyerOrderStatus(
                    ProductCode: productCode!,
                    Key: resolvedKey,
                    Status: status,
                    ExecutedSize: new Size(order.ExecutedSize),
                    OutstandingSize: new Size(order.OutstandingSize),
                    Price: order.Price == 0 ? null : new Price(order.Price),
                    AveragePrice: order.AveragePrice == 0 ? null : new Price(order.AveragePrice));
            });
    }

    public async Task<Call<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawParameters = request.Parameters.Select(p => new RawPrivateModels.CreateParentOrderParameter
        {
            ProductCode = p.ProductCode,
            ConditionType = BitflyerParentOrderMapper.ToApiConditionType(p.ConditionType),
            Side = BitflyerSideMapper.ToApi(p.Side),
            Price = p.Price?.Value,
            Size = p.Size.Value,
            TriggerPrice = p.TriggerPrice?.Value,
            Offset = p.Offset,
        }).ToArray();

        var rawRequest = new RawPrivateModels.CreateParentOrderRequest
        {
            OrderMethod = request.OrderMethod.HasValue
                ? BitflyerParentOrderMapper.ToApiOrderMethod(request.OrderMethod.Value)
                : null,
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = request.TimeInForce.HasValue
                ? BitflyerParentOrderMapper.ToApiTimeInForce(request.TimeInForce.Value)
                : null,
            Parameters = rawParameters
        };

        var bodyJson = BitflyerOrderEncoder.BuildParentOrderBodyJson(rawRequest);
        var rawCall = await _raw
            .SendParentOrderCallAsync(bodyJson, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CreateParentOrder",
            ok => new BitflyerParentOrderAcceptance(ok.ParentOrderAcceptanceId));
    }

    public async Task<Call<PrivateRequests.CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        PrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawPrivateModels.CancelParentOrderRequest
        {
            ProductCode = request.ProductCode,
            ParentOrderId = request.ParentOrderId,
            ParentOrderAcceptanceId = request.ParentOrderAcceptanceId,
        };

        var rawCall = await _raw
            .CancelParentOrderCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CancelParentOrder",
            _ => new BitflyerParentOrderCancelResult(true));
    }

    public async Task<Call<PrivateRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawPrivateModels.GetParentOrdersRequest(
            request.ProductCode,
            request.ParentOrderState.HasValue
                ? BitflyerParentOrderMapper.ToApiParentOrderState(request.ParentOrderState.Value)
                : null,
            request.Count,
            request.Before,
            request.After);

        var rawCall = await _raw
            .GetParentOrdersCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetParentOrders",
            ok => BitflyerParentOrderNormalizer.NormalizeList(ok, rawCall.Meta.RawJson));
    }

    public async Task<Call<PrivateRequests.GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        PrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawPrivateModels.GetParentOrderRequest(
            request.ParentOrderId,
            request.ParentOrderAcceptanceId);

        var rawCall = await _raw
            .GetParentOrderCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetParentOrder",
            ok => BitflyerParentOrderNormalizer.NormalizeDetail(ok, rawCall.Meta.RawJson));
    }


    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: rawCall.Meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: rawCall.Meta)
        };
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, TOk> mapper)
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
                Meta: rawCall.Meta);
        }
        catch (InvalidOperationException ex)
        {
            var error = new CallError(CallErrorKind.Semantic, ex.Message, ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: rawCall.Meta);
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
                Meta: rawCall.Meta);
        }
    }

    private static bool TryGetProductCode(
        Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo> marketCall,
        out string? productCode,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err err)
        {
            productCode = null;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Ok ok &&
            !string.IsNullOrWhiteSpace(ok.Response.ProductCode))
        {
            productCode = ok.Response.ProductCode;
            error = null;
            return true;
        }

        productCode = null;
        error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
        return false;
    }

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo> marketCall,
        TReq request,
        string component,
        CallError error)
    {
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: marketCall.Meta);
    }

    private static Call<TReq, TOk> CreateNotSupported<TReq, TOk>(
        TReq request,
        string component,
        string feature,
        string? reason,
        CallMeta meta)
    {
        var message = reason is null
            ? $"NotSupported:{feature}"
            : $"NotSupported:{feature}. reason={reason}";
        var error = new CallError(CallErrorKind.Semantic, message);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

}
