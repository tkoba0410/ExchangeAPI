using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Dtos.Trading;

public sealed record OrderSnapshotItem(
    DateTimeOffset CreatedAt,
    string OrderId,
    Symbol Market,
    Side Side,
    OrderSnapshotType OrderType,
    Price? Price,
    Size Size,
    OrderSnapshotStatus Status);
