using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Utilities.OrderBook;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.GetTickerResponse;
using CommonBoard = ExchangeApi.Contracts.Common.Dtos.GetBoardResponse;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;

internal static class MarketMapper
{
    public static CommonTicker MapTicker(Symbol symbol, BitflyerTickerNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        var ticker = new CommonTicker(
            Symbol: symbol,
            LastTradedPrice: new Price(normalized.LastTradedPrice),
            Timestamp: normalized.Timestamp);

        return ticker;
    }

    public static CommonBoard MapOrderBook(BitflyerOrderBookNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));

        var bids = normalized.Bids
            .Select(b => new GetBoardLevel(new Price(b.Price), new Size(b.Size)))
            .ToArray();

        var asks = normalized.Asks
            .Select(a => new GetBoardLevel(new Price(a.Price), new Size(a.Size)))
            .ToArray();

        if (!OrderBookNormalizer.TryNormalize(bids, asks, out var orderBook, out var error))
        {
            throw new ExchangeApiException(error?.Message ?? "OrderBook normalization failed.");
        }

        return orderBook!;
    }

    public static GetExecutionsPublicItem MapExecution(Symbol symbol, BitflyerExecutionNormalized normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        if (!BitflyerCommonMapper.TryMapSide(normalized.Side, out var side, out var error))
        {
            throw new ExchangeApiException(error?.Message ?? "bitFlyer side mapping failed.");
        }

        return new GetExecutionsPublicItem(
            Symbol: symbol,
            OrderId: OrderId.ParseOrThrow(normalized.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            Side: side,
            Price: new Price(normalized.Price),
            Size: new Size(normalized.Size),
            ExecutedAt: normalized.ExecutedAt);
    }

}
