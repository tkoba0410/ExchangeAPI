namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct TradeHistoryId(string Value)
{
    public override string ToString() => Value;
}
