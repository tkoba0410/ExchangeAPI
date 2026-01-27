using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Api;

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

    public Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken ct = default) =>
        _exchangeInfo.GetCurrencysCallAsync(ct);

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken ct = default) =>
        _exchangeInfo.GetTimestampCallAsync(ct);

    public Task<Call<GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>> GetHistoryKlineCallAsync(
        string productCode,
        string period,
        int? size = null,
        CancellationToken ct = default) =>
        _marketData.GetHistoryKlineCallAsync(productCode, period, size, ct);

    public Task<Call<GetTickersRequest, IReadOnlyList<BittradeTickerEntryNormalized>>> GetTickersCallAsync(
        CancellationToken ct = default) =>
        _marketData.GetTickersCallAsync(ct);

    public Task<Call<GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetHistoryTradeCallAsync(
        string productCode,
        CancellationToken ct = default) =>
        _marketData.GetHistoryTradeCallAsync(productCode, ct);

    public Task<Call<GetAccountsRequest, IReadOnlyList<BittradeAccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default) =>
        _account.GetAccountsCallAsync(ct);

    public Task<Call<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default) =>
        _account.GetAccountsBalanceByAccountIdCallAsync(ct);

    public Task<Call<GetDepositWithdrawRequest, IReadOnlyList<BittradeDepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default) =>
        _account.GetDepositWithdrawCallAsync(request, ct);

    public Task<Call<PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default) =>
        _trading.PostOrdersPlaceCallAsync(request, ct);

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        _trading.PostOrdersSubmitCancelByOrderIdCallAsync(symbol, orderKey, ct);

    public Task<Call<PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default) =>
        _trading.PostOrdersBatchCancelCallAsync(request, ct);

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default) =>
        _trading.PostOrdersBatchCancelOpenOrdersCallAsync(request, ct);

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default) =>
        _trading.GetOpenOrdersCallAsync(symbol, ct);

    public Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        _trading.GetOrdersByOrderIdCallAsync(symbol, orderKey, ct);

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default) =>
        _trading.GetOrdersMatchResultsByOrderIdCallAsync(request, ct);

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        _trading.GetMatchResultsCallAsync(symbol, limit, ct);

    public Task<Call<PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default) =>
        _trading.PostWithdrawApiCreateCallAsync(request, ct);

    public Task<Call<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default) =>
        _trading.PostRetailOrderPlaceCallAsync(request, ct);
}
