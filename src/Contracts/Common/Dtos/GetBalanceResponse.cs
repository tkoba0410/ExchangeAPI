using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetBalanceResponse(IReadOnlyList<GetBalanceEntry> Balances);

public sealed record GetBalanceEntry(
    CurrencyCode Currency,
    decimal Amount,
    decimal Available);
