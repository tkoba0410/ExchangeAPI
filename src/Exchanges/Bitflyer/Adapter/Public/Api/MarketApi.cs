using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;

internal sealed class MarketApi
{
    private readonly INormalizedApi _normalized;
    private readonly IExchangeMarketResolver _markets;

    public MarketApi(INormalizedApi normalized, IExchangeMarketResolver markets)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<TickerRequest, CommonTicker>> GetTickerAsync(
        TickerRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = request.Symbol;
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionCallMapper.FromResolverError<TickerRequest, CommonTicker>(
                    request,
                    marketCall,
                    err.Error,
                    Operations.MarketData.GetTicker);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _normalized.GetTickerCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.MarketData.GetTicker,
                ok => MarketMapper.MapTicker(symbol, new TickerNormalized(
                    ok.ProductCode,
                    ok.LastTradedPrice,
                    ok.Timestamp,
                    ok.RawSnapshot,
                    ok.Extras)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return MarketResolutionCallMapper.SymbolNotSupported<TickerRequest, CommonTicker>(
                request,
                startedAt,
                Operations.MarketData.GetTicker,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<TickerRequest, CommonTicker>(
                request,
                startedAt,
                Operations.MarketData.GetTicker,
                ex);
        }
    }

    public async Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        BoardRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = request.Symbol;
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
            var call = await _normalized.GetBoardCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        ExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = request.Symbol;
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
            var call = await _normalized.GetExecutionsPublicCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.MarketData.GetExecutions,
                ok => new ExecutionsPublicResponse(ToExecutionList(symbol, ok)));
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

    public Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CandlesticksRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NotSupportedCall.Create<CandlesticksRequest, CandlesticksResponse>(
            "Contracts",
            Operations.MarketData.GetCandlesticks,
            request,
            "Candlesticks"));
    }

    private static IReadOnlyList<ExecutionsPublicItem> ToExecutionList(
        Symbol symbol,
        GetExecutionsPublicResponse executions)
    {
        IReadOnlyList<ExecutionsPublicItem> mapped = executions.Items
            .Select(e => MarketMapper.MapExecution(symbol, e.Value))
            .ToArray();
        return mapped;
    }
}
