using System.Collections.Generic;
using System.Linq;
using System;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Utilities.Account;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Map;

internal static partial class ContractMapper
{
    public static IReadOnlyList<BalanceEntry> MapBalances(IReadOnlyList<BalanceEntryNormalized> entries)
    {
        var result = new List<BalanceEntry>();
        foreach (var group in entries.GroupBy(e => e.Currency))
        {
            var total = group.Sum(e => e.Balance);
            var available = group
                .Where(x => x.Type.IsKnown && string.Equals(x.Type.ToString(), "Trade", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Balance);
            result.Add(BalanceFactory.Create(
                currency: group.Key.ToString().ToUpperInvariant(),
                amount: total,
                available: available));
        }
        return result;
    }
}
