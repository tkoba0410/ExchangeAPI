using System.Globalization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

public readonly record struct Price(decimal Value)
{
    public static bool TryParsePrice(string s, out Price value)
    {
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            value = new Price(parsed);
            return true;
        }

        value = default;
        return false;
    }

    public static Price ParsePriceOrThrow(string s)
    {
        if (!TryParsePrice(s, out var value))
        {
            throw new FormatException($"Invalid price: '{s}'.");
        }

        return value;
    }

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
