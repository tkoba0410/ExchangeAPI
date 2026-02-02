using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;

/// <summary>
/// Bittrade の Public REST 実装（Ticker/OrderBook/Executions）。
/// </summary>
internal sealed class MarketApi
{
    private readonly BittradeNormalizedPublicApi _marketData;
    private readonly IExchangeMarketResolver _markets;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public MarketApi(BittradeNormalizedPublicApi marketData, IExchangeMarketResolver markets)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDetailMergedCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDepthCallAsync(symbol, cancellationToken);

    public async Task<Call<GetTickerRequest, Ticker>> GetDetailMergedCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTickerRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetTickerRequest, Ticker>(
                    request,
                    marketCall,
                    err.Error,
                    BittradeOperations.MarketData.GetTicker);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetDetailMergedCallAsync(productCode, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetTicker,
                ok => BittradeMarketMapper.MapTicker(symbol, ok));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetTickerRequest, Ticker>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetTicker,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetTickerRequest, Ticker>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetTicker,
                ex);
        }
    }

    public async Task<Call<GetOrderBookRequest, OrderBook>> GetDepthCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderBookRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetOrderBookRequest, OrderBook>(
                    request,
                    marketCall,
                    err.Error,
                    BittradeOperations.MarketData.GetOrderBook);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetDepthCallAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetOrderBook,
                BittradeMarketMapper.MapOrderBook);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetOrderBookRequest, OrderBook>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetOrderBook,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOrderBookRequest, OrderBook>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetOrderBook,
                ex);
        }
    }

    public async Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetMarketExecutionsRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                    request,
                    marketCall,
                    err.Error,
                    BittradeOperations.MarketData.GetExecutions);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetTradeCallAsync(productCode, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetExecutions,
                ok => ToExecutionList(symbol, ok));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetExecutions,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetExecutions,
                ex);
        }
    }

    public Task<Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>> GetHistoryKlineCallAsync(
        CommonSymbol symbol,
        Period period,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        GetHistoryKlineInternalAsync(symbol, period, size, cancellationToken);

    public Task<Call<GetTickersRequest, IReadOnlyList<Ticker>>> GetTickersCallAsync(
        CancellationToken cancellationToken = default) =>
        GetTickersInternalAsync(cancellationToken);

    public Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetHistoryTradeInternalAsync(symbol, cancellationToken);

    private static IReadOnlyList<ExecutionMarket> ToExecutionList(
        CommonSymbol symbol,
        IReadOnlyList<BittradeExecutionNormalized> executions)
    {
        IReadOnlyList<ExecutionMarket> mapped = executions
            .Select(n => BittradeMarketMapper.MapExecution(symbol, n))
            .ToList();
        return mapped;
    }

    private async Task<Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>> GetHistoryKlineInternalAsync(
        CommonSymbol symbol,
        Period period,
        int? size,
        CancellationToken cancellationToken)
    {
        var request = new GetHistoryKlineRequest(symbol, period, size);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>(
                    request,
                    marketCall,
                    err.Error,
                    BittradeOperations.MarketData.GetCandlesticks);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData
                .GetHistoryKlineCallAsync(productCode, period, size, cancellationToken)
                .ConfigureAwait(false);

            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetCandlesticks,
                ok => MapCandlesticks(symbol, period, ok));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetCandlesticks,
                ex);
        }
    }

    private async Task<Call<GetTickersRequest, IReadOnlyList<Ticker>>> GetTickersInternalAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetTickersRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _marketData.GetTickersCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetTickers,
                MapTickers);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetTickersRequest, IReadOnlyList<Ticker>>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetTickers,
                ex);
        }
    }

    private async Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeInternalAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken)
    {
        var request = new GetHistoryTradeRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>(
                    request,
                    marketCall,
                    err.Error,
                    BittradeOperations.MarketData.GetHistoryTrade);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetHistoryTradeCallAsync(productCode, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetHistoryTrade,
                ok =>
                {
                    IReadOnlyList<ExecutionMarket> mapped = ok
                        .Select(e => BittradeMarketMapper.MapExecution(symbol, e))
                        .ToList();
                    return mapped;
                });
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetHistoryTrade,
                ex);
        }
    }

    private static IReadOnlyList<Candlestick> MapCandlesticks(
        CommonSymbol symbol,
        Period period,
        IReadOnlyList<BittradeKlineNormalized> klines)
    {
        var timescale = ParseTimescale(period);
        var candles = new List<Candlestick>(klines.Count);
        foreach (var kline in klines)
        {
            if (!long.TryParse(kline.Id, out var seconds))
            {
                throw new InvalidOperationException($"Invalid kline id: '{kline.Id}'.");
            }

            var openTime = DateTimeOffset.FromUnixTimeSeconds(seconds);
            var closeTime = timescale == TimeSpan.Zero ? openTime : openTime.Add(timescale);
            candles.Add(new Candlestick(
                Symbol: symbol,
                Timescale: timescale,
                OpenTime: openTime,
                CloseTime: closeTime,
                Open: kline.Open,
                High: kline.High,
                Low: kline.Low,
                Close: kline.Close,
                Volume: kline.Amount,
                IsFinal: true,
                QuoteVolume: kline.Volume,
                NumberOfTrades: kline.Count));
        }

        return candles;
    }

    private static TimeSpan ParseTimescale(Period period)
    {
        if (period.IsEmpty)
        {
            return TimeSpan.Zero;
        }

        return period.Value switch
        {
            "1min" => TimeSpan.FromMinutes(1),
            "3min" => TimeSpan.FromMinutes(3),
            "5min" => TimeSpan.FromMinutes(5),
            "15min" => TimeSpan.FromMinutes(15),
            "30min" => TimeSpan.FromMinutes(30),
            "60min" => TimeSpan.FromHours(1),
            "1day" => TimeSpan.FromDays(1),
            "1week" => TimeSpan.FromDays(7),
            "1mon" => TimeSpan.FromDays(30),
            "1year" => TimeSpan.FromDays(365),
            _ => TimeSpan.Zero,
        };
    }

    private static IReadOnlyList<Ticker> MapTickers(IReadOnlyList<BittradeTickerEntryNormalized> entries)
    {
        return entries
            .Select(entry => new Ticker(
                Symbol: new CommonSymbol(entry.Symbol),
                LastTradedPrice: new Price(entry.LastTradedPrice),
                Timestamp: entry.Timestamp))
            .ToList();
    }

    private static Call<TReq, TOk> MarketResolutionError<TReq, TOk>(
        TReq request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
        CallError error,
        string component)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: component,
            EndpointId: marketCall.Meta.EndpointId,
            Tags: null,
            Children: new[] { marketCall.Id });

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

    private static Call<TReq, TOk> SymbolNotSupported<TReq, TOk>(
        TReq request,
        DateTimeOffset startedAt,
        string component,
        Exception ex)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: component,
            EndpointId: CallMeta.InternalEndpointId,
            Tags: null,
            Children: null);
        var error = new CallError(CallErrorKind.Semantic, ex.Message, ex);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: DateTimeOffset.UtcNow - startedAt,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }
}
