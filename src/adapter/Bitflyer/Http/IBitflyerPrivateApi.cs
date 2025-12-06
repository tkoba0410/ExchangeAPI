using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer Private REST API への Raw アクセスインターフェース。
/// Stage2 では /v1/me/getbalance のみ対象とする。
/// </summary>
public interface IBitflyerPrivateApi
{
    Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<BitflyerCollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default);
}
