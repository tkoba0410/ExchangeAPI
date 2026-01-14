using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Dtos.Trading;

public sealed record ExecutionItem(
    DateTimeOffset Timestamp,
    string ExecutionId,
    Symbol Market,
    Side Side,
    Price Price,
    Size Size);
