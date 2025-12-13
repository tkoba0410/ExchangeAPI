using System;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

internal static class BitflyerCommonMapper
{
    public static OrderSide MapSide(string side) =>
        string.Equals(side, "BUY", StringComparison.OrdinalIgnoreCase)
            ? OrderSide.Buy
            : OrderSide.Sell;

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
