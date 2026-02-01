using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.DomainCommon.Enums;
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

        return new OrderBook(bids, asks);
    }

    public static ExecutionMarket MapExecution(CommonSymbol symbol, BittradeExecutionNormalized normalized)
    {
        return new ExecutionMarket(
            symbol,
            normalized.Id,
            MapSide(normalized.Side),
            new Price(normalized.Price),
            new Size(normalized.Size),
            normalized.Timestamp);
    }

    private static Side MapSide(BittradeOrderSide side) =>
        side switch
        {
            BittradeOrderSide.Buy => Side.Buy,
            BittradeOrderSide.Sell => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported side: {side}.")
        };
}
