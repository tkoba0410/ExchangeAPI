namespace ExchangeApi.Common.Types.Extensions;

public static class PriceSizeDecimalExtensions
{
    public static Price AsPrice(this decimal value) => new(value);
    public static Size AsSize(this decimal value) => new(value);
}
