using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;

namespace ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;

internal sealed class ExchangeInfoMarketResolver : IExchangeMarketResolver
{
    private readonly IExchangeInfoProvider _exchangeInfo;
    private ExchangeInfoDto? _cache;

    public ExchangeInfoMarketResolver(IExchangeInfoProvider exchangeInfo) =>
        _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));

    public async Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(
        ResolveExchangeMarketRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var symbol = request.Symbol;

        if (symbol.IsEmpty)
        {
            return ErrorCall(request, startedAt, new CallError(CallErrorKind.Semantic, "symbol is required."));
        }

        var exchangeInfoCall = await _exchangeInfo.GetExchangeInfoAsync(new ExchangeInfoRequest(), cancellationToken).ConfigureAwait(false);
        if (exchangeInfoCall.Result is CallResult<ExchangeInfoDto>.Err err)
        {
            return ErrorFromChild(request, exchangeInfoCall, err.Error);
        }

        _cache ??= ((CallResult<ExchangeInfoDto>.Ok)exchangeInfoCall.Result).Response;
        var market = FindMarket(_cache, symbol);
        if (market is null)
        {
            return ErrorFromChild(request, exchangeInfoCall, new CallError(CallErrorKind.Semantic, $"Symbol not supported: {symbol.Value}"));
        }

        return OkFromChild(request, exchangeInfoCall, market);
    }

    private static ExchangeMarketInfo? FindMarket(ExchangeInfoDto info, Symbol symbol)
    {
        foreach (var market in info.Markets)
        {
            if (market.Symbol.Equals(symbol))
            {
                return market;
            }
        }

        return null;
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> OkFromChild(
        ResolveExchangeMarketRequest request,
        Call<ExchangeInfoRequest, ExchangeInfoDto> child,
        ExchangeMarketInfo market)
    {
        var meta = new CallMeta(
            Layer: CallMetaVocabulary.Layer.Contracts,
            Component: CallMetaVocabulary.Component.ExchangeInfoMarketResolver,
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<ExchangeMarketInfo>.Ok(market),
            Meta: meta);
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> ErrorFromChild(
        ResolveExchangeMarketRequest request,
        Call<ExchangeInfoRequest, ExchangeInfoDto> child,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: CallMetaVocabulary.Layer.Contracts,
            Component: CallMetaVocabulary.Component.ExchangeInfoMarketResolver,
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<ExchangeMarketInfo>.Err(error),
            Meta: meta);
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> ErrorCall(
        ResolveExchangeMarketRequest request,
        DateTimeOffset startedAt,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: CallMetaVocabulary.Layer.Contracts,
            Component: CallMetaVocabulary.Component.ExchangeInfoMarketResolver,
            EndpointId: CallMeta.InternalEndpointId,
            Tags: null,
            Children: null);

        return new Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: DateTimeOffset.UtcNow - startedAt,
            Request: request,
            Result: new CallResult<ExchangeMarketInfo>.Err(error),
            Meta: meta);
    }
}
