using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record GetTickersResponse(
    IReadOnlyList<TickerEntryNormalized> Items);
