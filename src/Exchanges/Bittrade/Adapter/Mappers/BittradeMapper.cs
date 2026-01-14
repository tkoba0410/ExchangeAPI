using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
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
