using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade の Public REST 実装（Ticker/OrderBook/Executions）。
/// </summary>
internal sealed class BittradeMarketDataApi : IMarketDataApi
{
    private readonly IRestClient _restClient;
    private readonly IExchangeMarketResolver _markets;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeMarketDataApi(IRestClient restClient, IExchangeMarketResolver markets)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Market.GetTimestamp";
        try
        {
            return await _restClient.GetAsync<TimestampResponse>(
                "v1/common/timestamp",
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<Ticker> GetTickerAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Market.GetTicker";
        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var response = await _restClient.GetAsync<MergedResponse>(
                $"market/detail/merged?symbol={apiSymbol}",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade ticker response is invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            var tick = response.Tick;
            var ts = response.Ts ?? tick.Ts;
            var timestamp = ts ?? DateTimeOffset.UtcNow;
            return new Ticker(
                Exchange: Exchange,
                Symbol: symbol,
                LastTradedPrice: new Price(tick.Close),
                Timestamp: timestamp);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
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
            var response = await _restClient.GetAsync<DepthResponse>(
                $"market/depth?symbol={apiSymbol}&type=step0",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade depth response is invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            var bids = response.Tick.Bids?.Select(ToLevel).ToList() ?? new List<OrderBookLevel>();
            var asks = response.Tick.Asks?.Select(ToLevel).ToList() ?? new List<OrderBookLevel>();

            return new OrderBook(Exchange, bids, asks);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
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
            var response = await _restClient.GetAsync<TradeResponse>(
                $"market/trade?symbol={apiSymbol}",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick?.Data is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade trades response is invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            var executions = response.Tick.Data
                .Select(d => new ExecutionMarket(
                    Exchange,
                    symbol,
                    d.Id.ToString(),
                    MapSide(d.Direction),
                    new Price(d.Price),
                    new Size(d.Amount),
                    d.Ts))
                .ToList();

            return executions;
        }
        catch (SymbolNotSupportedException)
        {
            throw;
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

    private static OrderBookLevel ToLevel(IReadOnlyList<decimal> level)
    {
        if (level.Count < 2) throw new ExchangeApiException("Invalid order book level.");
        return new OrderBookLevel(new Price(level[0]), new Size(level[1]));
    }

    private static Side MapSide(string direction) =>
        direction switch
        {
            var value when string.Equals(value, "buy", StringComparison.OrdinalIgnoreCase) => Side.Buy,
            var value when string.Equals(value, "sell", StringComparison.OrdinalIgnoreCase) => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported side: {direction}.", exchange: Exchange)
        };

    private async Task<string> ToApiSymbolAsync(CommonSymbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }
}
