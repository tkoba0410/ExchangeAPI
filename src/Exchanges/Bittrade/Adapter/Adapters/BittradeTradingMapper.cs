using System;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;

internal static class BittradeTradingMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static BittradeWireCreateOrderRequest ToWire(string apiSymbol, OrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(apiSymbol))
        {
            throw new ArgumentException("apiSymbol is required.", nameof(apiSymbol));
        }

        var side = MapSide(request.Side);
        var type = MapOrderType(request.Side, request.OrderType);
        var price = request.Price?.Value;
        var size = request.Size.Value;

        return new BittradeWireCreateOrderRequest(
            Symbol: apiSymbol,
            Side: side,
            Type: type,
            Price: price,
            Size: size);
    }

    public static OrderResult ToOrderResult(BittradeWireOrder wire)
    {
        var key = new OrderKey(OrderIdKind.ExchangeOrderId, wire.OrderId);
        return new OrderResult(key, ExchangeOrderId: wire.OrderId);
    }

    public static OpenOrder ToOpenOrder(Symbol symbol, BittradeWireOpenOrder wire)
    {
        var (side, type) = ParseSideAndType(wire.Side, wire.Type);
        var status = ParseStatus(wire.State);
        var price = wire.Price is null ? (Price?)null : new Price(wire.Price.Value);
        var size = new Size(wire.Size);
        var executed = new Size(wire.FilledSize);
        var outstanding = new Size(Math.Max(0m, wire.Size - wire.FilledSize));

        return new OpenOrder(
            ExchangeCode: Exchange,
            Symbol: symbol,
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, wire.OrderId),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: executed,
            Price: price,
            OrderedAt: wire.CreatedAt,
            UpdatedAt: null,
            StopPrice: null,
            Status: wire.State,
            ExchangeOrderId: wire.OrderId);
    }

    public static OrderStatus ToOrderStatus(string productCode, BittradeWireOrder wire, OrderKey key)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var status = ParseStatus(wire.State);
        var price = wire.Price is null ? (Price?)null : new Price(wire.Price.Value);
        var executed = new Size(wire.FilledSize ?? 0m);
        var outstanding = new Size(wire.OutstandingSize ?? wire.Size);

        return new OrderStatus(
            productCode,
            key,
            status,
            executed,
            outstanding,
            price,
            null);
    }

    private static string MapSide(Side side) =>
        side switch
        {
            Side.Buy => "buy",
            Side.Sell => "sell",
            _ => throw new ExchangeApiException($"Unsupported side: {side}.", exchange: Exchange)
        };

    private static string MapOrderType(Side side, OrderType type)
    {
        return (side, type) switch
        {
            (Side.Buy, OrderType.Market) => "buy-market",
            (Side.Sell, OrderType.Market) => "sell-market",
            (Side.Buy, OrderType.Limit) => "buy-limit",
            (Side.Sell, OrderType.Limit) => "sell-limit",
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };
    }

    private static (Side Side, OrderType OrderType) ParseSideAndType(string side, string type)
    {
        var parsedSide = side.ToLowerInvariant() switch
        {
            "buy" => Side.Buy,
            "sell" => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported side: {side}.", exchange: Exchange)
        };

        var parsedType = type.ToLowerInvariant() switch
        {
            var value when value.Contains("market", StringComparison.OrdinalIgnoreCase) => OrderType.Market,
            var value when value.Contains("limit", StringComparison.OrdinalIgnoreCase) => OrderType.Limit,
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };

        return (parsedSide, parsedType);
    }

    private static ExchangeApi.Common.Enums.OrderState ParseStatus(string? state)
    {
        return state switch
        {
            "submitted" => ExchangeApi.Common.Enums.OrderState.Active,
            "partial-filled" => ExchangeApi.Common.Enums.OrderState.Active,
            "filled" => ExchangeApi.Common.Enums.OrderState.Completed,
            "partial-canceled" => ExchangeApi.Common.Enums.OrderState.Canceled,
            "canceled" => ExchangeApi.Common.Enums.OrderState.Canceled,
            _ => throw new ExchangeApiException($"Unsupported order state: {state}.", exchange: Exchange)
        };
    }
}
