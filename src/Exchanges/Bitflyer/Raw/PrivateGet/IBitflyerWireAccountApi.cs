using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

public interface IBitflyerWireAccountApi
{
    Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
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

    Task<CollateralResponse> GetCollateralAsync(
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

    Task<JsonElement> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}
