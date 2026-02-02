using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record OrderSnapshotItem(
    DateTimeOffset CreatedAt,
    OrderId OrderId,
    Symbol Market,
    Side Side,
    OrderSnapshotType OrderType,
    Price? Price,
    Size Size,
    OrderSnapshotStatus Status);
