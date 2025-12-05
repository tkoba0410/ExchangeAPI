using System;

namespace ExchangeApi.Core.Dtos;

/// <summary>
/// ローソク足（OHLCV）。
/// </summary>
public sealed record Candlestick(
    string Symbol,
    string Timescale,
    DateTimeOffset OpenTime,
    DateTimeOffset CloseTime,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    bool IsFinal,
    decimal? QuoteVolume = null,
    long? NumberOfTrades = null);
