using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Adapters;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Apis.Market;

/// <summary>
/// bitFlyer の MarketData API 実装（REST）。
/// </summary>
public sealed class BitflyerMarketApi : IMarketDataApi
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly string _exchangeId;

    public BitflyerMarketApi(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        string exchangeId = "bitFlyer")
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _exchangeId = exchangeId;
    }

    public async Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var raw = await _publicApi.GetTickerRawAsync(productCode, cancellationToken).ConfigureAwait(false);
            return MapToTicker(symbol, raw);
        }
        catch (SymbolNotSupportedException ex)
        {
            throw new ExchangeApiException(
                message: ex.Message,
                exchangeId: _exchangeId,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetTicker");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getticker API.",
                exchangeId: _exchangeId,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var rawBoard = await _publicApi.GetBoardRawAsync(productCode, cancellationToken).ConfigureAwait(false);
            return MapToOrderBook(symbol, rawBoard);
        }
        catch (SymbolNotSupportedException ex)
        {
            throw new ExchangeApiException(
                message: ex.Message,
                exchangeId: _exchangeId,
                operation: "GetOrderBook",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetOrderBook");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getboard API.",
                exchangeId: _exchangeId,
                operation: "GetOrderBook",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<Execution>> GetExecutionsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var raw = await _privateApi.GetExecutionsAsync(productCode, cancellationToken).ConfigureAwait(false);

            var mapped = raw
                .Select(e => new Execution(
                    ProductCode: e.ProductCode,
                    Id: e.Id,
                    Side: BitflyerCommonMapper.MapSide(e.Side),
                    Price: e.Price,
                    Size: e.Size,
                    ExecutedAt: e.ExecDate,
                    ChildOrderAcceptanceId: e.ChildOrderAcceptanceId))
                .ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchangeId: _exchangeId,
                operation: "GetExecutions",
                statusCode: null,
                innerException: ex);
        }
    }

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        string symbol,
        string timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        // bitFlyer RESTでは未サポート。将来Raw経由で実装する場合はここを置き換える。
        throw new ExchangeApiException(
            message: "bitFlyer does not support candlesticks via REST.",
            exchangeId: _exchangeId,
            operation: "GetCandlesticks",
            statusCode: null,
            exchangeErrorCode: "NOT_SUPPORTED");
    }

    private static Ticker MapToTicker(string symbol, BitflyerTickerRaw raw)
    {
        return new Ticker(
            Symbol: symbol,
            BestBid: raw.BestBid,
            BestAsk: raw.BestAsk,
            LastTradedPrice: raw.LastTradedPrice,
            Timestamp: raw.Timestamp);
    }

    private static OrderBook MapToOrderBook(string symbol, BitflyerBoardRaw rawBoard)
    {
        var bids = rawBoard.Bids?
            .Select(b => new OrderBookLevel(b.Price, b.Size))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        var asks = rawBoard.Asks?
            .Select(a => new OrderBookLevel(a.Price, a.Size))
            .ToArray() ?? Array.Empty<OrderBookLevel>();

        return new OrderBook(
            Bids: bids,
            Asks: asks,
            MidPrice: rawBoard.MidPrice);
    }
}
