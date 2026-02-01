using System;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>
/// ローソク足（OHLCV）。
/// </summary>
public sealed record Candlestick(
    Symbol Symbol,
    TimeSpan Timescale,
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
