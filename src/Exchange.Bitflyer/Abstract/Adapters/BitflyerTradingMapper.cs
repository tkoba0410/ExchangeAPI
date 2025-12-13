using System;
using Exchange.Bitflyer.Raw;
using Common.Contract.Dtos;
using ContractTimeInForce = Common.Contract.Dtos.TimeInForce;
using RawTimeInForce = Exchange.Bitflyer.Raw.TimeInForce;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerTradingMapper
{
    public static ChildOrderType MapOrderType(OrderType orderType, decimal? price) =>
        orderType switch
        {
            OrderType.Market => ChildOrderType.Market,
            OrderType.Limit => ChildOrderType.Limit,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, "Unsupported child_order_type. Only LIMIT/MARKET are accepted."),
        };

    public static OrderType MapOrderTypeFromExchange(ChildOrderType childOrderType) =>
        childOrderType switch
        {
            ChildOrderType.Limit => OrderType.Limit,
            ChildOrderType.Market => OrderType.Market,
            _ => OrderType.Market,
        };

    public static RawTimeInForce? MapTimeInForce(ContractTimeInForce? tif) =>
        tif switch
        {
            ContractTimeInForce.Gtc => RawTimeInForce.Gtc,
            ContractTimeInForce.Ioc => RawTimeInForce.Ioc,
            ContractTimeInForce.Fok => RawTimeInForce.Fok,
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
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Stop orders are not supported on sendchildorder. Use parent orders.");
            default:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Unsupported order type.");
        }
    }
}
