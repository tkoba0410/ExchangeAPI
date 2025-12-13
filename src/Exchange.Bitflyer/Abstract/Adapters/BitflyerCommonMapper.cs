using System;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace Exchange.Bitflyer.Abstract.Adapters;

internal static class BitflyerCommonMapper
{
    public static OrderSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static string MapSideToExchange(OrderSide side) =>
        BitflyerSideMapper.ToApi(side);

    public static OrderStatusType MapOrderStatusType(string childOrderState) =>
        childOrderState.ToUpperInvariant() switch
        {
            BitflyerConstants.States.OrderActive => OrderStatusType.Active,
            BitflyerConstants.States.OrderCompleted => OrderStatusType.Completed,
            BitflyerConstants.States.OrderCanceled => OrderStatusType.Canceled,
            BitflyerConstants.States.OrderExpired => OrderStatusType.Expired,
            _ => OrderStatusType.Unknown,
        };

    public static string MapSymbolToProductCode(string symbol)
    {
        if (string.Equals(symbol, "BTC/JPY", StringComparison.Ordinal))
        {
            return "BTC_JPY";
        }

        throw new SymbolNotSupportedException(symbol);
    }
}
