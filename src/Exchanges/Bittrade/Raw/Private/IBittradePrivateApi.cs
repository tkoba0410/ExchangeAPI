using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw アクセス。
/// </summary>
internal interface IBittradePrivateApi
{
    Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountBalanceAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrderAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrderMatchResultsAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawsAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetRetailOrdersAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default);
}
