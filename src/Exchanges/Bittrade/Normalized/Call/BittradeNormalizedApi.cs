using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Call;

public sealed class BittradeNormalizedApi
{
    private readonly IBittradeNormalizedMarketDataApi _marketData;
    private readonly IBittradeNormalizedExchangeInfoApi _exchangeInfo;
    private readonly IBittradeNormalizedAccountApi _account;
    private readonly IBittradeNormalizedTradingApi _trading;
    public string? AccountId { get; }

    private BittradeNormalizedApi(
        IBittradeNormalizedMarketDataApi marketData,
        IBittradeNormalizedExchangeInfoApi exchangeInfo,
        IBittradeNormalizedAccountApi account,
        IBittradeNormalizedTradingApi trading,
        string? accountId)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        _account = account ?? throw new ArgumentNullException(nameof(account));
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }

    public static BittradeNormalizedApi FromRestClient(
        IRestClient restClient,
        IBittradeMarketResolver? markets = null,
        string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var bundle = BittradeNormalizeFactory.FromRestClient(restClient, accountId);
        var normalizedAccountId = bundle.AccountId;
        IBittradeNormalizedTradingApi trading = string.IsNullOrWhiteSpace(normalizedAccountId)
            ? new BittradePreconditionMissingNormalizedTradingApi(string.Empty)
            : new BittradeNormalizedTradingApi(
                bundle.Raw,
                markets ?? throw new ArgumentNullException(nameof(markets)),
                normalizedAccountId);

        return new BittradeNormalizedApi(
            marketData: bundle.MarketData,
            exchangeInfo: bundle.ExchangeInfo,
            account: bundle.Account,
            trading: trading,
            accountId: normalizedAccountId);
    }

    public Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
        string productCode,
        CancellationToken ct = default) =>
        _marketData.GetDetailMergedCallAsync(productCode, ct);

    public Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
        string productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default) =>
        _marketData.GetDepthCallAsync(productCode, depthType, ct);

    public Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
        string productCode,
        CancellationToken ct = default) =>
        _marketData.GetTradeCallAsync(productCode, ct);

    public Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default) =>
        _exchangeInfo.GetSymbolsCallAsync(ct);

    public Task<Call<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default) =>
        _account.GetAccountsBalanceByAccountIdCallAsync(ct);

    public Task<Call<PlaceOrderRequest, BittradeOrderResult>> PlaceOrderCallAsync(
        BittradeOrderRequest request,
        CancellationToken ct = default) =>
        _trading.PlaceOrderCallAsync(request, ct);

    public Task<Call<CancelOrderRequest, BittradeCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        _trading.CancelOrderCallAsync(symbol, orderKey, ct);

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default) =>
        _trading.GetOpenOrdersCallAsync(symbol, ct);

    public Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        _trading.GetOrdersByOrderIdCallAsync(symbol, orderKey, ct);

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        _trading.GetMatchResultsCallAsync(symbol, limit, ct);
}
