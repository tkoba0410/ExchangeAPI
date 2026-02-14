using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record GetRetailAccountBalanceResponse(
    IReadOnlyList<RetailBalanceEntryNormalized> Items);
