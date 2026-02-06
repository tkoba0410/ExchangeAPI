using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record BalanceResponse(IReadOnlyList<BalanceEntry> Balances);

public sealed record BalanceEntry(
    CurrencyCode Currency,
    decimal Amount,
    decimal Available);
