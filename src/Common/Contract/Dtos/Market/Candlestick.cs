using System;

namespace Common.Contract.Dtos;

/// <summary>
/// ローソク足（OHLCV）。
/// </summary>
/// <remarks>
/// Stage6 の bitFlyer 実装では REST 取得を提供しないため、利用側で NotSupported を受け取る前提で使用する。
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
