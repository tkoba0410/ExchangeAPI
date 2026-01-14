using System.Globalization;

namespace ExchangeApi.Contracts.Common.DomainCommon.Types;

public readonly record struct Size(decimal Value)
{
    public static bool TryParseSize(string s, out Size value)
    {
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            value = new Size(parsed);
            return true;
        }

        value = default;
        return false;
    }

    public static Size ParseSizeOrThrow(string s)
    {
        if (!TryParseSize(s, out var value))
        {
            throw new FormatException($"Invalid size: '{s}'.");
        }

        return value;
    }

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
