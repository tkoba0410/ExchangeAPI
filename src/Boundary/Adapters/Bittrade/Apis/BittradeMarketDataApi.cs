using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade の Public REST 実装（Ticker/OrderBook/Executions）。
/// </summary>
internal sealed class BittradeMarketDataApi : IMarketDataApi
{
    private readonly IBittradeNormalizedMarketDataApi _marketData;
    private readonly IExchangeMarketResolver _markets;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeMarketDataApi(IBittradeNormalizedMarketDataApi marketData, IExchangeMarketResolver markets)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Ticker> GetTickerAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Market.GetTicker";
        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var normalized = await _marketData.GetTickerAsync(apiSymbol, cancellationToken).ConfigureAwait(false);
            return BittradeMarketMapper.MapTicker(symbol, normalized);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<OrderBook> GetOrderBookAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Market.GetOrderBook";
        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var normalized = await _marketData.GetOrderBookAsync(apiSymbol, cancellationToken).ConfigureAwait(false);
            return BittradeMarketMapper.MapOrderBook(normalized);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Market.GetExecutions";
        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var normalized = await _marketData.GetExecutionsAsync(apiSymbol, cancellationToken).ConfigureAwait(false);
            return normalized.Select(n => BittradeMarketMapper.MapExecution(symbol, n)).ToList();
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        CommonSymbol symbol,
        TimeSpan timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Candlesticks");
    }

    private async Task<string> ToApiSymbolAsync(CommonSymbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }
}
