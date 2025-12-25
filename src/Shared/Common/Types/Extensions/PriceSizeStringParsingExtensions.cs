using System;
using System.Globalization;

namespace ExchangeApi.Common.Types.Extensions;

public static class PriceSizeStringParsingExtensions
{
    public static bool TryParsePriceInvariant(this string text, out Price price)
    {
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
        {
            price = new Price(v);
            return true;
        }

        price = default;
        return false;
    }

    public static bool TryParseSizeInvariant(this string text, out Size size)
    {
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
        {
            size = new Size(v);
            return true;
        }

        size = default;
        return false;
    }

    public static Price ParsePriceInvariant(this string text)
    {
        if (!text.TryParsePriceInvariant(out var p))
            throw new FormatException($"Invalid price: '{text}'");
        return p;
    }

    public static Size ParseSizeInvariant(this string text)
    {
        if (!text.TryParseSizeInvariant(out var s))
            throw new FormatException($"Invalid size: '{text}'");
        return s;
    }
}
