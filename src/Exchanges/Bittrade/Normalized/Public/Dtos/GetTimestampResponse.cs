using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record GetTimestampResponse(
    DateTimeOffset Item);
