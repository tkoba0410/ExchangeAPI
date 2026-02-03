using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Utilities.OrderBook;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;

internal static class BittradeMarketMapper
{
    public static Ticker MapTicker(CommonSymbol symbol, BittradeTickerNormalized normalized)
    {
        return new Ticker(
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

        return OrderBookNormalizer.Normalize(bids, asks);
    }

    public static ExecutionMarket MapExecution(CommonSymbol symbol, BittradeExecutionNormalized normalized)
    {
        return new ExecutionMarket(
            Symbol: symbol,
            OrderId: normalized.Id,
            Side: MapSide(normalized.Side),
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.Timestamp);
    }

    private static Side MapSide(BittradeOrderSide side) =>
        side switch
        {
            BittradeOrderSide.Buy => Side.Buy,
            BittradeOrderSide.Sell => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported side: {side}.")
        };
}
