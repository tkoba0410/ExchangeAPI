using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Call;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Market;

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
        var call = await GetTickerCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, BitflyerOperations.MarketData.GetTicker);
    }

    public async Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var call = await GetOrderBookCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, BitflyerOperations.MarketData.GetOrderBook);
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var call = await GetMarketExecutionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, BitflyerOperations.MarketData.GetExecutions);
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
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        return await _marketData.GetHealthAsync(productCode, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BitflyerBoardStateNormalized> GetBoardStateAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        return await _marketData.GetBoardStateAsync(productCode, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Call<GetTickerRequest, CommonTicker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTickerRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            var call = await _marketData.GetTickerCallAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.MarketData.GetTicker,
                ok => MarketMapper.MapTicker(symbol, ok));
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetTickerRequest, CommonTicker>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetTicker,
                ex);
        }
    }

    public async Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderBookRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            var call = await _marketData.GetOrderBookCallAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.MarketData.GetOrderBook,
                MarketMapper.MapOrderBook);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderBookRequest, OrderBook>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetOrderBook,
                ex);
        }
    }

    public async Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetMarketExecutionsRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
            var call = await _marketData.GetExecutionsCallAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.MarketData.GetExecutions,
                ok => ToExecutionList(symbol, ok));
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetExecutions,
                ex);
        }
    }

    private async Task<string> ToApiProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode;
    }

    private static IReadOnlyList<ExecutionMarket> ToExecutionList(
        Symbol symbol,
        IReadOnlyList<BitflyerExecutionNormalized> executions)
    {
        IReadOnlyList<ExecutionMarket> mapped = executions
            .Select(e => MarketMapper.MapExecution(symbol, e))
            .ToArray();
        return mapped;
    }

    private static TOk Unwrap<TReq, TOk>(Call<TReq, TOk> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TOk>.Ok ok => ok.Response,
            CallResult<TOk>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.Error.HttpStatus),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error)),
            _ => throw new ExchangeApiException(
                message: "Unknown call result.",
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(new CallError(CallErrorKind.Unknown, "Unknown call result.")))
        };
    }
}
