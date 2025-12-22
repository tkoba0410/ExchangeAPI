using System.Globalization;

namespace ExchangeApi.Common.Types;

public readonly record struct Size(decimal Value)
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
