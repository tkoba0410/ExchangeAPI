using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>口座残高（通貨のみ型安全）。</summary>
public sealed record Balance(
    string Currency,
    decimal Amount,
    decimal Available,
    CurrencyCode CurrencyCode = CurrencyCode.Unknown)
{
    public static Balance Create(
        string currency,
        decimal amount,
        decimal available,
        Func<string, CurrencyCode>? codeResolver = null)
    {
        if (currency is null) throw new ArgumentNullException(nameof(currency));

        var code = codeResolver?.Invoke(currency) ?? CurrencyCodeConverter.FromString(currency);

        return new Balance(
            Currency: currency,
            Amount: amount,
            Available: available,
            CurrencyCode: code);
    }
}
