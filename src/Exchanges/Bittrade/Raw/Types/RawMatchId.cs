namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawMatchId(string Value)
{
    public override string ToString() => Value;
}
