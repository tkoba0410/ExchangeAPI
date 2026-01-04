using System;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;
using ContractTimeInForce = ExchangeApi.Common.Enums.TimeInForce;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

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

    public static BitflyerTimeInForce? MapTimeInForce(ContractTimeInForce? tif) =>
        tif switch
        {
            ContractTimeInForce.Gtc => BitflyerTimeInForce.Gtc,
            ContractTimeInForce.Ioc => BitflyerTimeInForce.Ioc,
            ContractTimeInForce.Fok => BitflyerTimeInForce.Fok,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(tif), tif, "Unknown bitFlyer time_in_force"),
        };

    public static BitflyerTimeInForce ParseTimeInForce(string tif) =>
        (tif ?? string.Empty).ToUpperInvariant() switch
        {
            "GTC" => BitflyerTimeInForce.Gtc,
            "IOC" => BitflyerTimeInForce.Ioc,
            "FOK" => BitflyerTimeInForce.Fok,
            _ => throw new ArgumentOutOfRangeException(nameof(tif), tif, "Unknown bitFlyer time_in_force"),
        };

    public static string? ToApiTimeInForce(BitflyerTimeInForce? tif) =>
        tif switch
        {
            BitflyerTimeInForce.Gtc => "GTC",
            BitflyerTimeInForce.Ioc => "IOC",
            BitflyerTimeInForce.Fok => "FOK",
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(tif), tif, "Unknown bitFlyer time_in_force"),
        };

    public static void ValidateOrderRequest(OrderRequest request)
    {
        if (request.Symbol.IsEmpty)
            throw new ArgumentException("Symbol is required.", nameof(request));
        if (request.Size.Value <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(request));
        if (request.MinuteToExpire is { } mte && mte <= 0)
            throw new ArgumentException("MinuteToExpire must be positive when specified.", nameof(request));
        if (request.Price is { } price && price.Value <= 0)
            throw new ArgumentException("Price must be greater than zero when specified.", nameof(request));
        if (request.TriggerPrice is { } tp && tp.Value <= 0)
            throw new ArgumentException("TriggerPrice must be greater than zero when specified.", nameof(request));

        switch (request.OrderType)
        {
            case OrderType.Market:
                if (request.Price is not null || request.TriggerPrice is not null)
                    throw new ArgumentException("Market order must not specify Price or TriggerPrice.", nameof(request));
                break;
            case OrderType.Limit:
                if (request.Price is null)
                    throw new ArgumentException("Limit order requires Price.", nameof(request));
                if (request.TriggerPrice is not null)
                    throw new ArgumentException("Limit order must not specify TriggerPrice.", nameof(request));
                break;
            case OrderType.Stop:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Stop orders are not supported on sendchildorder. Use parent orders.");
            default:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Unsupported order type.");
        }
    }
}
