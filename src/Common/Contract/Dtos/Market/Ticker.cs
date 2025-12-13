namespace Common.Contract.Dtos;

/// <summary>
/// 取引所共通フォーマットのティッカー情報。
/// Stage1 では最小限の情報のみを提供する。
/// </summary>
/// <param name="Symbol">シンボル (例: "BTC/JPY")。</param>
/// <param name="BestBid">最良買い気配価格。</param>
/// <param name="BestAsk">最良売り気配価格。</param>
/// <param name="LastTradedPrice">
/// 直近約定価格。取引所のレスポンスに無い場合は例外として扱う。
/// </param>
/// <param name="Timestamp">
/// ティッカーの発生時刻（UTC）。取引所のタイムスタンプ。
/// </param>
public sealed record Ticker(
    string Symbol,
    decimal BestBid,
    decimal BestAsk,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
