using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RawPrivateDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerAccountMapper
{
    public static IReadOnlyList<BitflyerBalanceEntryNormalized> MapBalances(
        IReadOnlyList<RawPrivateDtos.BalanceResponse> rawBalances)
    {
        if (rawBalances is null) throw new ArgumentNullException(nameof(rawBalances));

        return rawBalances
            .Select(b => new BitflyerBalanceEntryNormalized(
                CurrencyCode: CurrencyCodeConverter.FromString(b.CurrencyCode),
                Amount: b.Amount,
                Available: b.Available))
            .ToArray();
    }

    public static IReadOnlyList<BitflyerExecutionAccountNormalized> MapAccountExecutions(
        Symbol symbol,
        IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse> rawExecutions)
    {
        if (rawExecutions is null) throw new ArgumentNullException(nameof(rawExecutions));

        return rawExecutions
            .Select(e => new BitflyerExecutionAccountNormalized(
                Symbol: symbol,
                OrderId: new OrderId(e.Id.ToString(CultureInfo.InvariantCulture)),
                Side: BitflyerCommonMapper.MapSide(e.Side),
                Price: new Price(e.Price),
                Size: new Size(e.Size),
                ExecutedAt: e.ExecDate,
                Commission: null,
                Pnl: null,
                Liquidity: null))
            .ToArray();
    }

    public static bool TryMapAccountExecutions(
        Symbol symbol,
        IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse> rawExecutions,
        out IReadOnlyList<BitflyerExecutionAccountNormalized>? normalized,
        out CallError? error)
    {
        if (rawExecutions is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer executions response is null.");
            return false;
        }

        var mapped = new List<BitflyerExecutionAccountNormalized>(rawExecutions.Count);
        foreach (var e in rawExecutions)
        {
            if (!BitflyerCommonMapper.TryMapSide(e.Side, out var side, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BitflyerExecutionAccountNormalized(
                Symbol: symbol,
                OrderId: new OrderId(e.Id.ToString(CultureInfo.InvariantCulture)),
                Side: side,
                Price: new Price(e.Price),
                Size: new Size(e.Size),
                ExecutedAt: e.ExecDate,
                Commission: null,
                Pnl: null,
                Liquidity: null));
        }

        normalized = mapped.ToArray();
        error = null;
        return true;
    }
}
