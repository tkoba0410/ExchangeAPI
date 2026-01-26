using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;

internal sealed class BitflyerNormalizedMarketResolver : IBitflyerMarketResolver
{
    private readonly IExchangeMarketResolver _inner;

    public BitflyerNormalizedMarketResolver(IExchangeMarketResolver inner) =>
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public async Task<Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var innerCall = await _inner.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new ResolveBitflyerMarketRequest(symbol);

        if (innerCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            return ErrorFromChild(request, innerCall, err.Error);
        }

        if (innerCall.Result is CallResult<ExchangeMarketInfo>.Ok ok &&
            !string.IsNullOrWhiteSpace(ok.Response.ProductCode))
        {
            var market = new BitflyerMarketInfo(symbol, ok.Response.ProductCode);
            return OkFromChild(request, innerCall, market);
        }

        return ErrorFromChild(
            request,
            innerCall,
            new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
    }

    private static Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo> OkFromChild(
        ResolveBitflyerMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        BitflyerMarketInfo market)
    {
        var meta = new CallMeta(
            Layer: "Adapter",
            Component: "BitflyerNormalizedMarketResolver",
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<BitflyerMarketInfo>.Ok(market),
            Meta: meta);
    }

    private static Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo> ErrorFromChild(
        ResolveBitflyerMarketRequest request,
        Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> child,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Adapter",
            Component: "BitflyerNormalizedMarketResolver",
            EndpointId: child.Meta.EndpointId,
            Tags: null,
            Children: new[] { child.Id });

        return new Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo>(
            Id: CallId.New(),
            StartedAt: child.StartedAt,
            Duration: child.Duration,
            Request: request,
            Result: new CallResult<BitflyerMarketInfo>.Err(error),
            Meta: meta);
    }
}
