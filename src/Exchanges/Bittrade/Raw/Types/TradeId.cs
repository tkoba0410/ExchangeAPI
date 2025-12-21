namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct TradeId(string Value)
{
    public override string ToString() => Value;
}
