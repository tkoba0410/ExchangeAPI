namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Types;

internal enum BittradeOrderState
{
    Submitted,
    PartialFilled,
    PartialCanceled,
    Filled,
    Canceled
}
