using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

internal sealed class BitflyerNormalizedTradingApi : IBitflyerNormalizedTradingApi
{
    private readonly IBitflyerRawPrivateTradingApi _tradingApi;
    private readonly IBitflyerRawAccountApi _accountApi;
    private readonly IExchangeMarketResolver _markets;

    public BitflyerNormalizedTradingApi(
        IBitflyerRawPrivateTradingApi tradingApi,
        IBitflyerRawAccountApi accountApi,
        IExchangeMarketResolver markets)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BitflyerTradingMapper.ValidateOrderRequest(request);

        var dto = new CreateChildOrderRequest
        {
            ProductCode = await ToProductCodeAsync(request.Symbol, cancellationToken).ConfigureAwait(false),
            Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
            ChildOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price),
            Size = request.Size.Value,
            Price = request.Price?.Value,
            TriggerPrice = request.TriggerPrice?.Value,
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce),
        };

        var response = await _tradingApi.CreateChildOrderAsync(dto, cancellationToken).ConfigureAwait(false);
        var acceptanceId = response.ChildOrderAcceptanceId;
        var key = new OrderKey(OrderIdKind.AcceptanceId, acceptanceId);
        return new OrderResult(key, AcceptanceId: acceptanceId);
    }

    public async Task<CancelResult> CancelOrderAsync(
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

        var response = await _tradingApi.CancelChildOrderAsync(dto, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            throw new ExchangeApiException(
                message: "bitFlyer cancelchildorder returned no response.",
                exchange: ExchangeCode.Bitflyer,
                operation: "Bitflyer.CancelChildOrder",
                statusCode: null);
        }

        return new CancelResult(true);
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawOrders = await _accountApi
            .GetChildOrdersAsync(productCode, childOrderStatusState: "ACTIVE", childOrderAcceptanceId: null, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var mapped = rawOrders.Select(o =>
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
                OrderType: BitflyerTradingMapper.MapOrderTypeFromExchange(o.ChildOrderType),
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
    }

    public async Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        IReadOnlyList<ChildOrderResponse> orders;
        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        switch (orderKey.Kind)
        {
            case OrderIdKind.AcceptanceId:
                orders = await _accountApi
                    .GetChildOrdersAsync(productCode, childOrderStatusState: null, childOrderAcceptanceId: orderKey.Value, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                break;
            case OrderIdKind.ExchangeOrderId:
                orders = await _accountApi
                    .GetChildOrdersAsync(productCode, childOrderStatusState: null, childOrderId: orderKey.Value, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                break;
            default:
                throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, $"GetOrderBy{orderKey.Kind}");
        }

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
    }

    public async Task<BitflyerNormalizedCall<OrderResult, JsonElement>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BitflyerTradingMapper.ValidateOrderRequest(request);

        var dto = new CreateChildOrderRequest
        {
            ProductCode = await ToProductCodeAsync(request.Symbol, cancellationToken).ConfigureAwait(false),
            Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
            ChildOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price),
            Size = request.Size.Value,
            Price = request.Price?.Value,
            TriggerPrice = request.TriggerPrice?.Value,
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce),
        };

        var rawCall = await _tradingApi.CreateChildOrderCallAsync(dto, cancellationToken).ConfigureAwait(false);
        var requestMeta = CreateRequest("Bitflyer.CreateChildOrder", new Dictionary<string, string?>
        {
            ["symbol"] = request.Symbol.ToString(),
            ["side"] = request.Side.ToString(),
            ["orderType"] = request.OrderType.ToString(),
            ["size"] = request.Size.ToString(),
            ["price"] = request.Price?.ToString(),
            ["timeInForce"] = request.TimeInForce?.ToString(),
        });

        return CreateCall(
            rawCall,
            requestMeta,
            ok =>
            {
                var acceptanceId = ok.ChildOrderAcceptanceId;
                var key = new OrderKey(OrderIdKind.AcceptanceId, acceptanceId);
                return new OrderResult(key, AcceptanceId: acceptanceId);
            });
    }

    public async Task<BitflyerNormalizedCall<CancelResult, JsonElement>> CancelOrderCallAsync(
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

        var rawCall = await _tradingApi.CancelChildOrderCallAsync(dto, cancellationToken).ConfigureAwait(false);
        var requestMeta = CreateRequest("Bitflyer.CancelChildOrder", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["productCode"] = productCode.Value,
            ["orderKey"] = orderKey.ToString(),
        });

        return CreateCall(rawCall, requestMeta, _ => new CancelResult(true));
    }

    public async Task<BitflyerNormalizedCall<IReadOnlyList<OpenOrder>, JsonElement>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _accountApi
            .GetChildOrdersCallAsync(productCode, childOrderStatusState: "ACTIVE", childOrderAcceptanceId: null, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var requestMeta = CreateRequest("Bitflyer.GetOpenOrders", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["productCode"] = productCode.Value,
        });

        return CreateCall(
            rawCall,
            requestMeta,
            rawOrders => rawOrders.Select(o =>
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
                    OrderType: BitflyerTradingMapper.MapOrderTypeFromExchange(o.ChildOrderType),
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
            }).ToArray());
    }

    public async Task<BitflyerNormalizedCall<OrderStatus, JsonElement>> GetOrderCallAsync(
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
            OrderIdKind.AcceptanceId => await _accountApi
                .GetChildOrdersCallAsync(productCode, childOrderStatusState: null, childOrderAcceptanceId: orderKey.Value, cancellationToken: cancellationToken)
                .ConfigureAwait(false),
            OrderIdKind.ExchangeOrderId => await _accountApi
                .GetChildOrdersCallAsync(productCode, childOrderStatusState: null, childOrderId: orderKey.Value, cancellationToken: cancellationToken)
                .ConfigureAwait(false),
            _ => throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, $"GetOrderBy{orderKey.Kind}")
        };

        var requestMeta = CreateRequest("Bitflyer.GetOrder", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["productCode"] = productCode.Value,
            ["orderKey"] = orderKey.ToString(),
        });

        return CreateCall(
            rawCall,
            requestMeta,
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

    private static BitflyerNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BitflyerRawCall<TRaw, JsonElement> rawCall,
        BitflyerNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }

    private async Task<RawProductCode> ToProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return new RawProductCode(market.ProductCode);
    }
}
