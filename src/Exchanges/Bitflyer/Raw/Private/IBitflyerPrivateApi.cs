using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Contracts.Common.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

/// <summary>
/// bitFlyer Private REST API への Raw アクセスインターフェース。
/// </summary>
public interface IBitflyerPrivateApi : IBitflyerRawAccountApi
{
    Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default);
}
