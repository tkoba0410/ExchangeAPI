using System;
using System.Collections.Generic;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Utilities.Candlestick;

public sealed class CandlestickColumnar
{
    public Symbol Symbol { get; }
    public TimeSpan Timescale { get; }
    public int Count { get; }
    public DateTimeOffset[] OpenTimes { get; }
    public DateTimeOffset[] CloseTimes { get; }
    public decimal[] Open { get; }
    public decimal[] High { get; }
    public decimal[] Low { get; }
    public decimal[] Close { get; }
    public decimal[] Volume { get; }
    public bool[] IsFinal { get; }
    public decimal?[] QuoteVolume { get; }
    public long?[] NumberOfTrades { get; }

    internal CandlestickColumnar(
        Symbol symbol,
        TimeSpan timescale,
        int count,
        DateTimeOffset[] openTimes,
        DateTimeOffset[] closeTimes,
        decimal[] open,
        decimal[] high,
        decimal[] low,
        decimal[] close,
        decimal[] volume,
        bool[] isFinal,
        decimal?[] quoteVolume,
        long?[] numberOfTrades)
    {
        Symbol = symbol;
        Timescale = timescale;
        Count = count;
        OpenTimes = openTimes;
        CloseTimes = closeTimes;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        IsFinal = isFinal;
        QuoteVolume = quoteVolume;
        NumberOfTrades = numberOfTrades;
    }
}
