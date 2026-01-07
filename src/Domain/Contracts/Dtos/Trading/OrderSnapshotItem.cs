using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Dtos;

public sealed record OrderSnapshotItem(
    DateTimeOffset CreatedAt,
    string OrderId,
    Symbol Market,
    Side Side,
    OrderSnapshotType OrderType,
    Price? Price,
    Size Size,
    OrderSnapshotStatus Status);
