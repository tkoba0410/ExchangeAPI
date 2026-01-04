using System;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

internal static class BitflyerRawMappers
{
    public static RawSendChildOrderRequest MapSendChildOrderRequest(PrivatePost.CreateChildOrderRequest request) => new()
    {
        ProductCode = MapProductCode(request.ProductCode),
        ChildOrderType = MapChildOrderType(request.ChildOrderType),
        Side = MapSide(request.Side),
        Size = request.Size,
        Price = request.Price,
        MinuteToExpire = request.MinuteToExpire,
        TimeInForce = request.TimeInForce is null ? null : MapTimeInForce(request.TimeInForce.Value),
        TriggerPrice = request.TriggerPrice,
    };

    private static string MapProductCode(RawProductCode productCode) =>
        string.IsNullOrWhiteSpace(productCode.Value)
            ? throw new ArgumentOutOfRangeException(nameof(productCode), productCode, "Unsupported product_code.")
            : productCode.Value;

    private static string MapChildOrderType(ChildOrderType childOrderType) =>
        childOrderType switch
        {
            ChildOrderType.Market => "MARKET",
            ChildOrderType.Limit => "LIMIT",
            _ => throw new ArgumentOutOfRangeException(nameof(childOrderType), childOrderType, "Unsupported child_order_type."),
        };

    private static string MapSide(Side side) =>
        side switch
        {
            Side.Buy => "BUY",
            Side.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unsupported side."),
        };

    private static string MapTimeInForce(TimeInForce timeInForce) =>
        timeInForce switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
            _ => throw new ArgumentOutOfRangeException(nameof(timeInForce), timeInForce, "Unsupported time_in_force."),
        };
}
