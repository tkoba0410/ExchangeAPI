using System;

namespace ExchangeApi.Abstractions.Dtos;

/// <summary>
/// 建玉情報。
/// </summary>
public sealed record Position(
    string ProductCode,
    OrderSide Side,
    decimal Size,
    decimal Price,
    DateTimeOffset? OpenDate = null,
    decimal? Pnl = null);
