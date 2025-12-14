using System;
using System.Collections.Generic;

namespace Common.Contract.Enums;

/// <summary>主要通貨コード。</summary>
public enum CurrencyCode
{
    Unknown = 0,
    Jpy,
    Usd,
    Eur,
    Usdt,
    Usdc,
    Btc,
    Eth,
    Bch,
    Ltc,
    Xrp,
    Bnb,
    Ada,
    Dot,
    Sol,
    Matic,
    Avax,
    Dai,
    Doge,
}

public static class CurrencyCodeConverter
{
    private static readonly Dictionary<string, CurrencyCode> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["JPY"] = CurrencyCode.Jpy,
        ["USD"] = CurrencyCode.Usd,
        ["EUR"] = CurrencyCode.Eur,
        ["USDT"] = CurrencyCode.Usdt,
        ["USDC"] = CurrencyCode.Usdc,
        ["BTC"] = CurrencyCode.Btc,
        ["XBT"] = CurrencyCode.Btc,
        ["ETH"] = CurrencyCode.Eth,
        ["BCH"] = CurrencyCode.Bch,
        ["LTC"] = CurrencyCode.Ltc,
        ["XRP"] = CurrencyCode.Xrp,
        ["BNB"] = CurrencyCode.Bnb,
        ["ADA"] = CurrencyCode.Ada,
        ["DOT"] = CurrencyCode.Dot,
        ["SOL"] = CurrencyCode.Sol,
        ["MATIC"] = CurrencyCode.Matic,
        ["AVAX"] = CurrencyCode.Avax,
        ["DAI"] = CurrencyCode.Dai,
        ["DOGE"] = CurrencyCode.Doge,
    };

    public static CurrencyCode FromString(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency)) return CurrencyCode.Unknown;
        return Map.TryGetValue(currency.Trim(), out var code)
            ? code
            : CurrencyCode.Unknown;
    }

    public static string ToCurrencyString(CurrencyCode code) =>
        code switch
        {
            CurrencyCode.Unknown => string.Empty,
            _ => code.ToString().ToUpperInvariant()
        };
}
