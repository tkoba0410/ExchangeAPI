using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record GetSymbolsResponse(
    IReadOnlyList<SymbolNormalized> Items);
