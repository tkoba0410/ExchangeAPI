using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos.Trading;

public sealed record ExecutionItem(
    DateTimeOffset Timestamp,
    string ExecutionId,
    Symbol Market,
    Side Side,
    Price Price,
    Size Size);
