using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawApi
{
    Task<Call<GetMergedTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        GetMergedTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepthRequest, RawDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetSymbolsRequest, RawSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCurrenciesRequest, RawCurrenciesResponse>> GetCurrencysCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTimestampRequest, RawTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetKlinesRequest, RawKlinesResponse>> GetHistoryKlineCallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickersRequest, RawTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradeHistoryRequest, RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
        GetTradeHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailMaintainTimeRequest, RawRetailMaintainTimeResponse>> GetMaintainTimeCallAsync(
        GetRetailMaintainTimeRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetOrderListCallAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualCancelByWithdrawIdCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);
}
