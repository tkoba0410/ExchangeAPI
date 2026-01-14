using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
namespace ExchangeApi.Contracts.Dtos.Market;

/// <summary>
/// ローソク足（OHLCV）。
/// </summary>
/// <remarks>
/// Stage6 の bitFlyer 実装では REST 取得を提供しないため、利用側で NotSupported を受け取る前提で使用する。
/// </remarks>
public sealed record Candlestick(
    ExchangeCode ExchangeCode,
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
