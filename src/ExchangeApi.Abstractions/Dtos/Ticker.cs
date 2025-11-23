namespace ExchangeApi.Abstractions.Dtos;

/// <summary>
/// 取引所共通フォーマットのティッカー情報。
/// </summary>
/// <param name="Symbol">シンボル (例: "BTC/JPY")。</param>
/// <param name="BestBidPrice">最良買い気配価格。</param>
/// <param name="BestAskPrice">最良売り気配価格。</param>
/// <param name="LastTradedPrice">
/// 直近約定価格。取引所のレスポンスに無い場合は null。
/// </param>
/// <param name="Volume">
/// 24時間出来高などの代表的なボリューム情報。取引所の仕様により null の場合がある。
/// </param>
/// <param name="Timestamp">
/// ティッカーの発生時刻（UTC 推奨）。取引所のタイムスタンプを正規化したもの。
/// </param>
public sealed record Ticker(
    string Symbol,
    decimal BestBidPrice,
    decimal BestAskPrice,
    decimal? LastTradedPrice,
    decimal? Volume,
    DateTime Timestamp);
