using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.Market.Ticker;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IPublicApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly BitflyerExchangeInfoApi _exchangeInfoApi;

    public IPublicApi? Public => this;
    public IPrivateApi? Private => null;

    internal BitflyerPublicClient(IBitflyerNormalizedApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _exchangeInfoApi = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(_exchangeInfoApi);
        _marketApi = new MarketApi(normalized, markets);
    }

    public Task<Call<GetTickerRequest, CommonTicker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookCallAsync(symbol, cancellationToken);

    public Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsCallAsync(symbol, cancellationToken);

    public Task<Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>> GetHistoryKlineCallAsync(
        Symbol symbol,
        string period,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetHistoryKlineCallAsync(symbol, period, size, cancellationToken);

    public Task<Call<GetTickersRequest, IReadOnlyList<CommonTicker>>> GetTickersCallAsync(
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickersCallAsync(cancellationToken);

    public Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetHistoryTradeCallAsync(symbol, cancellationToken);

    public Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    public Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetCurrencysCallAsync(cancellationToken);

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetTimestampCallAsync(cancellationToken);

    // Raw access removed from public facade.
}
