using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeInfoDto = ExchangeApi.Contracts.Dtos.ExchangeInfo.ExchangeInfo;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;

internal sealed class ExchangeInfoMarketResolver : IExchangeMarketResolver
{
    private readonly IExchangeInfoApi _exchangeInfo;
    private ExchangeInfoDto? _cache;

    public ExchangeInfoMarketResolver(IExchangeInfoApi exchangeInfo) =>
        _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));

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
        if (exchangeInfoCall.Result is CallResult<ExchangeInfoDto>.Err err)
        {
            return ErrorFromChild(request, exchangeInfoCall, err.Error);
        }

        _cache ??= ((CallResult<ExchangeInfoDto>.Ok)exchangeInfoCall.Result).Response;
        var market = FindMarket(_cache, symbol.Value);
        if (market is null)
        {
            return ErrorFromChild(request, exchangeInfoCall, new CallError(CallErrorKind.Semantic, $"Symbol not supported: {symbol.Value}"));
        }

        return OkFromChild(request, exchangeInfoCall, market);
    }

    private static ExchangeMarketInfo? FindMarket(ExchangeInfoDto info, string symbol)
    {
        foreach (var market in info.Markets)
        {
            if (string.Equals(market.Symbol, symbol, StringComparison.Ordinal))
            {
                return market;
            }
        }

        return null;
    }

    private static Call<ResolveExchangeMarketRequest, ExchangeMarketInfo> OkFromChild(
        ResolveExchangeMarketRequest request,
        Call<GetExchangeInfoRequest, ExchangeInfoDto> child,
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
        Call<GetExchangeInfoRequest, ExchangeInfoDto> child,
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
