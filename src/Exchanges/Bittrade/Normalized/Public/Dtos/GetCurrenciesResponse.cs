using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record GetCurrenciesResponse(
    IReadOnlyList<CurrencyCode> Items);
