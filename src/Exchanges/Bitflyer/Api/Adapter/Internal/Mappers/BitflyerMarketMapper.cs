using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Utilities.OrderBook;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.Ticker;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;

internal static class MarketMapper
{
    public static CommonTicker MapTicker(Symbol symbol, BitflyerTickerNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        return new CommonTicker(
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

        if (!OrderBookNormalizer.TryNormalize(bids, asks, out var orderBook, out var error))
        {
            throw new ExchangeApiException(error?.Message ?? "OrderBook normalization failed.");
        }

        return orderBook!;
    }

    public static ExecutionMarket MapExecution(Symbol symbol, BitflyerExecutionNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        if (!BitflyerCommonMapper.TryMapSide(normalized.Side, out var side, out var error))
        {
            throw new ExchangeApiException(error?.Message ?? "bitFlyer side mapping failed.");
        }

        return new ExecutionMarket(
            Symbol: symbol,
            OrderId: OrderId.ParseOrThrow(normalized.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            Side: side,
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.ExecutedAt);
    }

}
