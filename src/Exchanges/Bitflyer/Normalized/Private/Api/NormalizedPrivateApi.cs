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
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Api;

internal sealed class NormalizedPrivateApi
{
    private readonly IRawApi _raw;
    private readonly IMarketResolver _markets;

    public NormalizedPrivateApi(
        IRawApi raw,
        IMarketResolver markets)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (!TradingMapper.TryValidateOrderRequest(request, out var validationError))
        {
            return CreateImmediateError<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>(
                new PrivateRequests.SendChildOrderRequest(request),
                "Bitflyer.CreateChildOrder",
                validationError!);
        }

        var callRequest = new PrivateRequests.SendChildOrderRequest(request);
        var marketCall = await _markets.ResolveCallAsync(request.Symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>(
                marketCall,
                callRequest,
                "Bitflyer.CreateChildOrder",
                marketError!);
        }

        if (!TradingMapper.TryMapOrderType(request.OrderType, request.Price, out var childOrderType, out var orderTypeError))
        {
            return CreateImmediateError<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>(
                callRequest,
                "Bitflyer.CreateChildOrder",
                orderTypeError!);
        }

        if (!CommonMapper.TryMapSideToExchange(request.Side, out var apiSide, out var sideError))
        {
            return CreateImmediateError<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>(
                callRequest,
                "Bitflyer.CreateChildOrder",
                sideError!);
        }

        if (!TradingMapper.TryToApiChildOrderType(childOrderType, out var apiChildOrderType, out var childOrderError))
        {
            return CreateImmediateError<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>(
                callRequest,
                "Bitflyer.CreateChildOrder",
                childOrderError!);
        }

        var dto = new RawPrivateRequests.SendChildOrderRequest
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
            ok => MapResult<SendChildOrderResponse>.Ok(
                new SendChildOrderResponse(
                    new OrderKey(OrderIdKind.AcceptanceId, ok.ChildOrderAcceptanceId),
                    AcceptanceId: new AcceptanceId(ok.ChildOrderAcceptanceId))));
    }

    public async Task<Call<PrivateRequests.CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.CancelChildOrderRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.CancelChildOrderRequest, CancelChildOrderResponse>(
                callRequest,
                Component(EndpointIds.CancelChildOrder),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.CancelChildOrderRequest, CancelChildOrderResponse>(
                marketCall,
                callRequest,
                Component(EndpointIds.CancelChildOrder),
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
                return CreateNotSupported<PrivateRequests.CancelChildOrderRequest, CancelChildOrderResponse>(
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
            Component(EndpointIds.CancelChildOrder),
            _ => MapResult<CancelChildOrderResponse>.Ok(new CancelChildOrderResponse(true)));
    }

    public async Task<Call<PrivateRequests.CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.CancelAllChildOrdersRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>(
                callRequest,
                Component(EndpointIds.CancelAllChildOrders),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>(
                marketCall,
                callRequest,
                Component(EndpointIds.CancelAllChildOrders),
                marketError!);
        }

        var rawCall = await _raw
            .CancelAllChildOrdersCallAsync(new RawPrivateRequests.CancelAllChildOrdersRequest { ProductCode = productCode }, cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(EndpointIds.CancelAllChildOrders),
            _ => MapResult<CancelAllChildOrdersResponse>.Ok(new CancelAllChildOrdersResponse(true)));
    }

    public async Task<Call<PrivateRequests.GetChildOrdersRequest, GetChildOrdersResponse>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var callRequest = new PrivateRequests.GetChildOrdersRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetChildOrdersRequest, GetChildOrdersResponse>(
                callRequest,
                Component(EndpointIds.GetChildOrders),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<PrivateRequests.GetChildOrdersRequest, GetChildOrdersResponse>(
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
                var mapped = new List<OpenOrder>(rawOrders.Count);
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
                            return MapResult<GetChildOrdersResponse>.Fail(
                            new CallError(CallErrorKind.Mapping, "bitFlyer order is missing both acceptanceId and exchangeOrderId."));
                    }

                    var key = acceptanceId is not null
                        ? new OrderKey(OrderIdKind.AcceptanceId, acceptanceId.Value.ToString())
                        : new OrderKey(OrderIdKind.ExchangeOrderId, exchangeOrderId!.Value.ToString());

                    if (!CommonMapper.TryMapSide(o.Side, out var side, out var sideError))
                    {
                        return MapResult<GetChildOrdersResponse>.Fail(sideError!);
                    }

                    if (!TradingMapper.TryParseChildOrderType(o.ChildOrderType, out var parsedOrderType, out var orderTypeError))
                    {
                        return MapResult<GetChildOrdersResponse>.Fail(orderTypeError!);
                    }

                    if (!TradingMapper.TryToOrderType(parsedOrderType, out var mappedOrderType, out var mapError))
                    {
                        return MapResult<GetChildOrdersResponse>.Fail(mapError!);
                    }

                    mapped.Add(new OpenOrder(
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

                return MapResult<GetChildOrdersResponse>.Ok(
                    new GetChildOrdersResponse(mapped.Select(static x => new GetChildOrdersItem(x)).ToArray()));
            });
    }

    public async Task<Call<PrivateRequests.SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        var rawParameters = new List<RawPrivateRequests.CreateParentOrderParameter>(request.Parameters.Count);
        foreach (var p in request.Parameters)
        {
            if (!ParentOrderMapper.TryToApiConditionType(p.ConditionType, out var conditionType, out var conditionError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, SendParentOrderResponse>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    conditionError!);
            }

            if (!SideMapper.TryToApi(p.Side, out var side, out var sideError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, SendParentOrderResponse>(
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
            if (!ParentOrderMapper.TryToApiOrderMethod(request.OrderMethod.Value, out var method, out var methodError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, SendParentOrderResponse>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    methodError!);
            }

            orderMethod = method;
        }

        string? timeInForce = null;
        if (request.TimeInForce.HasValue)
        {
            if (!ParentOrderMapper.TryToApiTimeInForce(request.TimeInForce.Value, out var tif, out var tifError))
            {
                return CreateImmediateError<PrivateRequests.SendParentOrderRequest, SendParentOrderResponse>(
                    callRequest,
                    "Bitflyer.CreateParentOrder",
                    tifError!);
            }

            timeInForce = tif;
        }

        var rawRequest = new RawPrivateRequests.SendParentOrderRequest
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
            ok => MapResult<SendParentOrderResponse>.Ok(
                new SendParentOrderResponse(new ParentOrderAcceptance(new AcceptanceId(ok.ParentOrderAcceptanceId)))));
    }

    public async Task<Call<PrivateRequests.CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
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
            Component(EndpointIds.CancelParentOrder),
            _ => MapResult<CancelParentOrderResponse>.Ok(new CancelParentOrderResponse(new ParentOrderCancelResult(true))));
    }

    public async Task<Call<PrivateRequests.GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var callRequest = request;
        string? parentOrderState = null;
        if (request.ParentOrderState.HasValue)
        {
            if (!ParentOrderMapper.TryToApiParentOrderState(request.ParentOrderState.Value, out var state, out var stateError))
            {
                return CreateImmediateError<PrivateRequests.GetParentOrdersRequest, GetParentOrdersResponse>(
                    callRequest,
                    Component(EndpointIds.GetParentOrders),
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
            Component(EndpointIds.GetParentOrders),
            ok =>
            {
                if (!ParentOrderNormalizer.TryNormalizeList(ok, rawCall.Meta.RawJson, out var normalized, out var error))
                {
                    return MapResult<GetParentOrdersResponse>.Fail(error!);
                }

                return MapResult<GetParentOrdersResponse>.Ok(
                    new GetParentOrdersResponse(normalized!.Select(static x => new GetParentOrdersItem(x)).ToArray()));
            });
    }

    public async Task<Call<PrivateRequests.GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
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
            Component(EndpointIds.GetParentOrder),
            ok =>
            {
                if (!ParentOrderNormalizer.TryNormalizeDetail(ok, rawCall.Meta.RawJson, out var normalized, out var error))
                {
                    return MapResult<GetParentOrderResponse>.Fail(error!);
                }

                return MapResult<GetParentOrderResponse>.Ok(new GetParentOrderResponse(normalized!));
            });
    }

    public async Task<Call<PrivateRequests.GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceCallAsync(new RawPrivateRequests.GetBalanceRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalanceRequest();
        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetBalances",
            raw =>
            {
                if (!AccountMapper.TryMapBalances(raw, out var balances, out var mapError))
                {
                    return MapResult<GetBalanceResponse>.Fail(mapError!);
                }

                return MapResult<GetBalanceResponse>.Ok(
                    new GetBalanceResponse(balances!.Select(static x => new GetBalanceItem(x)).ToArray()));
            });
    }

    public async Task<Call<PrivateRequests.GetPermissionsRequest, GetPermissionsResponse>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetPermissionsCallAsync(new RawPrivateRequests.GetPermissionsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetPermissionsRequest();
        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetPermissions),
            raw => MapResult<GetPermissionsResponse>.Ok(
                new GetPermissionsResponse(raw.Select(static x => new GetPermissionsItem(x)).ToArray())));
    }

    public async Task<Call<PrivateRequests.GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralCallAsync(new RawPrivateRequests.GetCollateralRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralRequest();
        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetCollateral),
            raw => MapResult<GetCollateralResponse>.Ok(
                new GetCollateralResponse(new CollateralNormalized(
                    raw.Collateral,
                    raw.OpenPositionPnl,
                    raw.RequireCollateral,
                    raw.KeepRate))));
    }

    public async Task<Call<PrivateRequests.GetCollateralAccountsRequest, GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralAccountsCallAsync(new RawPrivateRequests.GetCollateralAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralAccountsRequest();
        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetCollateralAccounts),
            raw => MapResult<GetCollateralAccountsResponse>.Ok(
                new GetCollateralAccountsResponse(
                    raw.Select(item => new GetCollateralAccountsItem(
                            new CollateralAccountNormalized(CurrencyCodeConverter.FromString(item.CurrencyCode), item.Amount, item.Available)))
                        .ToArray())));
    }

    public async Task<Call<PrivateRequests.GetAddressesRequest, GetAddressesResponse>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetAddressesCallAsync(new RawPrivateRequests.GetAddressesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetAddressesRequest();
        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetAddresses),
            raw => MapResult<GetAddressesResponse>.Ok(new GetAddressesResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetCoinInsRequest, GetCoinInsResponse>> GetCoinInsCallAsync(
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
            Component(EndpointIds.GetCoinIns),
            raw => MapResult<GetCoinInsResponse>.Ok(new GetCoinInsResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetCoinOutsRequest, GetCoinOutsResponse>> GetCoinOutsCallAsync(
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
            Component(EndpointIds.GetCoinOuts),
            raw => MapResult<GetCoinOutsResponse>.Ok(new GetCoinOutsResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetBankAccountsRequest, GetBankAccountsResponse>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBankAccountsCallAsync(new RawPrivateRequests.GetBankAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBankAccountsRequest();
        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetBankAccounts),
            raw => MapResult<GetBankAccountsResponse>.Ok(new GetBankAccountsResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetDepositsRequest, GetDepositsResponse>> GetDepositsCallAsync(
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
            Component(EndpointIds.GetDeposits),
            raw => MapResult<GetDepositsResponse>.Ok(new GetDepositsResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default)
    {
        var currencyText = CurrencyCodeConverter.ToCurrencyString(currencyCode);
        var rawCall = await _raw
            .WithdrawCallAsync(new RawPrivateRequests.WithdrawRequest
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
            Component(EndpointIds.Withdraw),
            raw => MapResult<WithdrawResponse>.Ok(new WithdrawResponse(new WithdrawResult(FreeText.Parse(raw.MessageId)))));
    }

    public async Task<Call<PrivateRequests.GetWithdrawalsRequest, GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
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
            Component(EndpointIds.GetWithdrawals),
            raw => MapResult<GetWithdrawalsResponse>.Ok(new GetWithdrawalsResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetBalanceHistoryRequest, GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
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
            Component(EndpointIds.GetBalanceHistory),
            raw => MapResult<GetBalanceHistoryResponse>.Ok(new GetBalanceHistoryResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetPositionsRequest, GetPositionsResponse>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new PrivateRequests.GetPositionsRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetPositionsRequest, GetPositionsResponse>(
                request,
                Component(EndpointIds.GetPositions),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<MarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, GetPositionsResponse>(
                marketCall,
                request,
                Component(EndpointIds.GetPositions),
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<MarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : ProductCode.Empty;
        if (productCode.IsEmpty)
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, GetPositionsResponse>(
                marketCall,
                request,
                Component(EndpointIds.GetPositions),
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetPositionsCallAsync(new RawPrivateRequests.GetPositionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetPositions),
            raw =>
            {
                var mapped = new List<PositionNormalized>(raw.Count);
                foreach (var item in raw)
                {
                    if (!CommonMapper.TryMapSide(item.Side, out var side, out var sideError))
                    {
                        return MapResult<GetPositionsResponse>.Fail(sideError!);
                    }

                    mapped.Add(new PositionNormalized(
                        ProductCode.ParseNormalized(item.ProductCode),
                        side,
                        item.Size,
                        item.Price,
                        item.Pnl,
                        item.OpenDate));
                }

                return MapResult<GetPositionsResponse>.Ok(
                    new GetPositionsResponse(mapped.Select(static x => new GetPositionsItem(x)).ToArray()));
            });
    }

    public async Task<Call<PrivateRequests.GetCollateralHistoryRequest, GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
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
            Component(EndpointIds.GetCollateralHistory),
            raw => MapResult<GetCollateralHistoryResponse>.Ok(new GetCollateralHistoryResponse(FreeText.Parse(raw.RawJson))));
    }

    public async Task<Call<PrivateRequests.GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new PrivateRequests.GetExecutionsPrivateRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>(
                request,
                Component(EndpointIds.GetExecutionsPrivate),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<MarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>(
                marketCall,
                request,
                Component(EndpointIds.GetExecutionsPrivate),
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<MarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : ProductCode.Empty;
        if (productCode.IsEmpty)
        {
            return CreateCallError<PrivateRequests.GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>(
                marketCall,
                request,
                Component(EndpointIds.GetExecutionsPrivate),
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetExecutionsPrivateCallAsync(new RawPrivateRequests.GetExecutionsPrivateRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetExecutionsPrivate),
            raw =>
            {
                if (!AccountMapper.TryMapAccountExecutions(symbol, raw, out var executions, out var error))
                {
                    return MapResult<GetExecutionsPrivateResponse>.Fail(error!);
                }

                return MapResult<GetExecutionsPrivateResponse>.Ok(
                    new GetExecutionsPrivateResponse(executions!.Select(static x => new GetExecutionsPrivateItem(x)).ToArray()));
            });
    }

    public async Task<Call<PrivateRequests.GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new PrivateRequests.GetTradingCommissionRequest(symbol);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<PrivateRequests.GetTradingCommissionRequest, GetTradingCommissionResponse>(
                request,
                Component(EndpointIds.GetTradingCommission),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<MarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, GetTradingCommissionResponse>(
                marketCall,
                request,
                Component(EndpointIds.GetTradingCommission),
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<MarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : ProductCode.Empty;
        if (productCode.IsEmpty)
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, GetTradingCommissionResponse>(
                marketCall,
                request,
                Component(EndpointIds.GetTradingCommission),
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetTradingCommissionCallAsync(new RawPrivateRequests.GetTradingCommissionRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetTradingCommission),
            raw =>
            {
                if (!TryParseTradingCommission(raw.RawJson, productCode, out var parsed, out var error))
                {
                    return MapResult<GetTradingCommissionResponse>.Fail(error!);
                }

                return MapResult<GetTradingCommissionResponse>.Ok(new GetTradingCommissionResponse(parsed!));
            });
    }

    private static bool TryParseTradingCommission(
        string? rawJson,
        ProductCode productCode,
        out TradingCommissionNormalized? normalized,
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

        normalized = new TradingCommissionNormalized(
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
        Call<ResolveMarketRequest, MarketInfo> marketCall,
        out ProductCode productCode,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<MarketInfo>.Err err)
        {
            productCode = ProductCode.Empty;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<MarketInfo>.Ok ok &&
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
        Call<ResolveMarketRequest, MarketInfo> marketCall,
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
