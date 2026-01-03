using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Call;

public sealed class BittradeNormalizedApi
{
    public BittradeNormalizedMarketDataFacade MarketData { get; }
    public BittradeNormalizedExchangeInfoFacade ExchangeInfo { get; }
    public BittradeNormalizedAccountFacade? Account { get; }
    public string? AccountId { get; }

    private BittradeNormalizedApi(
        BittradeNormalizedMarketDataFacade marketData,
        BittradeNormalizedExchangeInfoFacade exchangeInfo,
        BittradeNormalizedAccountFacade? account,
        string? accountId)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Account = account;
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }

    public static BittradeNormalizedApi FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var bundle = BittradeNormalizeFactory.FromRestClient(restClient, accountId);

        return new BittradeNormalizedApi(
            marketData: new BittradeNormalizedMarketDataFacade(bundle.MarketData),
            exchangeInfo: new BittradeNormalizedExchangeInfoFacade(bundle.ExchangeInfo),
            account: bundle.Account is null ? null : new BittradeNormalizedAccountFacade(bundle.Account),
            accountId: bundle.AccountId);
    }
}

public sealed class BittradeNormalizedMarketDataFacade
{
    private readonly IBittradeNormalizedMarketDataApi _inner;

    internal BittradeNormalizedMarketDataFacade(IBittradeNormalizedMarketDataApi inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public Task<BittradeTickerNormalized> GetTickerAsync(string symbol, CancellationToken ct = default) =>
        _inner.GetTickerAsync(symbol, ct);

    public Task<BittradeNormalizedCall<BittradeTickerNormalized, JsonElement>> GetTickerCallAsync(
        string symbol,
        CancellationToken ct = default) =>
        _inner.GetTickerCallAsync(symbol, ct);

    public Task<BittradeOrderBookNormalized> GetOrderBookAsync(string symbol, CancellationToken ct = default) =>
        _inner.GetOrderBookAsync(symbol, ct);

    public Task<BittradeNormalizedCall<BittradeOrderBookNormalized, JsonElement>> GetOrderBookCallAsync(
        string symbol,
        CancellationToken ct = default) =>
        _inner.GetOrderBookCallAsync(symbol, ct);

    public Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(
        string symbol,
        CancellationToken ct = default) =>
        _inner.GetExecutionsAsync(symbol, ct);

    public Task<BittradeNormalizedCall<IReadOnlyList<BittradeExecutionNormalized>, JsonElement>> GetExecutionsCallAsync(
        string symbol,
        CancellationToken ct = default) =>
        _inner.GetExecutionsCallAsync(symbol, ct);
}

public sealed class BittradeNormalizedExchangeInfoFacade
{
    private readonly IBittradeNormalizedExchangeInfoApi _inner;

    internal BittradeNormalizedExchangeInfoFacade(IBittradeNormalizedExchangeInfoApi inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public Task<IReadOnlyList<BittradeSymbolNormalized>> GetSymbolsAsync(CancellationToken ct = default) =>
        _inner.GetSymbolsAsync(ct);

    public Task<BittradeNormalizedCall<IReadOnlyList<BittradeSymbolNormalized>, JsonElement>> GetSymbolsCallAsync(
        CancellationToken ct = default) =>
        _inner.GetSymbolsCallAsync(ct);
}

public sealed class BittradeNormalizedAccountFacade
{
    private readonly IBittradeNormalizedAccountApi _inner;

    internal BittradeNormalizedAccountFacade(IBittradeNormalizedAccountApi inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public Task<IReadOnlyList<BittradeBalanceEntryNormalized>> GetBalancesAsync(CancellationToken ct = default) =>
        _inner.GetBalancesAsync(ct);

    public Task<BittradeNormalizedCall<IReadOnlyList<BittradeBalanceEntryNormalized>, JsonElement>> GetBalancesCallAsync(
        CancellationToken ct = default) =>
        _inner.GetBalancesCallAsync(ct);
}
