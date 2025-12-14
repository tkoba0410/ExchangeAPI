using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Contract.Errors;

namespace Exchange.Bitflyer.Abstract;

/// <summary>
/// bitFlyer の MarketData API 実装（REST）。
/// </summary>
public sealed class BitflyerMarketApi : IMarketDataApi
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly string _exchangeId;

    public BitflyerMarketApi(
        IBitflyerPublicApi publicApi,
        string exchangeId = "bitFlyer")
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _exchangeId = exchangeId;
    }

    public async Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var raw = await _publicApi.GetTickerRawAsync(BitflyerCommonMapper.ToApiProductCode(productCode), cancellationToken: cancellationToken).ConfigureAwait(false);
            return BitflyerMarketMapper.MapTicker(symbol, raw);
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
            var rawBoard = await _publicApi.GetBoardRawAsync(BitflyerCommonMapper.ToApiProductCode(productCode), cancellationToken: cancellationToken).ConfigureAwait(false);
            return BitflyerMarketMapper.MapOrderBook(rawBoard);
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

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var raw = await _publicApi.GetExecutionsRawAsync(BitflyerCommonMapper.ToApiProductCode(productCode), cancellationToken: cancellationToken).ConfigureAwait(false);

            var mapped = raw
                .Select(e => BitflyerMarketMapper.MapExecution(productCode, e))
                .ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetMarketExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchangeId: _exchangeId,
                operation: "GetMarketExecutions",
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
}
