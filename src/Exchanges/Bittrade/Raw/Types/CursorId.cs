using System.Globalization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct CursorId(long Value)
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
