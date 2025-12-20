using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using CommonTicker = ExchangeApi.Common.Dtos.Ticker;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Market;

/// <summary>
/// bitFlyer の MarketData API 実装（REST）。
/// </summary>
internal sealed class MarketApi : IMarketDataApi
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly ExchangeCode _exchange;

    public MarketApi(
        IBitflyerPublicApi publicApi,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _exchange = exchange;
    }

    public async Task<CommonTicker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var raw = await _publicApi.GetTickerRawAsync(BitflyerCommonMapper.ToApiProductCode(productCode), cancellationToken: cancellationToken).ConfigureAwait(false);
            return MarketMapper.MapTicker(BitflyerCommonMapper.ToApiProductCode(productCode), raw);
        }
        catch (SymbolNotSupportedException ex)
        {
            throw new ExchangeApiException(
                message: ex.Message,
                exchange: _exchange,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetTicker");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getticker API.",
                exchange: _exchange,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var rawBoard = await _publicApi.GetBoardRawAsync(BitflyerCommonMapper.ToApiProductCode(productCode), cancellationToken: cancellationToken).ConfigureAwait(false);
            return MarketMapper.MapOrderBook(rawBoard);
        }
        catch (SymbolNotSupportedException ex)
        {
            throw new ExchangeApiException(
                message: ex.Message,
                exchange: _exchange,
                operation: "GetOrderBook",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetOrderBook");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getboard API.",
                exchange: _exchange,
                operation: "GetOrderBook",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = BitflyerCommonMapper.MapSymbolToProductCode(symbol);
            var raw = await _publicApi.GetExecutionsRawAsync(BitflyerCommonMapper.ToApiProductCode(productCode), cancellationToken: cancellationToken).ConfigureAwait(false);

            var mapped = raw
                .Select(e => MarketMapper.MapExecution(productCode, e))
                .ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetMarketExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchange: _exchange,
                operation: "GetMarketExecutions",
                statusCode: null,
                innerException: ex);
        }
    }

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        Symbol symbol,
        TimeSpan timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        // bitFlyer RESTでは未サポート。将来Raw経由で実装する場合はここを置き換える。
        throw new ExchangeFeatureNotSupportedException(_exchange, "Candlesticks");
    }

    public async Task<HealthResponse> GetHealthAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
            return await _publicApi.GetHealthAsync(productCode, cancellationToken).ConfigureAwait(false);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetHealth");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer gethealth API.",
                exchange: _exchange,
                operation: "GetHealth",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<BoardStateResponse> GetBoardStateAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
            return await _publicApi.GetBoardStateAsync(productCode, cancellationToken).ConfigureAwait(false);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetBoardState");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getboardstate API.",
                exchange: _exchange,
                operation: "GetBoardState",
                statusCode: null,
                innerException: ex);
        }
    }
}
