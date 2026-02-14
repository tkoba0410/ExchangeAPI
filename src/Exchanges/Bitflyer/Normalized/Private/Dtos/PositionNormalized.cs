using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record PositionNormalized(
    ProductCode ProductCode,
    Side Side,
    decimal Size,
    decimal Price,
    decimal Pnl,
    DateTimeOffset OpenDate);
