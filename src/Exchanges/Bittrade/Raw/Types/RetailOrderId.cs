namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RetailOrderId(string Value)
{
    public override string ToString() => Value;
}
