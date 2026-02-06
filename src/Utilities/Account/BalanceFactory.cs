using System;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Utilities.Account;

public static class BalanceFactory
{
    public static GetBalanceEntry Create(
        CurrencyCode currency,
        decimal amount,
        decimal available)
    {
        return new GetBalanceEntry(
            Currency: currency,
            Amount: amount,
            Available: available);
    }

    public static GetBalanceEntry Create(
        string currency,
        decimal amount,
        decimal available,
        Func<string, CurrencyCode>? codeResolver = null)
    {
        if (currency is null) throw new ArgumentNullException(nameof(currency));

        var code = codeResolver?.Invoke(currency) ?? CurrencyCodeConverter.FromString(currency);

        return new GetBalanceEntry(
            Currency: code,
            Amount: amount,
            Available: available);
    }
}
