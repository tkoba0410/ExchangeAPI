namespace ExchangeApi.Primitives.DomainCommon.Enums;

/// <summary>取引所識別コード。</summary>
public enum ExchangeCode
{
    None = 0,
    Unknown = -2,
    Sandbox = -1,

    Binance = 1,
    Bitbank,
    Bitflyer,
    Bittrade,
    Btcbox,
    Coincheck,
    Gmocoin,
    Okcoin,
}
