using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Operations;
using ExchangeApi.Utilities.Operations;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Common.Adapter.Internal;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;

internal sealed class MarketApi
{
    private static readonly string OpGetTicker = OperationNameBuilder.WithExchange("Bitflyer", ContractOperations.MarketData.GetTicker);
    private static readonly string OpGetBoard = OperationNameBuilder.WithExchange("Bitflyer", ContractOperations.MarketData.GetBoard);
    private static readonly string OpGetExecutions = OperationNameBuilder.WithExchange("Bitflyer", ContractOperations.MarketData.GetExecutions);

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
        var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            return MarketResolutionCallMapper.FromResolverError<TickerRequest, CommonTicker>(
                request,
                marketCall,
                err.Error,
                OpGetTicker);
        }

        var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
        return await AdapterCallExecutor.ExecuteMapCallAsync(
            request,
            OpGetTicker,
            ct => _normalized.GetTickerCallAsync(productCode, cancellationToken: ct),
            ok => MarketMapper.MapTicker(symbol, new TickerNormalized(
                ok.ProductCode,
                ok.LastTradedPrice,
                ok.Timestamp,
                ok.RawSnapshot,
                ok.Extras)),
            cancellationToken,
            (startedAt, ex) => TryMapSymbolNotSupported<TickerRequest, CommonTicker>(
                request,
                startedAt,
                OpGetTicker,
                ex))
            .ConfigureAwait(false);
    }

    public async Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        BoardRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = request.Symbol;
        var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            return MarketResolutionCallMapper.FromResolverError<BoardRequest, BoardResponse>(
                request,
                marketCall,
                err.Error,
                OpGetBoard);
        }

        var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
        return await AdapterCallExecutor.ExecuteMapCallAsync(
            request,
            OpGetBoard,
            ct => _normalized.GetBoardCallAsync(productCode, cancellationToken: ct),
            ok => MarketMapper.MapOrderBook(new OrderBookNormalized(ok.Bids, ok.Asks)),
            cancellationToken,
            (startedAt, ex) => TryMapSymbolNotSupported<BoardRequest, BoardResponse>(
                request,
                startedAt,
                OpGetBoard,
                ex))
            .ConfigureAwait(false);
    }

    public async Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        ExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = request.Symbol;
        var marketCall = await _markets.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            return MarketResolutionCallMapper.FromResolverError<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                request,
                marketCall,
                err.Error,
                OpGetExecutions);
        }

        var productCode = ((CallResult<ExchangeMarketInfo>.Ok)marketCall.Result).Response.ProductCode;
        return await AdapterCallExecutor.ExecuteMapCallAsync(
            request,
            OpGetExecutions,
            ct => _normalized.GetExecutionsPublicCallAsync(productCode, cancellationToken: ct),
            ok => new ExecutionsPublicResponse(ToExecutionList(symbol, ok)),
            cancellationToken,
            (startedAt, ex) => TryMapSymbolNotSupported<ExecutionsPublicRequest, ExecutionsPublicResponse>(
                request,
                startedAt,
                OpGetExecutions,
                ex))
            .ConfigureAwait(false);
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
