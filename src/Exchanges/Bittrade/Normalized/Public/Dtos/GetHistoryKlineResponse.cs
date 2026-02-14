using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record GetHistoryKlineResponse(
    IReadOnlyList<KlineNormalized> Items);
