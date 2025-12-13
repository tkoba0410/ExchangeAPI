using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;

namespace Common.Contract.Interfaces;

/// <summary>
/// 取引（REST）の抽象インターフェース。
/// </summary>
public interface ITradingApi
{
    Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);

    Task<CancelResult> CancelOrderAsync(string productCode, string childOrderAcceptanceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(string productCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 注文の状態をポーリングし、完了/キャンセル/期限切れを検知する。
    /// </summary>
    /// <param name="productCode">シンボル（例: BTC_JPY）。</param>
    /// <param name="childOrderAcceptanceId">注文の acceptance id。</param>
    /// <param name="pollInterval">ポーリング間隔（null なら 1s）。</param>
    /// <param name="maxAttempts">最大試行回数（デフォルト 30）。</param>
    Task<OrderStatus> PollOrderStatusAsync(
        string productCode,
        string childOrderAcceptanceId,
        TimeSpan? pollInterval = null,
        int maxAttempts = 30,
        CancellationToken cancellationToken = default);
}
