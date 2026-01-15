using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos.Market;

/// <summary>
/// Candlestick を列指向で保持する内部向けバッファ。
/// </summary>
public sealed class CandlestickColumnar
{
    public ExchangeCode ExchangeCode { get; }
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

    private CandlestickColumnar(
        ExchangeCode exchangeCode,
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
        ExchangeCode = exchangeCode;
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

    public static CandlestickColumnar FromRows(IReadOnlyList<Candlestick> rows)
    {
        if (rows.Count == 0)
        {
            throw new ArgumentException("Candlesticks is empty.", nameof(rows));
        }

        var count = rows.Count;
        var exchangeCode = rows[0].ExchangeCode;
        var symbol = rows[0].Symbol;
        var timescale = rows[0].Timescale;

        var openTimes = new DateTimeOffset[count];
        var closeTimes = new DateTimeOffset[count];
        var open = new decimal[count];
        var high = new decimal[count];
        var low = new decimal[count];
        var close = new decimal[count];
        var volume = new decimal[count];
        var isFinal = new bool[count];
        var quoteVolume = new decimal?[count];
        var numberOfTrades = new long?[count];

        for (var i = 0; i < count; i++)
        {
            var c = rows[i];
            if (c.ExchangeCode != exchangeCode || c.Symbol != symbol || c.Timescale != timescale)
            {
                throw new ArgumentException("Mixed exchange, symbol or timescale in candlesticks.", nameof(rows));
            }

            openTimes[i] = c.OpenTime;
            closeTimes[i] = c.CloseTime;
            open[i] = c.Open;
            high[i] = c.High;
            low[i] = c.Low;
            close[i] = c.Close;
            volume[i] = c.Volume;
            isFinal[i] = c.IsFinal;
            quoteVolume[i] = c.QuoteVolume;
            numberOfTrades[i] = c.NumberOfTrades;
        }

        return new CandlestickColumnar(
            exchangeCode,
            symbol,
            timescale,
            count,
            openTimes,
            closeTimes,
            open,
            high,
            low,
            close,
            volume,
            isFinal,
            quoteVolume,
            numberOfTrades);
    }

    public IReadOnlyList<Candlestick> ToRows()
    {
        var rows = new Candlestick[Count];
        for (var i = 0; i < Count; i++)
        {
            rows[i] = new Candlestick(
                ExchangeCode,
                Symbol,
                Timescale,
                OpenTimes[i],
                CloseTimes[i],
                Open[i],
                High[i],
                Low[i],
                Close[i],
                Volume[i],
                IsFinal[i],
                QuoteVolume[i],
                NumberOfTrades[i]);
        }

        return rows;
    }
}
