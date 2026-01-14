namespace ExchangeApi.Exchanges.Bittrade.Normalized.Types;

internal enum BittradeOrderState
{
    Submitted,
    PartialFilled,
    PartialCanceled,
    Filled,
    Canceled
}
