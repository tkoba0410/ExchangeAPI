using System.Globalization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawCursorId(long Value)
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
