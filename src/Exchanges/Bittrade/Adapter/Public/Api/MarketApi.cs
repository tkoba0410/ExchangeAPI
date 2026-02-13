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
        TickerRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return GetDetailMergedCallAsync(request.Symbol, cancellationToken);
    }

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        BoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return GetDepthCallAsync(request.Symbol, cancellationToken);
    }

    public async Task<Call<TickerRequest, TickerResponse>> GetDetailMergedCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new TickerRequest(symbol);
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
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                Operations.MarketData.GetTicker,
                ct => _marketData.GetDetailMergedCallAsync(productCode, ct),
                ok => MarketMapper.MapTicker(symbol, new TickerNormalized(
                    ok.LastTradedPrice,
                    ok.Timestamp,
                    ok.RawSnapshot,
                    ok.Extras)),
                cancellationToken,
                (startedAt, ex) => TryMapSymbolNotSupported<TickerRequest, TickerResponse>(
                    request,
                    startedAt,
                    Operations.MarketData.GetTicker,
                    ex))
            .ConfigureAwait(false);
    }

    public async Task<Call<BoardRequest, BoardResponse>> GetDepthCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new BoardRequest(symbol);
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
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                Operations.MarketData.GetBoard,
                ct => _marketData.GetDepthCallAsync(productCode, cancellationToken: ct),
                ok => MarketMapper.MapOrderBook(new OrderBookNormalized(ok.Bids, ok.Asks)),
                cancellationToken,
                (startedAt, ex) => TryMapSymbolNotSupported<BoardRequest, BoardResponse>(
                    request,
                    startedAt,
                    Operations.MarketData.GetBoard,
                    ex))
            .ConfigureAwait(false);
    }

    public async Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        ExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var symbol = request.Symbol;
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
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                Operations.MarketData.GetExecutions,
                ct => _marketData.GetTradeCallAsync(productCode, ct),
                ok => new ExecutionsPublicResponse(ToExecutionList(symbol, ok.Items)),
                cancellationToken,
                (startedAt, ex) => TryMapSymbolNotSupported<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                    request,
                    startedAt,
                    Operations.MarketData.GetExecutions,
                    ex))
            .ConfigureAwait(false);
    }

    public async Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CandlesticksRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = request.Symbol;
        var period = request.Period;
        var size = request.Size;
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
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                Operations.MarketData.GetCandlesticks,
                ct => _marketData.GetHistoryKlineCallAsync(productCode, new Period(period.Code), size, ct),
                ok => new CandlesticksResponse(MarketMapper.MapCandlesticks(symbol, period, ok.Items)),
                cancellationToken,
                (startedAt, ex) => TryMapSymbolNotSupported<CandlesticksRequest, CandlesticksResponse>(
                    request,
                    startedAt,
                    Operations.MarketData.GetCandlesticks,
                    ex))
            .ConfigureAwait(false);
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

    private static Call<TReq, TOk>? TryMapSymbolNotSupported<TReq, TOk>(
        TReq request,
        DateTimeOffset startedAt,
        string operation,
        Exception ex)
    {
        if (ex is InvalidOperationException invalidOperationException &&
            invalidOperationException.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return MarketResolutionCallMapper.SymbolNotSupported<TReq, TOk>(
                request,
                startedAt,
                operation,
                invalidOperationException);
        }

        return null;
    }
}
