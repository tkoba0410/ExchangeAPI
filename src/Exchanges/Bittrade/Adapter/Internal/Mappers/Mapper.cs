using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Utilities.Account;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;

internal static class Mapper
{
    public static IReadOnlyList<BalanceEntry> MapBalances(IReadOnlyList<BalanceEntryNormalized> entries)
    {
        var result = new List<BalanceEntry>();
        foreach (var group in entries.GroupBy(e => e.Currency.Value, StringComparer.OrdinalIgnoreCase))
        {
            var total = group.Sum(e => e.Balance);
            var available = group
                .Where(x => string.Equals(x.Type.Value, "trade", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Balance);
            result.Add(BalanceFactory.Create(
                currency: group.Key.ToUpperInvariant(),
                amount: total,
                available: available));
        }
        return result;
    }
}
