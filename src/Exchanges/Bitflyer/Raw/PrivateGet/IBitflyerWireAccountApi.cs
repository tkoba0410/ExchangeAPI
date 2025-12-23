using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

public interface IBitflyerWireAccountApi
{
    Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<JsonElement> GetTradingCommissionAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
