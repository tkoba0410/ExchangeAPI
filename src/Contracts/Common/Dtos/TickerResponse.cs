using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record TickerResponse(
    Symbol Symbol,
    Price LastTradedPrice,
    DateTimeOffset Timestamp);
