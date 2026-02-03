using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerPositionNormalized(
    ProductCode ProductCode,
    Side Side,
    decimal Size,
    decimal Price,
    decimal Pnl,
    DateTimeOffset OpenDate);
