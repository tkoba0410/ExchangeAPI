using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record GetMatchResultsResponse(
    IReadOnlyList<ExecutionNormalized> Items);
