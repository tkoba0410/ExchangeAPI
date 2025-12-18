using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Private REST API への Raw アクセスインターフェース。
/// </summary>
public interface IBitflyerPrivateApi
{
    Task<IReadOnlyList<string>> GetPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerExecutionPrivateResponse>> GetExecutionsAsync(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<BitflyerCollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerCollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerChildOrderResponse>> GetOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerParentOrderResponse>> GetParentOrdersAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default);

    Task<BitflyerParentOrderDetailResponse> GetParentOrderAsync(
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

    Task<JsonElement> GetTradingCommissionAsync(
        string productCode,
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
