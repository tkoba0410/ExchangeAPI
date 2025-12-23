using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ContractSide = ExchangeApi.Common.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
internal sealed class BitflyerTradingApi : ITradingApi
{
    private readonly IBitflyerPrivateTradingApi _privateTradingApi;
    private readonly IBitflyerPrivateApi _privateAccountApi;
    private readonly ExchangeCode _exchange;

    public BitflyerTradingApi(
        IBitflyerPrivateTradingApi privateTradingApi,
        IBitflyerPrivateApi privateAccountApi,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        _privateAccountApi = privateAccountApi ?? throw new ArgumentNullException(nameof(privateAccountApi));
        _exchange = exchange;
    }

    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Limit(symbol, side, size, price), cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Market(symbol, side, size), cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, "StopOrder");

    private async Task<OrderResult> PlaceOrderInternal(
        OrderRequest request,
        CancellationToken cancellationToken)
    {
        var operation = BitflyerOperations.Trading.PlaceOrder;
        BitflyerTradingMapper.ValidateOrderRequest(request);

        try
        {
            var dto = new CreateChildOrderRequest
            {
                ProductCode = BitflyerCommonMapper.MapSymbolToProductCode(request.Symbol),
                Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
                ChildOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price),
                Size = request.Size.Value,
                Price = request.Price?.Value,
                TriggerPrice = request.TriggerPrice?.Value,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce),
            };

            var response = await _privateTradingApi
                .CreateChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            var acceptanceId = response.ChildOrderAcceptanceId;
            var key = new OrderKey(OrderIdKind.AcceptanceId, acceptanceId);
            return new OrderResult(key, AcceptanceId: acceptanceId);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer sendchildorder API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Trading.CancelOrder;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            CancelChildOrderRequest dto;
            switch (orderKey.Kind)
            {
                case OrderIdKind.AcceptanceId:
                    dto = new CancelChildOrderRequest
                    {
                        ProductCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol),
                        ChildOrderAcceptanceId = orderKey.Value,
                    };
                    break;
                case OrderIdKind.ExchangeOrderId:
                    dto = new CancelChildOrderRequest
                    {
                        ProductCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol),
                        ChildOrderId = orderKey.Value,
                    };
                    break;
                default:
                    throw new ExchangeFeatureNotSupportedException(_exchange, $"CancelOrderBy{orderKey.Kind}");
            }

            var response = await _privateTradingApi
                .CancelChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            if (response is null)
            {
                throw new ExchangeApiException(
                    message: "bitFlyer cancelchildorder returned no response.",
                    exchange: _exchange,
                    operation: operation,
                    statusCode: null);
            }

            return new CancelResult(true);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer cancelchildorder API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Trading.GetOpenOrders;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var rawOrders = await _privateAccountApi
                .GetChildOrdersAsync(BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol)), childOrderStatusState: "ACTIVE", childOrderAcceptanceId: null, cancellationToken: cancellationToken)
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
                            exchange: _exchange,
                            operation: operation);

                return new OpenOrder(
                    ExchangeCode: ExchangeCode.Bitflyer,
                    Symbol: BitflyerCommonMapper.ToSymbol(BitflyerCommonMapper.ToApiProductCode(o.ProductCode)),
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
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getchildorders API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Trading.GetOrder;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        IReadOnlyList<ChildOrderResponse> orders;
        var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
        try
        {
            switch (orderKey.Kind)
            {
                case OrderIdKind.AcceptanceId:
                    orders = await _privateAccountApi
                        .GetChildOrdersAsync(productCode, childOrderStatusState: null, childOrderAcceptanceId: orderKey.Value, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                    break;
                case OrderIdKind.ExchangeOrderId:
                    orders = await _privateAccountApi
                        .GetChildOrdersAsync(productCode, childOrderStatusState: null, childOrderId: orderKey.Value, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                    break;
                default:
                    throw new ExchangeFeatureNotSupportedException(_exchange, $"GetOrderBy{orderKey.Kind}");
            }

            var order = orders.FirstOrDefault();

            if (order is null)
            {
                throw new ExchangeOrderNotFoundException(_exchange, operation, symbol.ToString(), orderKey.ToString());
            }

            var status = BitflyerCommonMapper.MapOrderStatus(order.ChildOrderStatusState);
            var resolvedKey = !string.IsNullOrWhiteSpace(order.ChildOrderAcceptanceId)
                ? new OrderKey(OrderIdKind.AcceptanceId, order.ChildOrderAcceptanceId)
                : new OrderKey(OrderIdKind.ExchangeOrderId, order.ChildOrderId);
            return new OrderStatus(
                ProductCode: BitflyerCommonMapper.ToApiProductCode(order.ProductCode),
                Key: resolvedKey,
                Status: status,
                ExecutedSize: new Size(order.ExecutedSize),
                OutstandingSize: new Size(order.OutstandingSize),
                Price: order.Price == 0 ? null : new Price(order.Price),
                AveragePrice: order.AveragePrice == 0 ? null : new Price(order.AveragePrice));
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
    }
}
