using System;
using System.Linq;
using Exchange.Bitflyer.Raw;
using Common.Contract.Enums;
using Common.Contract.Dtos;
using RawProductCode = Exchange.Bitflyer.Raw.ProductCode;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerMarketMapper
{
    public static Ticker MapTicker(string symbol, BitflyerTicker raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new Ticker(
            Exchange: ExchangeCode.Bitflyer,
            Symbol: BitflyerCommonMapper.ToSymbol(symbol),
            LastTradedPrice: raw.LastTradedPrice,
            Timestamp: raw.Timestamp);
    }

    public static OrderBook MapOrderBook(BitflyerBoard rawBoard)
    {
        if (rawBoard is null) throw new ArgumentNullException(nameof(rawBoard));

        var bids = rawBoard.Bids?
            .Select(b => new OrderBookLevel(b.Price, b.Size))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        var asks = rawBoard.Asks?
            .Select(a => new OrderBookLevel(a.Price, a.Size))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        return new OrderBook(bids, asks);
    }

    public static ExecutionMarket MapExecution(RawProductCode productCode, BitflyerExecutionPublicResponse raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new ExecutionMarket(
            ProductCode: BitflyerCommonMapper.ToApiProductCode(productCode),
            Id: raw.Id,
            Side: BitflyerCommonMapper.MapSide(raw.Side),
            Price: raw.Price,
            Size: raw.Size,
            ExecutedAt: raw.ExecDate);
    }
}
