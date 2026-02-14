using System.Globalization;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

public readonly record struct RetailOrderAmount(decimal Value)
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
