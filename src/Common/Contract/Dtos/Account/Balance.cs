using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>口座残高（通貨のみ型安全）。</summary>
public sealed record Balance(
    ExchangeCode Exchange,
    string Currency,
    decimal Amount,
    decimal Available,
    CurrencyCode CurrencyCode = CurrencyCode.Unknown)
{
    public static Balance Create(
        ExchangeCode exchange,
        string currency,
        decimal amount,
        decimal available,
        Func<string, CurrencyCode>? codeResolver = null)
    {
        if (currency is null) throw new ArgumentNullException(nameof(currency));

        var code = codeResolver?.Invoke(currency) ?? CurrencyCodeConverter.FromString(currency);

        return new Balance(
            Exchange: exchange,
            Currency: currency,
            Amount: amount,
            Available: available,
            CurrencyCode: code);
    }
}
