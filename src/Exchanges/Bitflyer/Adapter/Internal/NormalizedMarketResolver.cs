using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api.Markets;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

internal sealed class NormalizedMarketResolver : IMarketResolver
{
    private readonly IExchangeMarketResolver _inner;

    public NormalizedMarketResolver(IExchangeMarketResolver inner) =>
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public async Task<Call<ResolveMarketRequest, MarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new ResolveMarketRequest(symbol);
        var innerCall = await _inner.ResolveCallAsync(new ResolveExchangeMarketRequest(symbol), cancellationToken).ConfigureAwait(false);

        if (innerCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            return ErrorFromChild(request, innerCall, err.Error);
        }

        if (innerCall.Result is CallResult<ExchangeMarketInfo>.Ok ok &&
            !ok.Response.ProductCode.IsEmpty)
        {
            var market = new MarketInfo(symbol, ok.Response.ProductCode);
            return OkFromChild(request, innerCall, market);
        }

        return ErrorFromChild(
            request,
            innerCall,
            new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
    }

    private static Call<ResolveMarketRequest, MarketInfo> OkFromChild(
        ResolveMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        MarketInfo market)
    {
        var meta = new CallMeta(
            Layer: CallMetaVocabulary.Layer.Adapter,
            Component: CallMetaVocabulary.Component.NormalizedMarketResolver,
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveMarketRequest, MarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<MarketInfo>.Ok(market),
            Meta: meta);
    }

    private static Call<ResolveMarketRequest, MarketInfo> ErrorFromChild(
        ResolveMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: CallMetaVocabulary.Layer.Adapter,
            Component: CallMetaVocabulary.Component.NormalizedMarketResolver,
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveMarketRequest, MarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<MarketInfo>.Err(error),
            Meta: meta);
    }
}
