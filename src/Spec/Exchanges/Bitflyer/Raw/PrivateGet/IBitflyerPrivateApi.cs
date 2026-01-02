using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

/// <summary>
/// bitFlyer Private REST API への Raw アクセスインターフェース。
/// </summary>
#pragma warning disable CS0618
public interface IBitflyerPrivateApi : IBitflyerWireAccountApi
{
    Task<IReadOnlyList<string>> GetPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ParentOrderResponse>> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default);

    Task<ParentOrderDetailResponse> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetAddressesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(
        CancellationToken cancellationToken = default);
}
#pragma warning restore CS0618
