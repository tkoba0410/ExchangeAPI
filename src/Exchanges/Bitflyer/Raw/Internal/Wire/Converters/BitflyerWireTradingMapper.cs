using System;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Converters;

internal static class BitflyerWireTradingMapper
{
    public static RawSendChildOrderRequest MapSendChildOrderRequest(CreateChildOrderRequest request) => new()
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

    public static RawCancelChildOrderRequest MapCancelChildOrderRequest(CancelChildOrderRequest request) => new()
    {
        ProductCode = MapProductCode(request.ProductCode),
        ChildOrderId = request.ChildOrderId,
        ChildOrderAcceptanceId = request.ChildOrderAcceptanceId,
    };

    public static CreateChildOrderResponse MapSendChildOrderResponse(RawSendChildOrderResponse response) => new()
    {
        ChildOrderAcceptanceId = response.ChildOrderAcceptanceId,
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
