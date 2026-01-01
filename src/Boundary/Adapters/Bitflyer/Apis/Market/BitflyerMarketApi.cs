using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Facade;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Market;

/// <summary>
/// bitFlyer の MarketData API 実装（REST）。
/// </summary>
internal sealed class MarketApi : IMarketDataApi
{
    private readonly BitflyerNormalizedMarketDataFacade _marketData;
    private readonly IExchangeMarketResolver _markets;
    private readonly ExchangeCode _exchange;

    public MarketApi(
        BitflyerNormalizedMarketDataFacade marketData,
        IExchangeMarketResolver markets,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
        _exchange = exchange;
    }

    public async Task<CommonTicker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.MarketData.GetTicker;
        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            var normalized = await _marketData.GetTickerAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return MarketMapper.MapTicker(symbol, normalized);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getticker API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.MarketData.GetOrderBook;
        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            var normalized = await _marketData.GetOrderBookAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return MarketMapper.MapOrderBook(normalized);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getboard API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.MarketData.GetExecutions;
        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            var normalized = await _marketData.GetExecutionsAsync(productCode, ct: cancellationToken).ConfigureAwait(false);

            var mapped = normalized
                .Select(e => MarketMapper.MapExecution(symbol, e))
                .ToArray();

            return mapped;
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchange: _exchange,
                operation: operation,
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

    public async Task<BitflyerHealthNormalized> GetHealthAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.MarketData.GetHealth;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            return await _marketData.GetHealthAsync(productCode, cancellationToken).ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer gethealth API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<BitflyerBoardStateNormalized> GetBoardStateAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.MarketData.GetBoardState;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            return await _marketData.GetBoardStateAsync(productCode, cancellationToken).ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getboardstate API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    private async Task<string> ToApiProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode;
    }
}
