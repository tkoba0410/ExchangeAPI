using ExchangeApi.Adapter.Bitflyer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Adapter.Bitflyer
{
    /// <summary>
    /// bitFlyer Public REST API (GET /v1/getticker) への Raw アクセスインターフェース。
    /// </summary>
    public interface IBitflyerPublicApi
    {
        /// <summary>
        /// Ticker 情報の Raw レスポンスを取得します。
        /// </summary>
        /// <param name="productCode">
        /// 取引所の product_code（例: "BTC_JPY"）。
        /// </param>
        /// <param name="cancellationToken">
        /// キャンセル トークン。
        /// </param>
        /// <returns>bitFlyer の Ticker Raw レスポンス。</returns>
        Task<BitflyerTickerRaw> GetTickerRawAsync(
            string productCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 板情報の Raw レスポンスを取得します。
        /// </summary>
        Task<BitflyerBoardRaw> GetBoardRawAsync(
            string productCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 市場全体の約定履歴（歩み値）の Raw レスポンスを取得します。
        /// </summary>
        Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsRawAsync(
            string productCode,
            CancellationToken cancellationToken = default);
    }
}
