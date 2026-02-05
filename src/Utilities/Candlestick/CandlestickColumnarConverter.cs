using System;
using System.Collections.Generic;
using ExchangeApi.Contracts.Common.Dtos;

namespace ExchangeApi.Utilities.Candlestick;

public static class CandlestickColumnarConverter
{
    public static CandlestickColumnar FromRows(IReadOnlyList<Contracts.Common.Dtos.Candlestick> rows)
    {
        if (rows.Count == 0)
        {
            throw new ArgumentException("Candlesticks is empty.", nameof(rows));
        }

        var count = rows.Count;
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
            if (c.Symbol != symbol || c.Timescale != timescale)
            {
                throw new ArgumentException("Mixed symbol or timescale in candlesticks.", nameof(rows));
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

    public static IReadOnlyList<Contracts.Common.Dtos.Candlestick> ToRows(CandlestickColumnar columnar)
    {
        var rows = new Contracts.Common.Dtos.Candlestick[columnar.Count];
        for (var i = 0; i < columnar.Count; i++)
        {
            rows[i] = new Contracts.Common.Dtos.Candlestick(
                columnar.Symbol,
                columnar.Timescale,
                columnar.OpenTimes[i],
                columnar.CloseTimes[i],
                columnar.Open[i],
                columnar.High[i],
                columnar.Low[i],
                columnar.Close[i],
                columnar.Volume[i],
                columnar.IsFinal[i],
                columnar.QuoteVolume[i],
                columnar.NumberOfTrades[i]);
        }

        return rows;
    }
}
