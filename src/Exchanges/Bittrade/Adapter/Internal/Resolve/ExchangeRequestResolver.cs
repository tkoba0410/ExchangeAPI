using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Resolve;

internal sealed class ExchangeRequestResolver : IExchangeMarketResolver
{
    private static readonly IReadOnlyDictionary<string, ExchangeMarketInfo> MarketBySymbol =
        BuildMarketIndex();

    public Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(
        ResolveExchangeMarketRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var symbol = request.Symbol;

        if (symbol.IsEmpty)
        {
            return Task.FromResult(CreateError(request, startedAt, new CallError(CallErrorKind.Semantic, "symbol is required.")));
        }

        if (!MarketBySymbol.TryGetValue(symbol.Value, out var market))
        {
            return Task.FromResult(CreateError(
                request,
                startedAt,
                new CallError(CallErrorKind.Semantic, $"Symbol not supported: {symbol.Value}")));
        }

        return Task.FromResult(CreateOk(request, startedAt, market));
    }

    private static IReadOnlyDictionary<string, ExchangeMarketInfo> BuildMarketIndex()
    {
        var map = new Dictionary<string, ExchangeMarketInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var market in ExchangeMarketCatalog.Markets)
        {
            var mapped = new ExchangeMarketInfo(
                Symbol: Symbol.ParseOrThrow(market.Symbol),
                ProductCode: ProductCode.ParseOrThrow(market.ProductCode),
                Type: MarketType.ParseOrThrow(market.Type));
            map[mapped.Symbol.Value] = mapped;
        }

        return map;
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> CreateOk(
        ResolveExchangeMarketRequest request,
        DateTimeOffset startedAt,
        ExchangeMarketInfo market)
    {
        var now = DateTimeOffset.UtcNow;
        return new Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: now - startedAt,
            Request: request,
            Result: new CallResult<ExchangeMarketInfo>.Ok(market),
            Meta: CreateMeta());
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> CreateError(
        ResolveExchangeMarketRequest request,
        DateTimeOffset startedAt,
        CallError error)
    {
        var now = DateTimeOffset.UtcNow;
        return new Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: now - startedAt,
            Request: request,
            Result: new CallResult<ExchangeMarketInfo>.Err(error),
            Meta: CreateMeta());
    }

    private static CallMeta CreateMeta() =>
        new(
            Layer: CallMetaVocabulary.Layer.Contracts,
            Component: CallMetaVocabulary.Component.MarketCatalogResolver,
            EndpointId: CallMeta.InternalEndpointId,
            Tags: null,
            Children: null);
}
