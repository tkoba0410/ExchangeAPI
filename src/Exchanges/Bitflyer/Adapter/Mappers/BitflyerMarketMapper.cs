using System;
using System.Linq;
using ExchangeApi.Common.Enums;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
using RawBoard = ExchangeApi.Exchanges.Bitflyer.Wire.Public.Board;
using RawExecutionPublicResponse = ExchangeApi.Exchanges.Bitflyer.Wire.Public.ExecutionPublicResponse;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;

internal static class MarketMapper
{
    public static CommonTicker MapTicker(Symbol symbol, BitflyerTickerNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        return new CommonTicker(
            Exchange: ExchangeCode.Bitflyer,
            Symbol: symbol,
            LastTradedPrice: new Price(normalized.LastTradedPrice),
            Timestamp: normalized.Timestamp);
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
