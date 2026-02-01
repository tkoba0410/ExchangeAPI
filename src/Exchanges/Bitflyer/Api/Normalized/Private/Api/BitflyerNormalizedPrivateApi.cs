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
        var dto = new RawPrivateRequests.CreateChildOrderRequest
        {
            ProductCode = productCode!,
            Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
            ChildOrderType = BitflyerTradingMapper.ToApiChildOrderType(childOrderType),
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

        RawPrivateRequests.CancelChildOrderRequest dto;
        switch (orderKey.Kind)
        {
            case OrderIdKind.AcceptanceId:
                dto = new RawPrivateRequests.CancelChildOrderRequest
                {
                    ProductCode = productCode!,
                    ChildOrderAcceptanceId = orderKey.Value,
                };
                break;
            case OrderIdKind.ExchangeOrderId:
                dto = new RawPrivateRequests.CancelChildOrderRequest
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
            .CancelAllChildOrdersCallAsync(new RawPrivateRequests.CancelAllChildOrdersRequest { ProductCode = productCode! }, cancellationToken)
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
                new RawPrivateRequests.GetChildOrdersRequest(
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
                    new RawPrivateRequests.GetChildOrdersRequest(
                        productCode!,
                        ChildOrderStatusState: null,
                        ChildOrderAcceptanceId: orderKey.Value),
                    cancellationToken)
                .ConfigureAwait(false)
            : await _raw
                .GetChildOrdersCallAsync(
                    new RawPrivateRequests.GetChildOrdersRequest(
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
        var rawParameters = request.Parameters.Select(p => new RawPrivateRequests.CreateParentOrderParameter
        {
            ProductCode = p.ProductCode,
            ConditionType = BitflyerParentOrderMapper.ToApiConditionType(p.ConditionType),
            Side = BitflyerSideMapper.ToApi(p.Side),
            Price = p.Price?.Value,
            Size = p.Size.Value,
            TriggerPrice = p.TriggerPrice?.Value,
            Offset = p.Offset,
        }).ToArray();

        var rawRequest = new RawPrivateRequests.CreateParentOrderRequest
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

        var rawCall = await _raw
            .SendParentOrderCallAsync(rawRequest, cancellationToken)
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
        var rawRequest = new RawPrivateRequests.CancelParentOrderRequest
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
        var rawRequest = new RawPrivateRequests.GetParentOrdersRequest(
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
        var rawRequest = new RawPrivateRequests.GetParentOrderRequest(
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

    public async Task<Call<PrivateRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceCallAsync(new RawPrivateRequests.GetBalancesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalancesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBalances", BitflyerAccountMapper.MapBalances);
    }

    public async Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetPermissionsCallAsync(new RawPrivateRequests.GetPermissionsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetPermissionsRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetPermissions", raw => raw);
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
            "Bitflyer.GetCollateral",
            raw => new BitflyerCollateralNormalized(raw.Collateral, raw.OpenPositionPnl, raw.RequireCollateral, raw.KeepRate));
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
            "Bitflyer.GetCollateralAccounts",
            raw =>
            {
                IReadOnlyList<BitflyerCollateralAccountNormalized> mapped = raw
                    .Select(item => new BitflyerCollateralAccountNormalized(item.CurrencyCode, item.Amount, item.Available))
                    .ToArray();
                return mapped;
            });
    }

    public async Task<Call<PrivateRequests.GetAddressesRequest, BitflyerRawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetAddressesCallAsync(new RawPrivateRequests.GetAddressesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetAddressesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetAddresses", raw => new BitflyerRawJsonNormalized(raw.RawJson));
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
        return CreateCall(rawCall, request, "Bitflyer.GetCoinIns", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetCoinOutsRequest, BitflyerRawJsonNormalized>> GetCoinOutsCallAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCoinOutsCallAsync(new RawPrivateRequests.GetCoinOutsRequest(messageId, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCoinOutsRequest(messageId, count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetCoinOuts", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetBankAccountsRequest, BitflyerRawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBankAccountsCallAsync(new RawPrivateRequests.GetBankAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBankAccountsRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBankAccounts", raw => new BitflyerRawJsonNormalized(raw.RawJson));
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
        return CreateCall(rawCall, request, "Bitflyer.GetDeposits", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.WithdrawRequest, BitflyerWithdrawResultNormalized>> WithdrawCallAsync(
        string currencyCode,
        int bankAccountId,
        decimal amount,
        string? code = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .WithdrawCallAsync(new RawPrivateRequests.CreateWithdrawalRequest
            {
                CurrencyCode = currencyCode,
                BankAccountId = bankAccountId,
                Amount = amount,
                Code = code,
            }, cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.WithdrawRequest(currencyCode, bankAccountId, amount, code);
        return CreateCall(rawCall, request, "Bitflyer.Withdraw", raw => new BitflyerWithdrawResultNormalized(raw.MessageId));
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
        return CreateCall(rawCall, request, "Bitflyer.GetWithdrawals", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceHistoryCallAsync(new RawPrivateRequests.GetBalanceHistoryRequest(currencyCode, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalanceHistoryRequest(currencyCode, count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetBalanceHistory", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new PrivateRequests.GetPositionsRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetPositions",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetPositions",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetPositionsCallAsync(new RawPrivateRequests.GetPositionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetPositions",
            raw =>
            {
                IReadOnlyList<BitflyerPositionNormalized> mapped = raw
                    .Select(item => new BitflyerPositionNormalized(
                        item.ProductCode,
                        BitflyerCommonMapper.MapSide(item.Side),
                        item.Size,
                        item.Price,
                        item.Pnl,
                        item.OpenDate))
                    .ToArray();
                return mapped;
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
        return CreateCall(rawCall, request, "Bitflyer.GetCollateralHistory", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new PrivateRequests.GetAccountExecutionsRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutionsPrivate",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutionsPrivate",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetExecutionsPrivateCallAsync(new RawPrivateRequests.GetAccountExecutionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutionsPrivate",
            raw => BitflyerAccountMapper.MapAccountExecutions(symbol, raw));
    }

    public async Task<Call<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new PrivateRequests.GetTradingCommissionRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetTradingCommissionCallAsync(new RawPrivateRequests.GetTradingCommissionRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetTradingCommission",
            raw => ParseTradingCommission(raw.RawJson, productCode));
    }

    private static BitflyerTradingCommissionNormalized ParseTradingCommission(string? rawJson, string productCode)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new InvalidOperationException("Trading commission response is empty.");
        }

        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;
        string? parsedProductCode = null;
        decimal? commissionRate = null;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("product_code", out var productCodeElement)
                && productCodeElement.ValueKind == JsonValueKind.String)
            {
                parsedProductCode = productCodeElement.GetString();
            }

            if (root.TryGetProperty("commission_rate", out var commissionElement))
            {
                commissionRate = TryParseDecimal(commissionElement);
            }
        }

        return new BitflyerTradingCommissionNormalized(
            ProductCode: parsedProductCode ?? productCode,
            CommissionRate: commissionRate);
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
