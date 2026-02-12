using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api.Markets;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal;

internal sealed class NormalizedMarketResolver : IMarketResolver
{
    private readonly IExchangeMarketResolver _inner;

    public NormalizedMarketResolver(IExchangeMarketResolver inner) =>
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public async Task<Call<ResolveBittradeMarketRequest, MarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new ResolveBittradeMarketRequest(symbol);
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

    private static Call<ResolveBittradeMarketRequest, MarketInfo> OkFromChild(
        ResolveBittradeMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        MarketInfo market)
    {
        var meta = new CallMeta(
            Layer: "Adapter",
            Component: "NormalizedMarketResolver",
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveBittradeMarketRequest, MarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<MarketInfo>.Ok(market),
            Meta: meta);
    }

    private static Call<ResolveBittradeMarketRequest, MarketInfo> ErrorFromChild(
        ResolveBittradeMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Adapter",
            Component: "NormalizedMarketResolver",
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveBittradeMarketRequest, MarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<MarketInfo>.Err(error),
            Meta: meta);
    }
}
