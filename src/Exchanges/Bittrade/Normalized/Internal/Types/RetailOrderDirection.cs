using System.Globalization;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

public readonly record struct RetailOrderDirection(int Value)
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
