using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Constants;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Api;

internal sealed class BitflyerNormalizedPrivateApi
{
    private readonly IBitflyerRawApi _raw;
    private readonly IBitflyerMarketResolver _markets;

    public BitflyerNormalizedPrivateApi(
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

        if (!BitflyerTradingMapper.TryValidateOrderRequest(request, out var validationError))
        {
            return CreateImmediateError<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>(
                new PrivateRequests.PlaceOrderRequest(request),
                "Bitflyer.CreateChildOrder",
                validationError!);
        }

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

        if (!BitflyerTradingMapper.TryMapOrderType(request.OrderType, request.Price, out var childOrderType, out var orderTypeError))
        {
            return CreateImmediateError<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>(
                callRequest,
                "Bitflyer.CreateChildOrder",
                orderTypeError!);
        }

        if (!BitflyerCommonMapper.TryMapSideToExchange(request.Side, out var apiSide, out var sideError))
        {
            return CreateImmediateError<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>(
                callRequest,
                "Bitflyer.CreateChildOrder",
                sideError!);
        }

        if (!BitflyerTradingMapper.TryToApiChildOrderType(childOrderType, out var apiChildOrderType, out var childOrderError))
        {
            return CreateImmediateError<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>(
                callRequest,
                "Bitflyer.CreateChildOrder",
                childOrderError!);
        }

        var dto = new RawPrivateRequests.CreateChildOrderRequest
        {
            ProductCode = productCode,
            Side = new FreeText(apiSide),
            ChildOrderType = new FreeText(apiChildOrderType),
            Size = request.Size.Value,
            Price = request.Price?.Value,
        };

        var rawCall = await _raw
            .SendChildOrderCallAsync(dto, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CreateChildOrder",
            ok => MapResult<BitflyerOrderResult>.Ok(
                new BitflyerOrderResult(
                    new OrderKey(OrderIdKind.AcceptanceId, ok.ChildOrderAcceptanceId),
                    AcceptanceId: new AcceptanceId(ok.ChildOrderAcceptanceId))));
    }

    public async Task<Call<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.CancelOrderRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>(
                callRequest,
                Component(BitflyerEndpointIds.CancelChildOrder),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>(
                marketCall,
                callRequest,
                Component(BitflyerEndpointIds.CancelChildOrder),
                marketError!);
        }

        RawPrivateRequests.CancelChildOrderRequest dto;
        switch (orderKey.Kind)
        {
            case OrderIdKind.AcceptanceId:
                dto = new RawPrivateRequests.CancelChildOrderRequest
                {
                    ProductCode = productCode,
                    ChildOrderAcceptanceId = new FreeText(orderKey.Value),
                };
                break;
            case OrderIdKind.ExchangeOrderId:
                dto = new RawPrivateRequests.CancelChildOrderRequest
                {
                    ProductCode = productCode,
                    ChildOrderId = new FreeText(orderKey.Value),
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

        return CreateCall(
            rawCall,
            callRequest,
            Component(BitflyerEndpointIds.CancelChildOrder),
            _ => MapResult<BitflyerCancelResult>.Ok(new BitflyerCancelResult(true)));
    }

    public async Task<Call<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.CancelAllChildOrdersRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>(
                callRequest,
                Component(BitflyerEndpointIds.CancelAllChildOrders),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>(
                marketCall,
                callRequest,
                Component(BitflyerEndpointIds.CancelAllChildOrders),
                marketError!);
        }

        var rawCall = await _raw
            .CancelAllChildOrdersCallAsync(new RawPrivateRequests.CancelAllChildOrdersRequest { ProductCode = productCode }, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BitflyerEndpointIds.CancelAllChildOrders),
            _ => MapResult<BitflyerCancelResult>.Ok(new BitflyerCancelResult(true)));
    }

    public async Task<Call<PrivateRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.GetOpenOrdersRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>(
                callRequest,
                Component(BitflyerEndpointIds.GetChildOrders),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

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
                new RawPrivateRequests.GetChildOrdersRequest(
                    productCode,
                    ChildOrderStatusState: new FreeText("ACTIVE"),
                    ChildOrderAcceptanceId: null),
                cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.GetOpenOrders",
            rawOrders =>
            {
                var mapped = new List<BitflyerOpenOrder>(rawOrders.Count);
                foreach (var o in rawOrders)
                {
                    var acceptanceId = AcceptanceId.TryParse(o.ChildOrderAcceptanceId, out var parsedAcceptanceId)
                        ? parsedAcceptanceId
                        : (AcceptanceId?)null;
                    var exchangeOrderId = ExchangeOrderId.TryParse(o.ChildOrderId, out var parsedExchangeOrderId)
                        ? parsedExchangeOrderId
                        : (ExchangeOrderId?)null;
                    if (acceptanceId is null && exchangeOrderId is null)
                    {
                        return MapResult<IReadOnlyList<BitflyerOpenOrder>>.Fail(
                            new CallError(CallErrorKind.Mapping, "bitFlyer order is missing both acceptanceId and exchangeOrderId."));
                    }

                    var key = acceptanceId is not null
                        ? new OrderKey(OrderIdKind.AcceptanceId, acceptanceId.Value.ToString())
                        : new OrderKey(OrderIdKind.ExchangeOrderId, exchangeOrderId!.Value.ToString());

                    if (!BitflyerCommonMapper.TryMapSide(o.Side, out var side, out var sideError))
                    {
                        return MapResult<IReadOnlyList<BitflyerOpenOrder>>.Fail(sideError!);
                    }

                    if (!BitflyerTradingMapper.TryParseChildOrderType(o.ChildOrderType, out var parsedOrderType, out var orderTypeError))
                    {
                        return MapResult<IReadOnlyList<BitflyerOpenOrder>>.Fail(orderTypeError!);
                    }

                    if (!BitflyerTradingMapper.TryToOrderType(parsedOrderType, out var mappedOrderType, out var mapError))
                    {
                        return MapResult<IReadOnlyList<BitflyerOpenOrder>>.Fail(mapError!);
                    }

                    mapped.Add(new BitflyerOpenOrder(
                        Symbol: symbol,
                        Key: key,
                        Side: side,
                        OrderType: mappedOrderType,
                        Size: new Size(o.Size),
                        OutstandingSize: new Size(o.OutstandingSize),
                        ExecutedSize: new Size(o.ExecutedSize),
                        Price: o.Price == 0 ? null : new Price(o.Price),
                        OrderedAt: o.ChildOrderDate,
                        UpdatedAt: null,
                        StopPrice: null,
                        Status: FreeText.Parse(o.ChildOrderStatusState),
                        ExchangeOrderId: exchangeOrderId,
                        AcceptanceId: acceptanceId));
                }

                return MapResult<IReadOnlyList<BitflyerOpenOrder>>.Ok(mapped.ToArray());
            });
    }

    public async Task<Call<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.GetOrderRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>(
                callRequest,
                "Bitflyer.GetOrder",
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

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
                    new RawPrivateRequests.GetChildOrdersRequest(
                        productCode,
                        ChildOrderStatusState: null,
                        ChildOrderAcceptanceId: new FreeText(orderKey.Value)),
                    cancellationToken)
                .ConfigureAwait(false)
            : await _raw
                .GetChildOrdersCallAsync(
                    new RawPrivateRequests.GetChildOrdersRequest(
                        productCode,
                        ChildOrderStatusState: null,
                        ChildOrderId: new FreeText(orderKey.Value)),
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
                    return MapResult<BitflyerOrderStatus>.Fail(
                        new CallError(CallErrorKind.Mapping, $"Order not found. symbol={symbol} orderKey={orderKey}"));
                }

                var status = BitflyerCommonMapper.MapOrderStatus(order.ChildOrderStatusState);
                var resolvedKey = !string.IsNullOrWhiteSpace(order.ChildOrderAcceptanceId)
                    ? new OrderKey(OrderIdKind.AcceptanceId, order.ChildOrderAcceptanceId)
                    : new OrderKey(OrderIdKind.ExchangeOrderId, order.ChildOrderId);
                return MapResult<BitflyerOrderStatus>.Ok(new BitflyerOrderStatus(
                    ProductCode: productCode,
                    Key: resolvedKey,
                    Status: status,
                    ExecutedSize: new Size(order.ExecutedSize),
                    OutstandingSize: new Size(order.OutstandingSize),
                    Price: order.Price == 0 ? null : new Price(order.Price),
                    AveragePrice: order.AveragePrice == 0 ? null : new Price(order.AveragePrice)));
            });
    }

    public async Task<Call<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawParameters = new List<RawPrivateRequests.CreateParentOrderParameter>(request.Parameters.Count);
        foreach (var p in request.Parameters)
        {
            if (!BitflyerParentOrderMapper.TryToApiConditionType(p.ConditionType, out var conditionType, out var conditionError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    conditionError!);
            }

            if (!BitflyerSideMapper.TryToApi(p.Side, out var side, out var sideError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    sideError!);
            }

            rawParameters.Add(new RawPrivateRequests.CreateParentOrderParameter
            {
                ProductCode = p.ProductCode,
                ConditionType = new FreeText(conditionType),
                Side = new FreeText(side),
                Price = p.Price?.Value,
                Size = p.Size.Value,
                TriggerPrice = p.TriggerPrice?.Value,
                Offset = p.Offset,
            });
        }

        string? orderMethod = null;
        if (request.OrderMethod.HasValue)
        {
            if (!BitflyerParentOrderMapper.TryToApiOrderMethod(request.OrderMethod.Value, out var method, out var methodError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    methodError!);
            }

            orderMethod = method;
        }

        string? timeInForce = null;
        if (request.TimeInForce.HasValue)
        {
            if (!BitflyerParentOrderMapper.TryToApiTimeInForce(request.TimeInForce.Value, out var tif, out var tifError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    tifError!);
            }

            timeInForce = tif;
        }

        var rawRequest = new RawPrivateRequests.CreateParentOrderRequest
        {
            OrderMethod = orderMethod is null ? null : new FreeText(orderMethod),
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = timeInForce is null ? null : new FreeText(timeInForce),
            Parameters = rawParameters.ToArray()
        };

        var rawCall = await _raw
            .SendParentOrderCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bitflyer.CreateParentOrder",
            ok => MapResult<BitflyerParentOrderAcceptance>.Ok(new BitflyerParentOrderAcceptance(new AcceptanceId(ok.ParentOrderAcceptanceId))));
    }

    public async Task<Call<PrivateRequests.CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        PrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawPrivateRequests.CancelParentOrderRequest
        {
            ProductCode = request.ProductCode,
            ParentOrderId = request.ParentOrderId is null ? null : new FreeText(request.ParentOrderId.Value.Value),
            ParentOrderAcceptanceId = request.ParentOrderAcceptanceId is null ? null : new FreeText(request.ParentOrderAcceptanceId.Value.Value),
        };

        var rawCall = await _raw
            .CancelParentOrderCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BitflyerEndpointIds.CancelParentOrder),
            _ => MapResult<BitflyerParentOrderCancelResult>.Ok(new BitflyerParentOrderCancelResult(true)));
    }

    public async Task<Call<PrivateRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        string? parentOrderState = null;
        if (request.ParentOrderState.HasValue)
        {
            if (!BitflyerParentOrderMapper.TryToApiParentOrderState(request.ParentOrderState.Value, out var state, out var stateError))
            {
                return CreateImmediateError<PrivateRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>(
                    callRequest,
                    Component(BitflyerEndpointIds.GetParentOrders),
                    stateError!);
            }

            parentOrderState = state;
        }

        var rawRequest = new RawPrivateRequests.GetParentOrdersRequest(
            request.ProductCode,
            parentOrderState is null ? null : new FreeText(parentOrderState),
            request.Count,
            request.Before,
            request.After);

        var rawCall = await _raw
            .GetParentOrdersCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BitflyerEndpointIds.GetParentOrders),
            ok =>
            {
                if (!BitflyerParentOrderNormalizer.TryNormalizeList(ok, rawCall.Meta.RawJson, out var normalized, out var error))
                {
                    return MapResult<IReadOnlyList<BitflyerParentOrderNormalized>>.Fail(error!);
                }

                return MapResult<IReadOnlyList<BitflyerParentOrderNormalized>>.Ok(normalized!);
            });
    }

    public async Task<Call<PrivateRequests.GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        PrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawRequest = new RawPrivateRequests.GetParentOrderRequest(
            request.ParentOrderId is null ? null : new FreeText(request.ParentOrderId.Value.Value),
            request.ParentOrderAcceptanceId is null ? null : new FreeText(request.ParentOrderAcceptanceId.Value.Value));

        var rawCall = await _raw
            .GetParentOrderCallAsync(rawRequest, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BitflyerEndpointIds.GetParentOrder),
            ok =>
            {
                if (!BitflyerParentOrderNormalizer.TryNormalizeDetail(ok, rawCall.Meta.RawJson, out var normalized, out var error))
                {
                    return MapResult<BitflyerParentOrderDetailNormalized>.Fail(error!);
                }

                return MapResult<BitflyerParentOrderDetailNormalized>.Ok(normalized!);
            });
    }

    public async Task<Call<PrivateRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceCallAsync(new RawPrivateRequests.GetBalancesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalancesRequest();
        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetBalances",
            raw =>
            {
                if (!BitflyerAccountMapper.TryMapBalances(raw, out var balances, out var mapError))
                {
                    return MapResult<IReadOnlyList<BitflyerBalanceEntryNormalized>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BitflyerBalanceEntryNormalized>>.Ok(balances!);
            });
    }

    public async Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetPermissionsCallAsync(new RawPrivateRequests.GetPermissionsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetPermissionsRequest();
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetPermissions),
            raw => MapResult<IReadOnlyList<FreeText>>.Ok(raw.Select(FreeText.Parse).ToArray()));
    }

    public async Task<Call<PrivateRequests.GetCollateralRequest, BitflyerCollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralCallAsync(new RawPrivateRequests.GetCollateralRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralRequest();
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetCollateral),
            raw => MapResult<BitflyerCollateralNormalized>.Ok(
                new BitflyerCollateralNormalized(
                    raw.Collateral,
                    raw.OpenPositionPnl,
                    raw.RequireCollateral,
                    raw.KeepRate)));
    }

    public async Task<Call<PrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<BitflyerCollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralAccountsCallAsync(new RawPrivateRequests.GetCollateralAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralAccountsRequest();
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetCollateralAccounts),
            raw => MapResult<IReadOnlyList<BitflyerCollateralAccountNormalized>>.Ok(
                raw.Select(item => new BitflyerCollateralAccountNormalized(CurrencyCodeConverter.FromString(item.CurrencyCode), item.Amount, item.Available))
                    .ToArray()));
    }

    public async Task<Call<PrivateRequests.GetAddressesRequest, BitflyerRawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetAddressesCallAsync(new RawPrivateRequests.GetAddressesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetAddressesRequest();
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetAddresses),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetCoinInsRequest, BitflyerRawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCoinInsCallAsync(new RawPrivateRequests.GetCoinInsRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCoinInsRequest(count, before, after);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetCoinIns),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetCoinOutsRequest, BitflyerRawJsonNormalized>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCoinOutsCallAsync(new RawPrivateRequests.GetCoinOutsRequest(messageId, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCoinOutsRequest(messageId, count, before, after);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetCoinOuts),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetBankAccountsRequest, BitflyerRawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBankAccountsCallAsync(new RawPrivateRequests.GetBankAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBankAccountsRequest();
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetBankAccounts),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetDepositsRequest, BitflyerRawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetDepositsCallAsync(new RawPrivateRequests.GetDepositsRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetDepositsRequest(count, before, after);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetDeposits),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.WithdrawRequest, BitflyerWithdrawResultNormalized>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default)
    {
        var currencyText = CurrencyCodeConverter.ToCurrencyString(currencyCode);
        var rawCall = await _raw
            .WithdrawCallAsync(new RawPrivateRequests.CreateWithdrawalRequest
            {
                CurrencyCode = new FreeText(currencyText),
                BankAccountId = bankAccountId,
                Amount = amount,
                Code = code,
            }, cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.WithdrawRequest(currencyCode, bankAccountId, amount, code);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.Withdraw),
            raw => MapResult<BitflyerWithdrawResultNormalized>.Ok(new BitflyerWithdrawResultNormalized(FreeText.Parse(raw.MessageId))));
    }

    public async Task<Call<PrivateRequests.GetWithdrawalsRequest, BitflyerRawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetWithdrawalsCallAsync(new RawPrivateRequests.GetWithdrawalsRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetWithdrawalsRequest(count, before, after);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetWithdrawals),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceHistoryCallAsync(new RawPrivateRequests.GetBalanceHistoryRequest(currencyCode, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalanceHistoryRequest(currencyCode, count, before, after);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetBalanceHistory),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new PrivateRequests.GetPositionsRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                request,
                Component(BitflyerEndpointIds.GetPositions),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                marketCall,
                request,
                Component(BitflyerEndpointIds.GetPositions),
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : ProductCode.Empty;
        if (productCode.IsEmpty)
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                marketCall,
                request,
                Component(BitflyerEndpointIds.GetPositions),
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetPositionsCallAsync(new RawPrivateRequests.GetPositionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetPositions),
            raw =>
            {
                var mapped = new List<BitflyerPositionNormalized>(raw.Count);
                foreach (var item in raw)
                {
                    if (!BitflyerCommonMapper.TryMapSide(item.Side, out var side, out var sideError))
                    {
                        return MapResult<IReadOnlyList<BitflyerPositionNormalized>>.Fail(sideError!);
                    }

                    mapped.Add(new BitflyerPositionNormalized(
                        ProductCode.ParseNormalized(item.ProductCode),
                        side,
                        item.Size,
                        item.Price,
                        item.Pnl,
                        item.OpenDate));
                }

                return MapResult<IReadOnlyList<BitflyerPositionNormalized>>.Ok(mapped.ToArray());
            });
    }

    public async Task<Call<PrivateRequests.GetCollateralHistoryRequest, BitflyerRawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralHistoryCallAsync(new RawPrivateRequests.GetCollateralHistoryRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralHistoryRequest(count, before, after);
        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetCollateralHistory),
            raw => MapResult<BitflyerRawJsonNormalized>.Ok(new BitflyerRawJsonNormalized(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new PrivateRequests.GetAccountExecutionsRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                request,
                Component(BitflyerEndpointIds.GetExecutionsPrivate),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                Component(BitflyerEndpointIds.GetExecutionsPrivate),
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : ProductCode.Empty;
        if (productCode.IsEmpty)
        {
            return CreateCallError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                Component(BitflyerEndpointIds.GetExecutionsPrivate),
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetExecutionsPrivateCallAsync(new RawPrivateRequests.GetAccountExecutionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetExecutionsPrivate),
            raw =>
            {
                if (!BitflyerAccountMapper.TryMapAccountExecutions(symbol, raw, out var executions, out var error))
                {
                    return MapResult<IReadOnlyList<BitflyerExecutionAccountNormalized>>.Fail(error!);
                }

                return MapResult<IReadOnlyList<BitflyerExecutionAccountNormalized>>.Ok(executions!);
            });
    }

    public async Task<Call<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new PrivateRequests.GetTradingCommissionRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                request,
                Component(BitflyerEndpointIds.GetTradingCommission),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                Component(BitflyerEndpointIds.GetTradingCommission),
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : ProductCode.Empty;
        if (productCode.IsEmpty)
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                Component(BitflyerEndpointIds.GetTradingCommission),
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetTradingCommissionCallAsync(new RawPrivateRequests.GetTradingCommissionRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetTradingCommission),
            raw =>
            {
                if (!TryParseTradingCommission(raw.RawJson, productCode, out var parsed, out var error))
                {
                    return MapResult<BitflyerTradingCommissionNormalized>.Fail(error!);
                }

                return MapResult<BitflyerTradingCommissionNormalized>.Ok(parsed!);
            });
    }

    private static bool TryParseTradingCommission(
        string? rawJson,
        ProductCode productCode,
        out BitflyerTradingCommissionNormalized? normalized,
        out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Trading commission response is empty.");
            return false;
        }

        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;
        ProductCode parsedProductCode = ProductCode.Empty;
        decimal? commissionRate = null;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("product_code", out var productCodeElement)
                && productCodeElement.ValueKind == JsonValueKind.String)
            {
                parsedProductCode = ProductCode.ParseNormalized(productCodeElement.GetString());
            }

            if (root.TryGetProperty("commission_rate", out var commissionElement))
            {
                commissionRate = TryParseDecimal(commissionElement);
            }
        }

        normalized = new BitflyerTradingCommissionNormalized(
            ProductCode: parsedProductCode.IsEmpty ? productCode : parsedProductCode,
            CommissionRate: commissionRate);
        error = null;
        return true;
    }

    private static decimal? TryParseDecimal(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.String => TryParseDecimalString(element.GetString()),
            _ => null
        };
    }

    private static decimal? TryParseDecimalString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, MapResult<TOk>> mapper)
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
        Func<TRaw, MapResult<TOk>> mapper)
    {
        try
        {
            var result = mapper(raw);
            if (result.Error is not null)
            {
                return new Call<TReq, TOk>(
                    Id: CallId.New(),
                    StartedAt: rawCall.StartedAt,
                    Duration: rawCall.Duration,
                    Request: request,
                    Result: new CallResult<TOk>.Err(result.Error),
                    Meta: rawCall.Meta);
            }

            var mapped = result.Value!;
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
        out ProductCode productCode,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err err)
        {
            productCode = ProductCode.Empty;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Ok ok &&
            !ok.Response.ProductCode.IsEmpty)
        {
            productCode = ok.Response.ProductCode;
            error = null;
            return true;
        }

        productCode = ProductCode.Empty;
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

    private static Call<TReq, TOk> CreateImmediateError<TReq, TOk>(
        TReq request,
        string component,
        CallError error)
    {
        var now = DateTimeOffset.UtcNow;
        var meta = CallMeta.CreateInternal("Normalized", component);
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

    private static string Component(string endpointId) => $"Bitflyer.{endpointId}";

    private readonly record struct MapResult<TOk>(TOk? Value, CallError? Error)
    {
        public static MapResult<TOk> Ok(TOk value) => new(value, null);
        public static MapResult<TOk> Fail(CallError error) => new(default, error);
    }

}
