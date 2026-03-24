using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;

public interface IBitflyerPrivateProtocolApi
{
    Task<Call<ProtocolRequest, ProtocolResponse>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCoinInsCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCoinOutsCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetDepositsCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> WithdrawCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetWithdrawalsCallAsync(
        int? count,
        long? before,
        long? after,
        string? messageId,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetParentOrdersCallAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetParentOrderCallAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetChildOrdersCallAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetExecutionsCallAsync(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceHistoryCallAsync(
        string? currencyCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetPositionsCallAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetTradingCommissionCallAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> CancelChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> CancelParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}
