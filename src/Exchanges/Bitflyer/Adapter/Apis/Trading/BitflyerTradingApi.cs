using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ContractSide = ExchangeApi.Common.Enums.Side;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
public sealed class BitflyerTradingApi : ITradingApi
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
        decimal size,
        decimal price,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Limit(symbol, side, size, price), cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        ContractSide side,
        decimal size,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Market(symbol, side, size), cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        ContractSide side,
        decimal size,
        decimal triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, "StopOrder");

    private async Task<OrderResult> PlaceOrderInternal(
        OrderRequest request,
        CancellationToken cancellationToken)
    {
        BitflyerTradingMapper.ValidateOrderRequest(request);

        try
        {
            var dto = new BitflyerSendChildOrderRequest
            {
                ProductCode = BitflyerCommonMapper.MapSymbolToProductCode(request.Symbol),
                Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
                ChildOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price),
                Size = request.Size,
                Price = request.Price,
                TriggerPrice = request.TriggerPrice,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce),
            };

            var response = await _privateTradingApi
                .PlaceChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            return new OrderResult(response.ChildOrderAcceptanceId);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "SendOrder");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer sendchildorder API.",
                exchange: _exchange,
                operation: "SendOrder",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        string childOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (string.IsNullOrWhiteSpace(childOrderAcceptanceId))
        {
            throw new ArgumentException("childOrderAcceptanceId is required.", nameof(childOrderAcceptanceId));
        }

        try
        {
            var dto = new BitflyerCancelChildOrderRequest
            {
                ProductCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol),
                ChildOrderAcceptanceId = childOrderAcceptanceId,
            };

            var response = await _privateTradingApi
                .CancelChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            if (response is null)
            {
                throw new ExchangeApiException(
                    message: "bitFlyer cancelchildorder returned no response.",
                    exchange: _exchange,
                    operation: "CancelOrder",
                    statusCode: null);
            }

            return new CancelResult(true);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "CancelOrder");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer cancelchildorder API.",
                exchange: _exchange,
                operation: "CancelOrder",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var rawOrders = await _privateAccountApi
                .GetOrdersAsync(BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol)), childOrderStatusState: "ACTIVE", childOrderAcceptanceId: null, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var mapped = rawOrders.Select(o => new OpenOrder(
                ExchangeCode: ExchangeCode.Bitflyer,
                Symbol: BitflyerCommonMapper.ToSymbol(BitflyerCommonMapper.ToApiProductCode(o.ProductCode)),
                OrderId: o.ChildOrderId,
                Side: BitflyerCommonMapper.MapSide(o.Side),
                OrderType: BitflyerTradingMapper.MapOrderTypeFromExchange(o.ChildOrderType),
                Size: o.Size,
                OutstandingSize: o.OutstandingSize,
                ExecutedSize: o.ExecutedSize,
                Price: o.Price == 0 ? null : o.Price,
                OrderedAt: o.ChildOrderDate,
                UpdatedAt: null,
                StopPrice: null,
                Status: o.ChildOrderStatusState)).ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetOpenOrders");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getchildorders API.",
                exchange: _exchange,
                operation: "GetOpenOrders",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var orders = await _privateAccountApi
            .GetOrdersAsync(BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol)), childOrderStatusState: null, orderId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var order = orders.FirstOrDefault();

        if (order is null)
        {
            var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
            return new OrderStatus(
                ProductCode: productCode,
                OrderAcceptanceId: orderId,
                Status: OrderState.Completed,
                ExecutedSize: 0m,
                OutstandingSize: 0m,
                Price: null,
                AveragePrice: null);
        }

        var status = BitflyerCommonMapper.MapOrderStatus(order.ChildOrderStatusState);
        return new OrderStatus(
            ProductCode: BitflyerCommonMapper.ToApiProductCode(order.ProductCode),
            OrderAcceptanceId: order.ChildOrderAcceptanceId,
            Status: status,
            ExecutedSize: order.ExecutedSize,
            OutstandingSize: order.OutstandingSize,
            Price: order.Price == 0 ? null : order.Price,
            AveragePrice: order.AveragePrice == 0 ? null : order.AveragePrice);
    }
}
