namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawTradeId(string Value)
{
    public override string ToString() => Value;
}
