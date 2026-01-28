using System;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record BitflyerPositionNormalized(
    string ProductCode,
    Side Side,
    decimal Size,
    decimal Price,
    decimal Pnl,
    DateTimeOffset OpenDate);
