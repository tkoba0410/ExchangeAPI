using System;
using System.Linq;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Types;
using CommonTicker = ExchangeApi.Common.Dtos.Ticker;
using RawBoard = ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet.Board;
using RawExecutionPublicResponse = ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet.ExecutionPublicResponse;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.ProductCode;
using RawTicker = ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet.Ticker;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class MarketMapper
{
    public static CommonTicker MapTicker(string symbol, RawTicker raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new CommonTicker(
            Exchange: ExchangeCode.Bitflyer,
            Symbol: BitflyerCommonMapper.ToSymbol(symbol),
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

    public static ExecutionMarket MapExecution(RawProductCode productCode, RawExecutionPublicResponse raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new ExecutionMarket(
            ExchangeCode: ExchangeCode.Bitflyer,
            Symbol: BitflyerCommonMapper.ToSymbol(BitflyerCommonMapper.ToApiProductCode(productCode)),
            OrderId: raw.Id.ToString(),
            Side: BitflyerCommonMapper.MapSide(raw.Side),
            Price: new Price(raw.Price),
            Size: new Size(raw.Size),
            ExecutedAt: raw.ExecDate);
    }
}
