using System;
using System.Collections.Generic;
using System.Linq;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Contracts.Dtos;

namespace Exchange.Bitflyer.Abstract;

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

    public static IReadOnlyList<AccountExecution> MapAccountExecutions(
        string productCode,
        IReadOnlyList<BitflyerExecutionPrivateResponse> rawExecutions)
    {
        if (rawExecutions is null) throw new ArgumentNullException(nameof(rawExecutions));

        return rawExecutions
            .Select(e => new AccountExecution(
                ProductCode: productCode,
                Id: e.Id,
                Side: BitflyerCommonMapper.MapSide(e.Side),
                Price: e.Price,
                Size: e.Size,
                ExecutedAt: e.ExecDate,
                ChildOrderAcceptanceId: e.ChildOrderAcceptanceId))
            .ToArray();
    }
}
