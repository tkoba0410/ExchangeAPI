using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Constants;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Utilities.OrderBook;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;

internal static class ContractMapStage
{
    public static TickerResponse MapTicker(CommonSymbol symbol, TickerNormalized normalized)
    {
        var ticker = new TickerResponse(
            Symbol: symbol,
            LastTradedPrice: new Price(normalized.LastTradedPrice),
            Timestamp: normalized.Timestamp);

        return ticker;
    }

    public static BoardResponse MapOrderBook(OrderBookNormalized normalized)
    {
        var bids = normalized.Bids
            .Select(level => new BoardLevel(new Price(level.Price), new Size(level.Size)))
            .ToList();
        var asks = normalized.Asks
            .Select(level => new BoardLevel(new Price(level.Price), new Size(level.Size)))
            .ToList();

        if (!OrderBookNormalizer.TryNormalize(bids, asks, out var orderBook, out var error))
        {
            throw new ExchangeApiException(error?.Message ?? "OrderBook normalization failed.");
        }

        return orderBook!;
    }

    public static ExecutionsPublicItem MapExecution(CommonSymbol symbol, ExecutionNormalized normalized)
    {
        return new ExecutionsPublicItem(
            Symbol: symbol,
            OrderId: normalized.OrderId,
            Side: MapSide(normalized.Side),
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.Timestamp);
    }

    public static IReadOnlyList<Candlestick> MapCandlesticks(
        CommonSymbol symbol,
        Period period,
        IReadOnlyList<KlineNormalized> klines)
    {
        if (!CandlestickPeriods.TryGetTimescale(period.Value, out var timescale))
        {
            throw new ExchangeApiException($"Unsupported candlestick period: {period.Value}");
        }

        return klines
            .Select(kline =>
            {
                var openTime = ParseUnixTimestamp(kline.OpenTimeUnix);
                var closeTime = openTime + timescale;
                return new Candlestick(
                    Symbol: symbol,
                    Timescale: timescale,
                    OpenTime: openTime,
                    CloseTime: closeTime,
                    Open: kline.Open,
                    High: kline.High,
                    Low: kline.Low,
                    Close: kline.Close,
                    Volume: kline.Volume,
                    IsFinal: true,
                    QuoteVolume: kline.Amount,
                    NumberOfTrades: kline.Count);
            })
            .ToList();
    }

    private static Side MapSide(object side)
    {
        var sideText = side.ToString();
        if (string.Equals(sideText, "Buy", StringComparison.OrdinalIgnoreCase))
        {
            return Side.Buy;
        }

        if (string.Equals(sideText, "Sell", StringComparison.OrdinalIgnoreCase))
        {
            return Side.Sell;
        }

        throw new ExchangeApiException($"Unsupported side: {sideText}.");
    }

    private static DateTimeOffset ParseUnixTimestamp(FreeText openTimeUnix)
    {
        if (!long.TryParse(openTimeUnix.Value, out var unixValue))
        {
            throw new ExchangeApiException($"Invalid candlestick timestamp: {openTimeUnix.Value}");
        }

        return unixValue >= 10_000_000_000
            ? DateTimeOffset.FromUnixTimeMilliseconds(unixValue)
            : DateTimeOffset.FromUnixTimeSeconds(unixValue);
    }
}
