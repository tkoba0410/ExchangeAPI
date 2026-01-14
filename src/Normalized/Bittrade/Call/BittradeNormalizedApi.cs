using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalize.Types;
using ExchangeApi.Spec.CallCommon;

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

    public Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
        BittradeSymbol symbol,
        CancellationToken ct = default) =>
        _inner.GetTickerCallAsync(symbol, ct);

    public Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetOrderBookCallAsync(
        BittradeSymbol symbol,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default) =>
        _inner.GetOrderBookCallAsync(symbol, depthType, ct);

    public Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
        BittradeSymbol symbol,
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

    public Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
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

    public Task<Call<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetBalancesCallAsync(
        CancellationToken ct = default) =>
        _inner.GetBalancesCallAsync(ct);
}
