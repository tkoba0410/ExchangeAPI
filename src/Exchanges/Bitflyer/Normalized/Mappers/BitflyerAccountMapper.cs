using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RawPrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerAccountMapper
{
    public static IReadOnlyList<BitflyerBalanceEntryNormalized> MapBalances(
        IReadOnlyList<RawPrivateModels.BalanceResponse> rawBalances)
    {
        if (rawBalances is null) throw new ArgumentNullException(nameof(rawBalances));

        return rawBalances
            .Select(b => new BitflyerBalanceEntryNormalized(
                Currency: b.CurrencyCode,
                Amount: b.Amount,
                Available: b.Available))
            .ToArray();
    }

    public static IReadOnlyList<BitflyerExecutionAccountNormalized> MapAccountExecutions(
        Symbol symbol,
        IReadOnlyList<RawPrivateModels.ExecutionPrivateResponse> rawExecutions)
    {
        if (rawExecutions is null) throw new ArgumentNullException(nameof(rawExecutions));

        return rawExecutions
            .Select(e => new BitflyerExecutionAccountNormalized(
                Symbol: symbol,
                OrderId: e.Id.ToString(CultureInfo.InvariantCulture),
                Side: BitflyerCommonMapper.MapSide(e.Side),
                Price: new Price(e.Price),
                Size: new Size(e.Size),
                ExecutedAt: e.ExecDate,
                Commission: null,
                Pnl: null,
                Liquidity: null))
            .ToArray();
    }
}
