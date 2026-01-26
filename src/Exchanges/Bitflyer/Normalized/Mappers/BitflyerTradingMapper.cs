using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerTradingMapper
{
    public static BitflyerChildOrderType MapOrderType(OrderType orderType, Price? price) =>
        orderType switch
        {
            OrderType.Market => BitflyerChildOrderType.Market,
            OrderType.Limit => BitflyerChildOrderType.Limit,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, "Unsupported child_order_type. Only LIMIT/MARKET are accepted."),
        };

    public static string ToApiChildOrderType(BitflyerChildOrderType childOrderType) =>
        childOrderType switch
        {
            BitflyerChildOrderType.Market => "MARKET",
            BitflyerChildOrderType.Limit => "LIMIT",
            _ => throw new ArgumentOutOfRangeException(nameof(childOrderType), childOrderType, "Unknown bitFlyer child_order_type"),
        };

    public static BitflyerChildOrderType ParseChildOrderType(string childOrderType) =>
        (childOrderType ?? string.Empty).ToUpperInvariant() switch
        {
            "LIMIT" => BitflyerChildOrderType.Limit,
            "MARKET" => BitflyerChildOrderType.Market,
            _ => throw new ArgumentOutOfRangeException(nameof(childOrderType), childOrderType, "Unknown bitFlyer child_order_type"),
        };

    public static OrderType ToOrderType(BitflyerChildOrderType childOrderType) =>
        childOrderType switch
        {
            BitflyerChildOrderType.Limit => OrderType.Limit,
            BitflyerChildOrderType.Market => OrderType.Market,
            _ => throw new ArgumentOutOfRangeException(nameof(childOrderType), childOrderType, "Unknown bitFlyer child_order_type"),
        };

    public static void ValidateOrderRequest(BitflyerOrderRequest request)
    {
        if (request.Symbol.IsEmpty)
            throw new ArgumentException("Symbol is required.", nameof(request));
        if (request.Size.Value <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(request));
        if (request.Price is { } price && price.Value <= 0)
            throw new ArgumentException("Price must be greater than zero when specified.", nameof(request));

        switch (request.OrderType)
        {
            case OrderType.Market:
                if (request.Price is not null)
                    throw new ArgumentException("Market order must not specify Price.", nameof(request));
                break;
            case OrderType.Limit:
                if (request.Price is null)
                    throw new ArgumentException("Limit order requires Price.", nameof(request));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Unsupported order type.");
        }
    }
}
