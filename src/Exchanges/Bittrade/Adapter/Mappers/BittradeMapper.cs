using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;

internal static class BittradeMapper
{
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
                currency: group.Key.ToUpperInvariant(),
                amount: total,
                available: available));
        }
        return result;
    }
}
