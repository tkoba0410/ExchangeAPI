using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Constants;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;

/// <summary>
/// Bittrade の Public REST 実装（Ticker/OrderBook/Executions）。
/// </summary>
internal sealed class MarketApi
{
    private readonly NormalizedPublicApi _marketData;
    private readonly IExchangeMarketResolver _markets;

    public MarketApi(NormalizedPublicApi marketData, IExchangeMarketResolver markets)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public Task<Call<TickerRequest, TickerResponse>> GetTickerAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDetailMergedCallAsync(symbol, cancellationToken);

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDepthCallAsync(symbol, cancellationToken);

    public async Task<Call<TickerRequest, TickerResponse>> GetDetailMergedCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new TickerRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionCallMapper.FromResolverError<TickerRequest, TickerResponse>(
                    request,
                    marketCall,
                    err.Error,
                    Operations.MarketData.GetTicker);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetDetailMergedCallAsync(productCode, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.MarketData.GetTicker,
                ok => MarketMapper.MapTicker(symbol, new TickerNormalized(
                    ok.LastTradedPrice,
                    ok.Timestamp,
                    ok.RawSnapshot,
                    ok.Extras)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return MarketResolutionCallMapper.SymbolNotSupported<TickerRequest, TickerResponse>(
                request,
                startedAt,
                Operations.MarketData.GetTicker,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<TickerRequest, TickerResponse>(
                request,
                startedAt,
                Operations.MarketData.GetTicker,
                ex);
        }
    }

    public async Task<Call<BoardRequest, BoardResponse>> GetDepthCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new BoardRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionCallMapper.FromResolverError<BoardRequest, BoardResponse>(
                    request,
                    marketCall,
                    err.Error,
                    Operations.MarketData.GetBoard);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetDepthCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.MarketData.GetBoard,
                ok => MarketMapper.MapOrderBook(new OrderBookNormalized(ok.Bids, ok.Asks)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return MarketResolutionCallMapper.SymbolNotSupported<BoardRequest, BoardResponse>(
                request,
                startedAt,
                Operations.MarketData.GetBoard,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<BoardRequest, BoardResponse>(
                request,
                startedAt,
                Operations.MarketData.GetBoard,
                ex);
        }
    }

    public async Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new ExecutionsPublicRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionCallMapper.FromResolverError<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                    request,
                    marketCall,
                    err.Error,
                    Operations.MarketData.GetExecutions);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetTradeCallAsync(productCode, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.MarketData.GetExecutions,
                ok => new ExecutionsPublicResponse(ToExecutionList(symbol, ok.Items)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return MarketResolutionCallMapper.SymbolNotSupported<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                request,
                startedAt,
                Operations.MarketData.GetExecutions,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                request,
                startedAt,
                Operations.MarketData.GetExecutions,
                ex);
        }
    }

    public async Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CommonSymbol symbol,
        PeriodDto period,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var request = new CandlesticksRequest(symbol, period, size);
        var startedAt = DateTimeOffset.UtcNow;

        if (period is null || period.IsEmpty)
        {
            return NotSupportedCall.Create<CandlesticksRequest, CandlesticksResponse>(
                "Contracts",
                Operations.MarketData.GetCandlesticks,
                request,
                "CandlesticksPeriod");
        }

        if (!CandlestickPeriods.TryGetTimescale(period.Code, out _))
        {
            return NotSupportedCall.Create<CandlesticksRequest, CandlesticksResponse>(
                "Contracts",
                Operations.MarketData.GetCandlesticks,
                request,
                $"CandlesticksPeriod:{period.Code}");
        }

        try
        {
            var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionCallMapper.FromResolverError<CandlesticksRequest, CandlesticksResponse>(
                    request,
                    marketCall,
                    err.Error,
                    Operations.MarketData.GetCandlesticks);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData
                .GetHistoryKlineCallAsync(productCode, new Period(period.Code), size, cancellationToken)
                .ConfigureAwait(false);

            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.MarketData.GetCandlesticks,
                ok => new CandlesticksResponse(MarketMapper.MapCandlesticks(symbol, period, ok.Items)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return MarketResolutionCallMapper.SymbolNotSupported<CandlesticksRequest, CandlesticksResponse>(
                request,
                startedAt,
                Operations.MarketData.GetCandlesticks,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CandlesticksRequest, CandlesticksResponse>(
                request,
                startedAt,
                Operations.MarketData.GetCandlesticks,
                ex);
        }
    }

    private static IReadOnlyList<ExecutionsPublicItem> ToExecutionList(
        CommonSymbol symbol,
        IReadOnlyList<ExecutionNormalized> executions)
    {
        IReadOnlyList<ExecutionsPublicItem> mapped = executions
            .Select(n => MarketMapper.MapExecution(symbol, n))
            .ToList();
        return mapped;
    }

}
