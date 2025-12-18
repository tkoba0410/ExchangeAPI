using System;
using ExchangeApi.Common.Enums;
namespace ExchangeApi.Common.Dtos;

/// <summary>建玉情報。</summary>
public sealed record Position(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    Side Side,
    decimal Size,
    decimal Price,
    DateTimeOffset? OpenDate = null,
    decimal? Pnl = null);
