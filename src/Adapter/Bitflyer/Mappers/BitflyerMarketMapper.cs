using System;
using System.Linq;
using ExchangeApi.Common.Enums;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
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

    public static OrderBook MapOrderBook(BitflyerOrderBookNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        var bids = normalized.Bids
            .Select(b => new OrderBookLevel(new Price(b.Price), new Size(b.Size)))
            .ToArray();

        var asks = normalized.Asks
            .Select(a => new OrderBookLevel(new Price(a.Price), new Size(a.Size)))
            .ToArray();

        return new OrderBook(ExchangeCode.Bitflyer, bids, asks);
    }

    public static ExecutionMarket MapExecution(Symbol symbol, BitflyerExecutionNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        return new ExecutionMarket(
            ExchangeCode: ExchangeCode.Bitflyer,
            Symbol: symbol,
            OrderId: normalized.Id.ToString(),
            Side: BitflyerCommonMapper.MapSide(normalized.Side),
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.ExecutedAt);
    }

}
