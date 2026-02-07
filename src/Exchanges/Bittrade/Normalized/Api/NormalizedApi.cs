using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Api;

public sealed class NormalizedApi : IBittradeNormalizedApi
{
    private readonly NormalizedPublicApi _publicApi;
    private readonly NormalizedPrivateApi _privateApi;
    public FreeText AccountId { get; }

    private NormalizedApi(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi privateApi,
        FreeText accountId)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        AccountId = accountId;
    }

    internal static NormalizedApi FromRaw(
        IBittradeRawApi raw,
        IBittradeMarketResolver markets,
        FreeText accountId)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var normalizedAccountId = accountId;
        var publicApi = new NormalizedPublicApi(raw);
        var privateApi = new NormalizedPrivateApi(raw, markets, normalizedAccountId);

        return new NormalizedApi(
            publicApi: publicApi,
            privateApi: privateApi,
            accountId: normalizedAccountId);
    }

    public Task<Call<GetDetailMergedRequest, TickerNormalized>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken ct = default) =>
        _publicApi.GetDetailMergedCallAsync(productCode, ct);

    public Task<Call<GetDepthRequest, OrderBookNormalized>> GetDepthCallAsync(
        ProductCode productCode,
        DepthType? depthType = null,
        CancellationToken ct = default) =>
        _publicApi.GetDepthCallAsync(productCode, depthType, ct);

    public Task<Call<GetTradeRequest, IReadOnlyList<ExecutionNormalized>>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default) =>
        _publicApi.GetTradeCallAsync(productCode, ct);

    public Task<Call<GetSymbolsRequest, IReadOnlyList<SymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetSymbolsCallAsync(ct);

    public Task<Call<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetCurrencysCallAsync(ct);

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetTimestampCallAsync(ct);

    public Task<Call<GetHistoryKlineRequest, IReadOnlyList<KlineNormalized>>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        int? size = null,
        CancellationToken ct = default) =>
        _publicApi.GetHistoryKlineCallAsync(productCode, period, size, ct);

    public Task<Call<GetTickersRequest, IReadOnlyList<TickerEntryNormalized>>> GetTickersCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetTickersCallAsync(ct);

    public Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionNormalized>>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default) =>
        _publicApi.GetHistoryTradeCallAsync(productCode, ct);

    public Task<Call<GetAccountsRequest, IReadOnlyList<AccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetAccountsCallAsync(ct);

    public Task<Call<GetAccountsBalanceByAccountIdRequest, IReadOnlyList<BalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetAccountsBalanceByAccountIdCallAsync(ct);

    public Task<Call<GetDepositWithdrawRequest, IReadOnlyList<DepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetDepositWithdrawCallAsync(request, ct);

    public Task<Call<GetWithdrawVirtualAddressesRequest, IReadOnlyList<WithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetWithdrawVirtualAddressesCallAsync(ct);

    public Task<Call<GetRetailAccountBalanceRequest, IReadOnlyList<RetailBalanceEntryNormalized>>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetRetailAccountBalanceCallAsync(ct);

    public Task<Call<PostOrdersPlaceRequest, OrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersPlaceCallAsync(request, ct);

    public Task<Call<GetOrdersRequest, IReadOnlyList<OrderSummaryNormalized>>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOrdersCallAsync(ct);

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, CancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersSubmitCancelByOrderIdCallAsync(request.Symbol, request.OrderKey, ct);

    public Task<Call<PostOrdersBatchCancelRequest, CancelResult>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersBatchCancelCallAsync(request, ct);

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, CancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersBatchCancelOpenOrdersCallAsync(request, ct);

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOpenOrdersCallAsync(request.Symbol, ct);

    public Task<Call<GetOrdersByOrderIdRequest, OrderStatus>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOrdersByOrderIdCallAsync(request.Symbol, request.OrderKey, ct);

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<ExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOrdersMatchResultsByOrderIdCallAsync(request, ct);

    public Task<Call<GetMatchResultsRequest, IReadOnlyList<ExecutionNormalized>>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetMatchResultsCallAsync(request.Symbol, request.Limit, ct);

    public Task<Call<PostWithdrawApiCreateRequest, WithdrawResult>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawApiCreateCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, WithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawVirtualByAddressIdCreateCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, WithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, WithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, ct);

    public Task<Call<PostRetailOrderPlaceRequest, RetailOrderResult>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderPlaceCallAsync(request, ct);

    public Task<Call<GetRetailOrderListRequest, IReadOnlyList<RetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetRetailOrderListCallAsync(request, ct);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, RetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetRetailOrderDetailByOrderIdCallAsync(request, ct);

    public Task<Call<PostRetailOrderHistoryRequest, IReadOnlyList<RetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderHistoryCallAsync(request, ct);

    public Task<Call<PostRetailOrderDetailRequest, RetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderDetailCallAsync(request, ct);

    public Task<Call<PostRetailOrderCreateRequest, RetailOrderResult>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderCreateCallAsync(request, ct);

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, RetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderCancelByOrderIdCallAsync(request, ct);
}
