using System;

namespace ExchangeApi.Contracts.Common.DomainCommon.Types.Extensions;

public static class PriceSizeStringParsingExtensions
{
    [Obsolete("Use Price.TryParsePrice(string, out Price) instead.")]
    public static bool TryParsePriceInvariant(this string text, out Price price)
    {
        return Price.TryParsePrice(text, out price);
    }

    [Obsolete("Use Size.TryParseSize(string, out Size) instead.")]
    public static bool TryParseSizeInvariant(this string text, out Size size)
    {
        return Size.TryParseSize(text, out size);
    }

    [Obsolete("Use Price.ParsePriceOrThrow(string) instead.")]
    public static Price ParsePriceInvariant(this string text)
    {
        return Price.ParsePriceOrThrow(text);
    }

    [Obsolete("Use Size.ParseSizeOrThrow(string) instead.")]
    public static Size ParseSizeInvariant(this string text)
    {
        return Size.ParseSizeOrThrow(text);
    }
}
