using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 発注系の抽象インターフェース。
/// Stage3 では MARKET 注文 1 本、Stage4 では注文拡張とキャンセル/ポジション系を扱う。
/// </summary>
public interface IExchangeTradingClient
{
    /// <summary>
    /// 注文を送信する。
    /// </summary>
    Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// child order をキャンセルする。
    /// </summary>
    Task<CancelResult> CancelOrderAsync(string productCode, string childOrderAcceptanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定 product_code の注文を全てキャンセルする。
    /// </summary>
    Task<CancelResult> CancelAllOrdersAsync(string productCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 建玉一覧を取得する。
    /// </summary>
    Task<IReadOnlyList<Position>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 約定履歴を取得する。
    /// </summary>
    Task<IReadOnlyList<Execution>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 証拠金情報を取得する。
    /// </summary>
    Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default);
}
