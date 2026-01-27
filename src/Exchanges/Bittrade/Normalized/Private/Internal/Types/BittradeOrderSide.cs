using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Types;

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

    public static BittradeOrderSide ParseOrThrow(string? value, string context)
    {
        if (TryParse(value, out var side))
        {
            return side;
        }

        throw new ArgumentException($"Unsupported {context} side: {value ?? "<null>"}.", nameof(value));
    }
}
