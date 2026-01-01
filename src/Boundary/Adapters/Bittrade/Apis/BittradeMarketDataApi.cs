using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Spec.CallCommon;
using System.Text.Json;
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
        var call = await GetTickerCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.Market.GetTicker");
    }

    public async Task<OrderBook> GetOrderBookAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        var call = await GetOrderBookCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.Market.GetOrderBook");
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        var call = await GetMarketExecutionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.Market.GetExecutions");
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

    public async Task<ApiCall<GetTickerRequest, Ticker, ApiError>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTickerRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var call = await _marketData.GetTickerCallAsync(apiSymbol, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<BittradeTickerNormalized, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    BittradeMarketMapper.MapTicker(symbol, ok.Value)),
                Err<BittradeTickerNormalized, JsonElement> err => ApiCallMapper.Err<GetTickerRequest, Ticker>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetTickerRequest, Ticker>(Exchange, request, startedAt, ex);
        }
    }

    public async Task<ApiCall<GetOrderBookRequest, OrderBook, ApiError>> GetOrderBookCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderBookRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var call = await _marketData.GetOrderBookCallAsync(apiSymbol, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<BittradeOrderBookNormalized, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    BittradeMarketMapper.MapOrderBook(ok.Value)),
                Err<BittradeOrderBookNormalized, JsonElement> err => ApiCallMapper.Err<GetOrderBookRequest, OrderBook>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderBookRequest, OrderBook>(Exchange, request, startedAt, ex);
        }
    }

    public async Task<ApiCall<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>, ApiError>> GetMarketExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetMarketExecutionsRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var call = await _marketData.GetExecutionsCallAsync(apiSymbol, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<BittradeExecutionNormalized>, JsonElement> ok => ApiCallMapper.Ok<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ToExecutionList(symbol, ok.Value)),
                Err<IReadOnlyList<BittradeExecutionNormalized>, JsonElement> err => ApiCallMapper.Err<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                Exchange,
                request,
                startedAt,
                ex);
        }
    }

    private async Task<string> ToApiSymbolAsync(CommonSymbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }

    private static IReadOnlyList<ExecutionMarket> ToExecutionList(
        CommonSymbol symbol,
        IReadOnlyList<BittradeExecutionNormalized> executions)
    {
        IReadOnlyList<ExecutionMarket> mapped = executions
            .Select(n => BittradeMarketMapper.MapExecution(symbol, n))
            .ToList();
        return mapped;
    }

    private static TOk Unwrap<TReq, TOk>(ApiCall<TReq, TOk, ApiError> call, string operation)
    {
        return call.Result switch
        {
            ApiOk<TOk, ApiError> ok => ok.Value,
            ApiErr<TOk, ApiError> err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: call.Exchange,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.StatusCode),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error.Kind)),
            _ => throw new InvalidOperationException("Unsupported ApiCallResult type.")
        };
    }
}
