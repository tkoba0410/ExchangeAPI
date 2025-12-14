using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>建玉情報。</summary>
public sealed record Position(
    ExchangeCode Exchange,
    Symbol Symbol,
    OrderSide Side,
    decimal Size,
    decimal Price,
    DateTimeOffset? OpenDate = null,
    decimal? Pnl = null);
