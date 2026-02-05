using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record ExecutionItem(
    DateTimeOffset Timestamp,
    ExecutionId ExecutionId,
    Symbol Market,
    Side Side,
    Price Price,
    Size Size);
