using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;

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

    Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrdersRequest, RawOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
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

    Task<Call<GetWithdrawVirtualAddressesRequest, RawWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetRetailOrderListCallAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailOrderDetailByOrderIdRequest, RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailAccountBalanceRequest, RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
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

    Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);
}
