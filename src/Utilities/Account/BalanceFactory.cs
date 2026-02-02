using System;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Utilities.Account;

public static class BalanceFactory
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
            Currency: code,
            Amount: amount,
            Available: available);
    }
}
