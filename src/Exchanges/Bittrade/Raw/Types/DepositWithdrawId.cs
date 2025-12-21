namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct DepositWithdrawId(string Value)
{
    public override string ToString() => Value;
}
