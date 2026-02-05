using System;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;

public enum BittradeOrderSide
{
    Buy,
    Sell
}

internal static class BittradeOrderSideParser
{
    public static bool TryParse(string? value, out BittradeOrderSide side)
    {
        if (string.Equals(value, "buy", StringComparison.OrdinalIgnoreCase))
        {
            side = BittradeOrderSide.Buy;
            return true;
        }

        if (string.Equals(value, "sell", StringComparison.OrdinalIgnoreCase))
        {
            side = BittradeOrderSide.Sell;
            return true;
        }

        side = default;
        return false;
    }

}
