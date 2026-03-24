using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;

public interface IBitflyerPrivateProtocolApi
{
    Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceCallAsync(
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

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetPositionsCallAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> CancelChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}
