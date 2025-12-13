using System;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Contracts.Dtos;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerTradingMapper
{
    public static string MapOrderType(OrderType orderType, decimal? price) =>
        orderType switch
        {
            OrderType.Market => BitflyerConstants.ConditionType.Market,
            OrderType.Limit => BitflyerConstants.ConditionType.Limit,
            OrderType.Stop => price is null ? BitflyerConstants.ConditionType.Stop : BitflyerConstants.ConditionType.StopLimit,
            _ => BitflyerConstants.ConditionType.Market,
        };

    public static OrderType MapOrderTypeFromExchange(string childOrderType) =>
        childOrderType.ToUpperInvariant() switch
        {
            BitflyerConstants.ConditionType.Limit => OrderType.Limit,
            BitflyerConstants.ConditionType.Market => OrderType.Market,
            BitflyerConstants.ConditionType.Stop or BitflyerConstants.ConditionType.StopLimit => OrderType.Stop,
            _ => OrderType.Market,
        };

    public static string? MapTimeInForce(TimeInForce? tif) =>
        tif switch
        {
            TimeInForce.Gtc => BitflyerConstants.TimeInForce.Gtc,
            TimeInForce.Ioc => BitflyerConstants.TimeInForce.Ioc,
            TimeInForce.Fok => BitflyerConstants.TimeInForce.Fok,
            _ => null,
        };

    public static void ValidateOrderRequest(OrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
            throw new ArgumentException("ProductCode is required.", nameof(request));
        if (request.Size <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(request));
        if (request.MinuteToExpire is { } mte && mte <= 0)
            throw new ArgumentException("MinuteToExpire must be positive when specified.", nameof(request));
        if (request.Price is { } price && price <= 0)
            throw new ArgumentException("Price must be greater than zero when specified.", nameof(request));
        if (request.TriggerPrice is { } tp && tp <= 0)
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
                if (request.TriggerPrice is null)
                    throw new ArgumentException("Stop order requires TriggerPrice.", nameof(request));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Unsupported order type.");
        }
    }
}
