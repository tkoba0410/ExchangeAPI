using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal;

internal sealed class BittradeNormalizedMarketResolver : IBittradeMarketResolver
{
    private readonly IExchangeMarketResolver _inner;

    public BittradeNormalizedMarketResolver(IExchangeMarketResolver inner) =>
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public async Task<Call<ResolveBittradeMarketRequest, BittradeMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var innerCall = await _inner.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new ResolveBittradeMarketRequest(symbol);

        if (innerCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            return ErrorFromChild(request, innerCall, err.Error);
        }

        if (innerCall.Result is CallResult<ExchangeMarketInfo>.Ok ok &&
            !string.IsNullOrWhiteSpace(ok.Response.ProductCode))
        {
            var market = new BittradeMarketInfo(symbol, ok.Response.ProductCode);
            return OkFromChild(request, innerCall, market);
        }

        return ErrorFromChild(
            request,
            innerCall,
            new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
    }

    private static Call<ResolveBittradeMarketRequest, BittradeMarketInfo> OkFromChild(
        ResolveBittradeMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        BittradeMarketInfo market)
    {
        var meta = new CallMeta(
            Layer: "Adapter",
            Component: "BittradeNormalizedMarketResolver",
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveBittradeMarketRequest, BittradeMarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<BittradeMarketInfo>.Ok(market),
            Meta: meta);
    }

    private static Call<ResolveBittradeMarketRequest, BittradeMarketInfo> ErrorFromChild(
        ResolveBittradeMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Adapter",
            Component: "BittradeNormalizedMarketResolver",
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveBittradeMarketRequest, BittradeMarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<BittradeMarketInfo>.Err(error),
            Meta: meta);
    }
}
