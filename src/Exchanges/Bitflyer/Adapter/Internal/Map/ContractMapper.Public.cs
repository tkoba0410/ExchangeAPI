using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Utilities.OrderBook;
using UtilityOrderBookNormalizer = ExchangeApi.Utilities.OrderBook.OrderBookNormalizer;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
using CommonBoard = ExchangeApi.Contracts.Common.Dtos.BoardResponse;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Map;

internal static class ContractMapper
{
    public static CommonTicker MapTicker(Symbol symbol, TickerNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        var ticker = new CommonTicker(
            Symbol: symbol,
            LastTradedPrice: new Price(normalized.LastTradedPrice),
            Timestamp: normalized.Timestamp);

        return ticker;
    }

    public static CommonBoard MapOrderBook(OrderBookNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        var bids = normalized.Bids
            .Select(b => new BoardLevel(new Price(b.Price), new Size(b.Size)))
            .ToArray();

        var asks = normalized.Asks
            .Select(a => new BoardLevel(new Price(a.Price), new Size(a.Size)))
            .ToArray();

        if (!UtilityOrderBookNormalizer.TryNormalize(bids, asks, out var orderBook, out var error))
        {
            throw new ExchangeApiException(error?.Message ?? "OrderBook normalization failed.");
        }

        return orderBook!;
    }

    public static ExecutionsPublicItem MapExecution(Symbol symbol, ExecutionNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        var side = MapSide(normalized.Side.ToString());

        return new ExecutionsPublicItem(
            Symbol: symbol,
            OrderId: OrderId.ParseOrThrow(normalized.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            Side: side,
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.ExecutedAt);
    }

    private static Side MapSide(string sideText)
    {
        if (string.Equals(sideText, "Buy", StringComparison.OrdinalIgnoreCase))
        {
            return Side.Buy;
        }

        if (string.Equals(sideText, "Sell", StringComparison.OrdinalIgnoreCase))
        {
            return Side.Sell;
        }

        throw new ExchangeApiException($"Unsupported side: {sideText}.");
    }
}
