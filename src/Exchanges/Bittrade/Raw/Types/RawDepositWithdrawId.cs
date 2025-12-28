namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawDepositWithdrawId(string Value)
{
    public override string ToString() => Value;
}
