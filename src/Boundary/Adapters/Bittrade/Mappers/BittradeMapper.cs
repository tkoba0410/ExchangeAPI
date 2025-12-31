using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;

internal static class BittradeMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static IReadOnlyList<Balance> MapBalances(IReadOnlyList<BittradeBalanceEntryNormalized> entries)
    {
        var result = new List<Balance>();
        foreach (var group in entries.GroupBy(e => e.Currency, StringComparer.OrdinalIgnoreCase))
        {
            var total = group.Sum(e => e.Balance);
            var available = group
                .Where(x => string.Equals(x.Type, "trade", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Balance);
            result.Add(Balance.Create(
                exchange: Exchange,
                currency: group.Key.ToUpperInvariant(),
                amount: total,
                available: available));
        }
        return result;
    }
}
