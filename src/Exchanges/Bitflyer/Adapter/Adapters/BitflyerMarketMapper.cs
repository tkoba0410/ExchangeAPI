using System;
using System.Linq;
using ExchangeApi.Common.Enums;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
using RawBoard = ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet.Board;
using RawExecutionPublicResponse = ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet.ExecutionPublicResponse;
using RawTicker = ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet.Ticker;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class MarketMapper
{
    public static CommonTicker MapTicker(Symbol symbol, RawTicker raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new CommonTicker(
            Exchange: ExchangeCode.Bitflyer,
            Symbol: symbol,
            LastTradedPrice: new Price(raw.LastTradedPrice),
            Timestamp: raw.Timestamp);
    }

    public static OrderBook MapOrderBook(RawBoard rawBoard)
    {
        if (rawBoard is null) throw new ArgumentNullException(nameof(rawBoard));

        var bids = rawBoard.Bids?
            .Select(b => new OrderBookLevel(new Price(b.Price), new Size(b.Size)))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        var asks = rawBoard.Asks?
            .Select(a => new OrderBookLevel(new Price(a.Price), new Size(a.Size)))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        return new OrderBook(ExchangeCode.Bitflyer, bids, asks);
    }

    public static ExecutionMarket MapExecution(Symbol symbol, RawExecutionPublicResponse raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new ExecutionMarket(
            ExchangeCode: ExchangeCode.Bitflyer,
            Symbol: symbol,
            OrderId: raw.Id.ToString(),
            Side: BitflyerCommonMapper.MapSide(raw.Side),
            Price: new Price(raw.Price),
            Size: new Size(raw.Size),
            ExecutedAt: raw.ExecDate);
    }
}
