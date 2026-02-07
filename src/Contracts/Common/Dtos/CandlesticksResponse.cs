using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record CandlesticksResponse(IReadOnlyList<Candlestick> Items);
