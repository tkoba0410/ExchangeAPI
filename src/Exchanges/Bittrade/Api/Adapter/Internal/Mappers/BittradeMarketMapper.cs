using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Constants;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Utilities.OrderBook;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;

internal static class BittradeMarketMapper
{
    public static TickerResponse MapTicker(CommonSymbol symbol, BittradeTickerNormalized normalized)
    {
        var ticker = new TickerResponse(
            Symbol: symbol,
            LastTradedPrice: new Price(normalized.LastTradedPrice),
            Timestamp: normalized.Timestamp);

        return ticker;
    }

    public static BoardResponse MapOrderBook(BittradeOrderBookNormalized normalized)
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

    public static ExecutionsPublicItem MapExecution(CommonSymbol symbol, BittradeExecutionNormalized normalized)
    {
        return new ExecutionsPublicItem(
            Symbol: symbol,
            OrderId: normalized.Id,
            Side: MapSide(normalized.Side),
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.Timestamp);
    }

    public static IReadOnlyList<Candlestick> MapCandlesticks(
        CommonSymbol symbol,
        PeriodDto period,
        IReadOnlyList<BittradeKlineNormalized> klines)
    {
        if (!BittradeCandlestickPeriods.TryGetTimescale(period.Code, out var timescale))
        {
            throw new ExchangeApiException($"Unsupported candlestick period: {period.Code}");
        }

        return klines
            .Select(kline =>
            {
                var openTime = ParseUnixTimestamp(kline.Id);
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

    private static Side MapSide(BittradeOrderSide side) =>
        side switch
        {
            BittradeOrderSide.Buy => Side.Buy,
            BittradeOrderSide.Sell => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported side: {side}.")
        };

    private static DateTimeOffset ParseUnixTimestamp(FreeText id)
    {
        if (!long.TryParse(id.Value, out var unixValue))
        {
            throw new ExchangeApiException($"Invalid candlestick timestamp: {id.Value}");
        }

        return unixValue >= 10_000_000_000
            ? DateTimeOffset.FromUnixTimeMilliseconds(unixValue)
            : DateTimeOffset.FromUnixTimeSeconds(unixValue);
    }
}
