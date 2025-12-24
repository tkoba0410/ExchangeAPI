using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
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
    private readonly IBitflyerWireTradingApi _tradingApi;
    private readonly IBitflyerWireAccountApi _accountApi;
    private readonly IExchangeMarketResolver _markets;
    private readonly ExchangeCode _exchange;

    public BitflyerTradingApi(
        IBitflyerWireTradingApi tradingApi,
        IBitflyerWireAccountApi accountApi,
        IExchangeMarketResolver markets,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
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
                ProductCode = await ToProductCodeAsync(request.Symbol, cancellationToken).ConfigureAwait(false),
                Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
                ChildOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price),
                Size = request.Size.Value,
                Price = request.Price?.Value,
                TriggerPrice = request.TriggerPrice?.Value,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce),
            };

            var response = await _tradingApi
                .CreateChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            var acceptanceId = response.ChildOrderAcceptanceId;
            var key = new OrderKey(OrderIdKind.AcceptanceId, acceptanceId);
            return new OrderResult(key, AcceptanceId: acceptanceId);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
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
                    throw new ExchangeFeatureNotSupportedException(_exchange, $"CancelOrderBy{orderKey.Kind}");
            }

            var response = await _tradingApi
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
        catch (SymbolNotSupportedException)
        {
            throw;
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
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
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
                            exchange: _exchange,
                            operation: operation);

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
        catch (SymbolNotSupportedException)
        {
            throw;
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
        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
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
                ProductCode: productCode,
                Key: resolvedKey,
                Status: status,
                ExecutedSize: new Size(order.ExecutedSize),
                OutstandingSize: new Size(order.OutstandingSize),
                Price: order.Price == 0 ? null : new Price(order.Price),
                AveragePrice: order.AveragePrice == 0 ? null : new Price(order.AveragePrice));
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
    }

    private async Task<string> ToApiProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode;
    }

    private async Task<ExchangeApi.Exchanges.Bitflyer.Raw.ProductCode> ToProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var productCode = await ToApiProductCodeAsync(symbol, ct).ConfigureAwait(false);
        return BitflyerCommonMapper.ParseProductCode(productCode);
    }
}
