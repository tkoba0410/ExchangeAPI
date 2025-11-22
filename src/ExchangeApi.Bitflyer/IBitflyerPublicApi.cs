using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer
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
    }
}
