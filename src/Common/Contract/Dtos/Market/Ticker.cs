using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>
/// 取引所共通フォーマットのティッカー情報。
/// Stage1 では最小限の情報のみを提供する。
/// </summary>
/// <param name="Exchange">取引所コード。</param>
/// <param name="Symbol">シンボル (例: "BTC/JPY")。</param>
/// <param name="LastTradedPrice">
/// 直近約定価格。取引所のレスポンスに無い場合は例外として扱う。
/// </param>
/// <param name="Timestamp">
/// ティッカーの発生時刻（UTC）。取引所のタイムスタンプ。
/// </param>
public sealed record Ticker(
    ExchangeCode Exchange,
    string Symbol,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
