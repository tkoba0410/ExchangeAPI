using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
namespace ExchangeApi.Contracts.Dtos.Account;

/// <summary>口座残高（通貨のみ型安全）。</summary>
public sealed record Balance(
    ExchangeCode ExchangeCode,
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
            ExchangeCode: exchange,
            Currency: currency,
            Amount: amount,
            Available: available,
            CurrencyCode: code);
    }
}
