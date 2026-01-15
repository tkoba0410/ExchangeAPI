using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Application.Extensions;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Application.Services;

public sealed class ExchangeInfoMarketResolver : IExchangeMarketResolver
{
    private readonly IExchangeInfoApi _exchangeInfo;
    private ExchangeInfo? _cache;

    public ExchangeInfoMarketResolver(IExchangeInfoApi exchangeInfo)
        => _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));

    public async Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken ct = default)
    {
        var request = new ResolveExchangeMarketRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        if (symbol.IsEmpty)
        {
            return ErrorCall(request, startedAt, new CallError(CallErrorKind.Semantic, "symbol is required."));
        }

        var exchangeInfoCall = await _exchangeInfo.GetExchangeInfoCallAsync(ct).ConfigureAwait(false);
        if (exchangeInfoCall.Result is CallResult<ExchangeInfo>.Err err)
        {
            return ErrorFromChild(request, exchangeInfoCall, err.Error);
        }

        _cache ??= ((CallResult<ExchangeInfo>.Ok)exchangeInfoCall.Result).Response;
        var market = _cache.FindMarket(symbol.Value);
        if (market is null)
        {
            return ErrorFromChild(request, exchangeInfoCall, new CallError(CallErrorKind.Semantic, $"Symbol not supported: {symbol.Value}"));
        }

        return OkFromChild(request, exchangeInfoCall, market);
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> OkFromChild(
        ResolveExchangeMarketRequest request,
        Call<GetExchangeInfoRequest, ExchangeInfo> child,
        ExchangeMarketInfo market)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: "ExchangeInfoMarketResolver",
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
        Call<GetExchangeInfoRequest, ExchangeInfo> child,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: "ExchangeInfoMarketResolver",
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
            Layer: "Contracts",
            Component: "ExchangeInfoMarketResolver",
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
