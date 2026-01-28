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
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

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

        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var wire = new WireTransport(restClient);
        var raw = new BittradeRawApi(wire);

        var marketData = new BittradeNormalizedMarketDataApi(raw);
        var exchangeInfo = new BittradeNormalizedExchangeInfoApi(raw);
        IBittradeNormalizedAccountApi account = normalizedAccountId is null
            ? new BittradePreconditionMissingNormalizedAccountApi(string.Empty)
            : new BittradeNormalizedAccountApi(raw, normalizedAccountId);

        IBittradeNormalizedTradingApi trading = string.IsNullOrWhiteSpace(normalizedAccountId)
            ? new BittradePreconditionMissingNormalizedTradingApi(string.Empty)
            : new BittradeNormalizedTradingApi(
                raw,
                markets ?? throw new ArgumentNullException(nameof(markets)),
                normalizedAccountId);

        return new BittradeNormalizedApi(
            marketData: marketData,
            exchangeInfo: exchangeInfo,
            account: account,
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

    public Task<Call<GetWithdrawVirtualAddressesRequest, IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default) =>
        _account.GetWithdrawVirtualAddressesCallAsync(ct);

    public Task<Call<GetRetailAccountBalanceRequest, IReadOnlyList<BittradeRetailBalanceEntryNormalized>>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default) =>
        _account.GetRetailAccountBalanceCallAsync(ct);

    public Task<Call<PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default) =>
        _trading.PostOrdersPlaceCallAsync(request, ct);

    public Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        CancellationToken ct = default) =>
        _trading.GetOrdersCallAsync(ct);

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

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default) =>
        _trading.PostWithdrawVirtualByAddressIdCreateCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default) =>
        _trading.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default) =>
        _trading.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, ct);

    public Task<Call<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default) =>
        _trading.PostRetailOrderPlaceCallAsync(request, ct);

    public Task<Call<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default) =>
        _trading.GetRetailOrderListCallAsync(request, ct);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default) =>
        _trading.GetRetailOrderDetailByOrderIdCallAsync(request, ct);

    public Task<Call<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default) =>
        _trading.PostRetailOrderHistoryCallAsync(request, ct);

    public Task<Call<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default) =>
        _trading.PostRetailOrderDetailCallAsync(request, ct);

    public Task<Call<PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default) =>
        _trading.PostRetailOrderCreateCallAsync(request, ct);

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        _trading.PostRetailOrderCancelByOrderIdCallAsync(request, ct);
}
