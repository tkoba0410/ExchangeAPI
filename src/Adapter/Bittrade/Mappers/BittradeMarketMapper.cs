using System;
using System.Linq;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;

internal static class BittradeMarketMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static Ticker MapTicker(CommonSymbol symbol, BittradeTickerNormalized normalized)
    {
        return new Ticker(
            Exchange: Exchange,
            Symbol: symbol,
            LastTradedPrice: new Price(normalized.LastTradedPrice),
            Timestamp: normalized.Timestamp);
    }

    public static OrderBook MapOrderBook(BittradeOrderBookNormalized normalized)
    {
        var bids = normalized.Bids
            .Select(level => new OrderBookLevel(new Price(level.Price), new Size(level.Size)))
            .ToList();
        var asks = normalized.Asks
            .Select(level => new OrderBookLevel(new Price(level.Price), new Size(level.Size)))
            .ToList();

        return new OrderBook(Exchange, bids, asks);
    }

    public static ExecutionMarket MapExecution(CommonSymbol symbol, BittradeExecutionNormalized normalized)
    {
        return new ExecutionMarket(
            Exchange,
            symbol,
            normalized.Id,
            MapSide(normalized.Side),
            new Price(normalized.Price),
            new Size(normalized.Size),
            normalized.Timestamp);
    }

    private static Side MapSide(string direction) =>
        direction switch
        {
            var value when string.Equals(value, "buy", StringComparison.OrdinalIgnoreCase) => Side.Buy,
            var value when string.Equals(value, "sell", StringComparison.OrdinalIgnoreCase) => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported side: {direction}.", exchange: Exchange)
        };
}
