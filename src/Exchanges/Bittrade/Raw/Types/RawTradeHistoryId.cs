namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawTradeHistoryId(string Value)
{
    public override string ToString() => Value;
}
