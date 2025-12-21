namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct MatchId(string Value)
{
    public override string ToString() => Value;
}
