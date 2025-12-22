using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class BitflyerAccountMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;

    public static IReadOnlyList<Balance> MapBalances(IReadOnlyList<BalanceResponse> rawBalances)
    {
        if (rawBalances is null) throw new ArgumentNullException(nameof(rawBalances));

        return rawBalances
            .Select(b => Balance.Create(
                exchange: Exchange,
                currency: b.CurrencyCode,
                amount: b.Amount,
                available: b.Available))
            .ToArray();
    }

    public static IReadOnlyList<ExecutionAccount> MapAccountExecutions(
        string productCode,
        IReadOnlyList<ExecutionPrivateResponse> rawExecutions)
    {
        if (rawExecutions is null) throw new ArgumentNullException(nameof(rawExecutions));

        return rawExecutions
            .Select(e => new ExecutionAccount(
                ExchangeCode: ExchangeCode.Bitflyer,
                Symbol: BitflyerCommonMapper.ToSymbol(BitflyerCommonMapper.ToApiProductCode(e.ProductCode)),
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
