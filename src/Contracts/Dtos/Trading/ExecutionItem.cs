using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Dtos;

public sealed record ExecutionItem(
    DateTimeOffset Timestamp,
    string ExecutionId,
    Symbol Market,
    Side Side,
    Price Price,
    Size Size);
