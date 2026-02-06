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

    public MarketApi(BittradeNormalizedPublicApi marketData, IExchangeMarketResolver markets)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDetailMergedCallAsync(symbol, cancellationToken);

    public Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDepthCallAsync(symbol, cancellationToken);

    public async Task<Call<GetTickerRequest, GetTickerResponse>> GetDetailMergedCallAsync(
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
                return MarketResolutionError<GetTickerRequest, GetTickerResponse>(
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
            return SymbolNotSupported<GetTickerRequest, GetTickerResponse>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetTicker,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetTickerRequest, GetTickerResponse>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetTicker,
                ex);
        }
    }

    public async Task<Call<GetBoardRequest, GetBoardResponse>> GetDepthCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetBoardRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetBoardRequest, GetBoardResponse>(
                    request,
                    marketCall,
                    err.Error,
                    BittradeOperations.MarketData.GetBoard);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _marketData.GetDepthCallAsync(productCode, ct: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.MarketData.GetBoard,
                BittradeMarketMapper.MapOrderBook);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetBoardRequest, GetBoardResponse>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetBoard,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBoardRequest, GetBoardResponse>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetBoard,
                ex);
        }
    }

    public async Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetExecutionsPublicRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetExecutionsPublicRequest, GetExecutionsPublicResponse>(
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
                ok => new GetExecutionsPublicResponse(ToExecutionList(symbol, ok)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetExecutionsPublicRequest, GetExecutionsPublicResponse>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetExecutions,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExecutionsPublicRequest, GetExecutionsPublicResponse>(
                request,
                startedAt,
                BittradeOperations.MarketData.GetExecutions,
                ex);
        }
    }

    private static IReadOnlyList<GetExecutionsPublicItem> ToExecutionList(
        CommonSymbol symbol,
        IReadOnlyList<BittradeExecutionNormalized> executions)
    {
        IReadOnlyList<GetExecutionsPublicItem> mapped = executions
            .Select(n => BittradeMarketMapper.MapExecution(symbol, n))
            .ToList();
        return mapped;
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
