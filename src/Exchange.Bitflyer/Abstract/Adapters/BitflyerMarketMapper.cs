using System;
using System.Linq;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

internal static class BitflyerMarketMapper
{
    public static Ticker MapTicker(string symbol, BitflyerTickerRaw raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new Ticker(
            Symbol: symbol,
            BestBid: raw.BestBid,
            BestAsk: raw.BestAsk,
            LastTradedPrice: raw.LastTradedPrice,
            Timestamp: raw.Timestamp);
    }

    public static OrderBook MapOrderBook(BitflyerBoardRaw rawBoard)
    {
        if (rawBoard is null) throw new ArgumentNullException(nameof(rawBoard));

        var bids = rawBoard.Bids?
            .Select(b => new OrderBookLevel(b.Price, b.Size))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        var asks = rawBoard.Asks?
            .Select(a => new OrderBookLevel(a.Price, a.Size))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        return new OrderBook(
            Bids: bids,
            Asks: asks,
            MidPrice: rawBoard.MidPrice);
    }

    public static MarketExecution MapExecution(string productCode, BitflyerExecutionResponse raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new MarketExecution(
            ProductCode: productCode,
            Id: raw.Id,
            Side: BitflyerCommonMapper.MapSide(raw.Side),
            Price: raw.Price,
            Size: raw.Size,
            ExecutedAt: raw.ExecDate);
    }
}
