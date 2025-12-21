namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct MatchResultId(string Value)
{
    public override string ToString() => Value;
}
