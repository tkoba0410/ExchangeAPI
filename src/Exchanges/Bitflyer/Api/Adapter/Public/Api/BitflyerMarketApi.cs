using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.GetTickerResponse;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;

/// <summary>
/// bitFlyer の MarketData API 実装（REST）。
/// </summary>
internal sealed class MarketApi
{
    private readonly IBitflyerNormalizedApi _normalized;
    private readonly IExchangeMarketResolver _markets;

    public MarketApi(
        IBitflyerNormalizedApi normalized,
        IExchangeMarketResolver markets)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<GetTickerRequest, CommonTicker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTickerRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
            {
                return MarketResolutionError<GetTickerRequest, CommonTicker>(
                    request,
                    marketCall,
                    err.Error,
                    BitflyerOperations.MarketData.GetTicker);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _normalized.GetTickerCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.MarketData.GetTicker,
                ok => MarketMapper.MapTicker(symbol, ok));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetTickerRequest, CommonTicker>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetTicker,
                ex);
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

    public async Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        Symbol symbol,
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
                    BitflyerOperations.MarketData.GetBoard);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _normalized.GetBoardCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.MarketData.GetBoard,
                MarketMapper.MapOrderBook);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetBoardRequest, GetBoardResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetBoard,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBoardRequest, GetBoardResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetBoard,
                ex);
        }
    }

    public async Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        Symbol symbol,
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
                    BitflyerOperations.MarketData.GetExecutions);
            }

            var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
            var call = await _normalized.GetExecutionsPublicCallAsync(productCode, cancellationToken: cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.MarketData.GetExecutions,
                ok => new GetExecutionsPublicResponse(ToExecutionList(symbol, ok)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<GetExecutionsPublicRequest, GetExecutionsPublicResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetExecutions,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExecutionsPublicRequest, GetExecutionsPublicResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetExecutions,
                ex);
        }
    }

    private static IReadOnlyList<GetExecutionsPublicItem> ToExecutionList(
        Symbol symbol,
        IReadOnlyList<BitflyerExecutionNormalized> executions)
    {
        IReadOnlyList<GetExecutionsPublicItem> mapped = executions
            .Select(e => MarketMapper.MapExecution(symbol, e))
            .ToArray();
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
