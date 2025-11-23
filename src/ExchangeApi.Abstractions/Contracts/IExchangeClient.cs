namespace ExchangeApi.Abstractions.Contracts;

public interface IExchangeClient
{
    /// <summary>
    /// 取引所を一意に識別する ID。
    /// 例: "bitflyer", "binance", "bybit" など。
    /// </summary>
    string ExchangeId { get; }

    /// <summary>
    /// 同一取引所内でのアカウント識別子。
    /// 例: "primary", "bot1" など。
    /// 単一アカウント運用の場合は "default" 等の固定値でもよい。
    /// </summary>
    string AccountId { get; }

    /// <summary>
    /// 指定シンボルのティッカー情報を取得する。
    /// Stage1 では主に BTC/JPY を対象とする。
    /// </summary>
    /// <param name="symbol">
    /// 共通シンボル表記。例: <c>Symbols.BtcJpy</c> (= "BTC/JPY")。
    /// </param>
    /// <param name="cancellationToken">キャンセル用トークン。</param>
    /// <returns>取得したティッカー。</returns>
    Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
