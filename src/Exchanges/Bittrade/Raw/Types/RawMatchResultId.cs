namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawMatchResultId(string Value)
{
    public override string ToString() => Value;
}
