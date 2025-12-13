using System.Collections.Generic;

namespace Common.Contract.Dtos;

public sealed record ExchangeInfo(
    IReadOnlyList<ExchangeMarketInfo> Markets,
    ExchangeFeatureFlags? Features,
    ExchangeRateLimits? RateLimits,
    ExchangeMaintenance? Maintenance);

/// <summary>
/// 取引可能な銘柄と、そのバリデーション向けメタ情報。
/// </summary>
/// <remarks>
/// MinSize/MaxSize/MinNotional/PriceIncrement/SizeIncrement はバリデーションのヒントとして利用し、欠損時は取引所デフォルトに従う。
/// IsSupported=false は「取引所に存在するがこのライブラリでは未サポート」を示し、StatusNote に理由を入れられる。
/// MakerFeeRate/TakerFeeRate は片道の手数料率 (例: 0.001 = 0.1%) を表し、負の値もリベートとして許容する。返せない場合は null にする。
/// FeeCurrency は手数料を徴収する通貨（null は約定通貨を意味する）。例: bitFlyer の BTC/JPY は BTC 徴収。
/// FeeType は Percentage/Flat を表す。Flat の場合は 1注文あたり固定額と解釈し、FeeCurrency と併せて使う。別トークン割引や特典は将来拡張で表現する前提。
/// </remarks>
public sealed record ExchangeMarketInfo(
    string Symbol,
    string ProductCode,
    string Type,
    decimal? MinSize = null,
    decimal? MaxSize = null,
    decimal? MinNotional = null,
    decimal? PriceIncrement = null,
    decimal? SizeIncrement = null,
    decimal? MakerFeeRate = null,
    decimal? TakerFeeRate = null,
    string? FeeCurrency = null,
    FeeType? FeeType = FeeType.Percentage,
    bool? IsSupported = null,
    string? StatusNote = null);

public sealed record ExchangeFeatureFlags(
    bool SupportsWebSocket,
    bool SupportsMargin,
    bool SupportsStopOrder,
    bool SupportsParentOrder,
    bool SupportsCandlestick,
    bool SupportsOrderBookDelta,
    bool SupportsRealtimeExecutions,
    bool SupportsWithdraw);

public sealed record ExchangeRateLimits(int? RequestsPerMinute, int? OrdersPerMinute);

public enum FeeType
{
    Percentage,
    Flat,
}

/// <summary>
/// メンテナンス情報（計画/臨時）。
/// </summary>
/// <param name="Status">現在の状態。null は不明を意味する。</param>
/// <param name="PlannedUntil">計画メンテ終了予定（UTC）。</param>
/// <param name="Message">任意の補足メッセージ。</param>
public sealed record ExchangeMaintenance(
    ExchangeMaintenanceStatus? Status,
    DateTimeOffset? PlannedUntil = null,
    string? Message = null);

public enum ExchangeMaintenanceStatus
{
    Normal,
    Planned,
    Unplanned,
}
