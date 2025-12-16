using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Enums;
namespace Common.Interfaces;

/// <summary>
/// 取引（REST）の抽象インターフェース。
/// </summary>
public interface ITradingApi
{
    Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        Side side,
        decimal size,
        decimal price,
        CancellationToken cancellationToken = default);

    Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        Side side,
        decimal size,
        CancellationToken cancellationToken = default);

    Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        Side side,
        decimal size,
        decimal triggerPrice,
        CancellationToken cancellationToken = default);

    Task<CancelResult> CancelOrderAsync(Symbol symbol, string childOrderAcceptanceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default);

    /// <summary>
    /// 注文の状態をポーリングし、完了/キャンセル/期限切れを検知する。
    /// </summary>
    /// <param name="symbol">シンボル。</param>
    /// <param name="orderId">注文の acceptance id。</param>
    /// <param name="pollInterval">ポーリング間隔（null なら 1s）。</param>
    /// <param name="maxAttempts">最大試行回数（デフォルト 30）。</param>
    Task<OrderStatus> PollOrderStatusAsync(
        Symbol symbol,
        string orderId,
        TimeSpan? pollInterval = null,
        int maxAttempts = 30,
        CancellationToken cancellationToken = default);
}
