using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.Market.Ticker;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IMarketDataApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly ITradingApi? _tradingApi;
    private readonly IAccountApi? _accountApi;
    private readonly ISpotHistoryApi? _historyApi;

    public IMarketDataApi? Market => _marketApi;
    public ITradingApi? Trading => _tradingApi;
    public IAccountApi? Account => _accountApi;
    public ISpotHistoryApi? History => _historyApi;

    internal BitflyerPublicClient(BitflyerNormalizedMarketDataFacade marketData)
    {
        if (marketData is null) throw new ArgumentNullException(nameof(marketData));
        var markets = new ExchangeInfoMarketResolver(new BitflyerExchangeInfoApi());
        _marketApi = new MarketApi(marketData, markets);
        _tradingApi = null;
        _accountApi = null;
        _historyApi = null;
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

    // Raw access removed from public facade.
}
