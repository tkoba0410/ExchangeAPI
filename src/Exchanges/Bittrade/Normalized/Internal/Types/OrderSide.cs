using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

public enum OrderSide
{
    Buy,
    Sell
}

internal static class OrderSideParser
{
    public static bool TryParse(string? value, out OrderSide side)
    {
        if (string.Equals(value, "buy", StringComparison.OrdinalIgnoreCase))
        {
            side = OrderSide.Buy;
            return true;
        }

        if (string.Equals(value, "sell", StringComparison.OrdinalIgnoreCase))
        {
            side = OrderSide.Sell;
            return true;
        }

        side = default;
        return false;
    }

}
