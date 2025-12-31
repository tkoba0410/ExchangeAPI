using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Contracts.Dtos;

/// <summary>建玉情報。</summary>
public sealed record Position(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    Side Side,
    Size Size,
    Price Price,
    DateTimeOffset? OpenDate = null,
    decimal? Pnl = null);
