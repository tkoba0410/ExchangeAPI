using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;
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

    Task<Call<GetBalanceHistoryRequest, IReadOnlyList<JsonElement>>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, IReadOnlyList<JsonElement>>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, IReadOnlyList<JsonElement>>> GetAddressesAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, IReadOnlyList<JsonElement>>> GetCoinInsAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, IReadOnlyList<JsonElement>>> GetCoinOutsAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, IReadOnlyList<JsonElement>>> GetDepositsAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, IReadOnlyList<JsonElement>>> GetWithdrawalsAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, IReadOnlyList<JsonElement>>> GetBankAccountsAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default);
}
