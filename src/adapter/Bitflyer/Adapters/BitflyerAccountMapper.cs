using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

internal static class BitflyerAccountMapper
{
    public static IReadOnlyList<Balance> MapBalances(IReadOnlyList<BitflyerBalanceResponse> rawBalances)
    {
        if (rawBalances is null) throw new ArgumentNullException(nameof(rawBalances));

        return rawBalances
            .Select(b => new Balance(
                b.CurrencyCode,
                b.Amount,
                b.Available))
            .ToArray();
    }
}
