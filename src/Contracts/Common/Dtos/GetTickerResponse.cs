using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetTickerResponse(
    Symbol Symbol,
    Price LastTradedPrice,
    DateTimeOffset Timestamp);
