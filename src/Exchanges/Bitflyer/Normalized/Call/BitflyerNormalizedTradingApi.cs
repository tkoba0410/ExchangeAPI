using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using RawPrivate = ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using RawPrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Call;

internal sealed class BitflyerNormalizedTradingApi : IBitflyerNormalizedTradingApi
{
    private readonly RawPrivate.IBitflyerRawPrivateTradingApi _tradingApi;
    private readonly RawPrivate.IBitflyerPrivateApi _privateApi;
    private readonly IExchangeMarketResolver _markets;

    public BitflyerNormalizedTradingApi(
        RawPrivate.IBitflyerRawPrivateTradingApi tradingApi,
        RawPrivate.IBitflyerPrivateApi privateApi,
        IExchangeMarketResolver markets)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BitflyerTradingMapper.ValidateOrderRequest(request);

        var callRequest = new PlaceOrderRequest(request);
        var marketCall = await _markets.ResolveCallAsync(request.Symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PlaceOrderRequest, OrderResult>(
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
        var rawCall = await _tradingApi
            .CreateChildOrderAsync(bodyJson, cancellationToken)
            .ConfigureAwait(false);

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

        var callRequest = new CancelOrderRequest(symbol, orderKey);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<CancelOrderRequest, CancelResult>(
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
                throw new ExchangeFeatureNotSupportedException(
                    ExchangeCode.Bitflyer,
                    feature: "CancelOrder",
                    reason: $"orderKey.Kind={orderKey.Kind}");
        }

        var rawCall = await _tradingApi
            .CancelChildOrderAsync(new RawRequests.CancelChildOrderRequest(dto), cancellationToken)
            .ConfigureAwait(false);

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

        var callRequest = new GetOpenOrdersRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>(
                marketCall,
                callRequest,
                "Bitflyer.GetOpenOrders",
                marketError!);
        }

        var rawCall = await _privateApi
            .GetChildOrdersAsync(
                new RawRequests.GetChildOrdersRequest(
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

        var callRequest = new GetOrderRequest(symbol, orderKey);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<GetOrderRequest, OrderStatus>(
                marketCall,
                callRequest,
                "Bitflyer.GetOrder",
                marketError!);
        }

        var rawCall = orderKey.Kind switch
        {
            OrderIdKind.AcceptanceId => await _privateApi
                .GetChildOrdersAsync(
                    new RawRequests.GetChildOrdersRequest(
                        productCode!,
                        ChildOrderStatusState: null,
                        ChildOrderAcceptanceId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false),
            OrderIdKind.ExchangeOrderId => await _privateApi
                .GetChildOrdersAsync(
                    new RawRequests.GetChildOrdersRequest(
                        productCode!,
                        ChildOrderStatusState: null,
                        ChildOrderId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false),
            _ => throw new ExchangeFeatureNotSupportedException(
                ExchangeCode.Bitflyer,
                feature: "GetOrder",
                reason: $"orderKey.Kind={orderKey.Kind}")
        };

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
                    ProductCode: productCode!,
                    Key: resolvedKey,
                    Status: status,
                    ExecutedSize: new Size(order.ExecutedSize),
                    OutstandingSize: new Size(order.OutstandingSize),
                    Price: order.Price == 0 ? null : new Price(order.Price),
                    AveragePrice: order.AveragePrice == 0 ? null : new Price(order.AveragePrice));
            });
    }

    public async Task<Call<SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
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
        var rawCall = await _tradingApi
            .CreateParentOrderAsync(bodyJson, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CreateParentOrder",
            ok => new BitflyerParentOrderAcceptance(ok.ParentOrderAcceptanceId));
    }

    public async Task<Call<CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
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

        var rawCall = await _tradingApi
            .CancelParentOrderAsync(new RawRequests.CancelParentOrderRequest(rawRequest), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CancelParentOrder",
            _ => new BitflyerParentOrderCancelResult(true));
    }

    public async Task<Call<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawRequests.GetParentOrdersRequest(
            request.ProductCode,
            request.ParentOrderState.HasValue
                ? BitflyerParentOrderMapper.ToApiParentOrderState(request.ParentOrderState.Value)
                : null,
            request.Count,
            request.Before,
            request.After);

        var rawCall = await _privateApi
            .GetParentOrdersAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetParentOrders",
            ok => BitflyerParentOrderNormalizer.NormalizeList(ok, rawCall.Meta.RawJson));
    }

    public async Task<Call<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawRequests.GetParentOrderRequest(
            request.ParentOrderId,
            request.ParentOrderAcceptanceId);

        var rawCall = await _privateApi
            .GetParentOrderAsync(rawRequest, cancellationToken)
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

    private static bool TryGetProductCode(
        Call<ExchangeApi.Contracts.Facade.Requests.ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
        out string? productCode,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            productCode = null;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Ok ok &&
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
        Call<ExchangeApi.Contracts.Facade.Requests.ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
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
