using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

public interface IBitflyerRawAccountApi
{
    Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<IReadOnlyList<BalanceResponse>, JsonElement>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<IReadOnlyList<ExecutionPrivateResponse>, JsonElement>> GetExecutionsCallAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<IReadOnlyList<PositionResponse>, JsonElement>> GetPositionsCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<CollateralResponse, JsonElement>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<IReadOnlyList<ChildOrderResponse>, JsonElement>> GetChildOrdersCallAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<JsonElement> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<JsonElement, JsonElement>> GetTradingCommissionCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}

[Obsolete("Use IBitflyerRawAccountApi instead. This interface will be removed in a future major release.")]
public interface IBitflyerWireAccountApi : IBitflyerRawAccountApi
{
}
