using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IPublicApi, IExchangeClient
{
    private readonly IBitflyerNormalizedApi _normalized;
    private readonly IExchangeMarketResolver _markets;
    private readonly BitflyerExchangeInfoApi _exchangeInfoApi;

    public IPublicApi? Public => this;
    public IPrivateApi? Private => null;

    internal BitflyerPublicClient(IBitflyerNormalizedApi normalized, BitflyerExchangeInfoApi exchangeInfo)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _exchangeInfoApi = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        _normalized = normalized;
        _markets = new ExchangeInfoMarketResolver(_exchangeInfoApi);
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
                return MarketResolutionError<TickerRequest, CommonTicker>(
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
            return SymbolNotSupported<TickerRequest, CommonTicker>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetTicker,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<TickerRequest, CommonTicker>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetTicker,
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
                return MarketResolutionError<BoardRequest, BoardResponse>(
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
            return SymbolNotSupported<BoardRequest, BoardResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetBoard,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<BoardRequest, BoardResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetBoard,
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
                return MarketResolutionError<ExecutionsPublicRequest, ExecutionsPublicResponse>(
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
                ok => new ExecutionsPublicResponse(ToExecutionList(symbol, ok)));
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SymbolNotSupported:", StringComparison.Ordinal))
        {
            return SymbolNotSupported<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetExecutions,
                ex);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                request,
                startedAt,
                BitflyerOperations.MarketData.GetExecutions,
                ex);
        }
    }

    public Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CandlesticksRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NotSupportedCall.Create<CandlesticksRequest, CandlesticksResponse>(
            "Contracts",
            BitflyerOperations.MarketData.GetCandlesticks,
            request,
            "Candlesticks"));
    }

    public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        ExchangeInfoRequest request,
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(request, cancellationToken);

    private static IReadOnlyList<ExecutionsPublicItem> ToExecutionList(
        Symbol symbol,
        IReadOnlyList<BitflyerExecutionNormalized> executions)
    {
        IReadOnlyList<ExecutionsPublicItem> mapped = executions
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

    // Raw access removed from public facade.
}
