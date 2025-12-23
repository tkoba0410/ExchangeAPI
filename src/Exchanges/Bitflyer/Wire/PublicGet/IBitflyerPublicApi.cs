using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Wire
{
    /// <summary>
    /// bitFlyer Public REST API への Wire アクセスインターフェース。
    /// </summary>
    internal interface IBitflyerPublicApi
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
        /// <param name="useAliasPath">true の場合 /v1/ticker を使用し、false の場合 /v1/getticker を使用。</param>
        /// <returns>bitFlyer の Ticker Raw レスポンス。</returns>
        Task<Ticker> GetTickerRawAsync(
            string productCode,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 板情報の Raw レスポンスを取得します。
        /// </summary>
        Task<Board> GetBoardRawAsync(
            string productCode,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 市場全体の約定履歴（歩み値）の Raw レスポンスを取得します。
        /// </summary>
        Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsRawAsync(
            string productCode,
            int? count = null,
            long? before = null,
            long? after = null,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 取扱い銘柄一覧（国別含む）を取得します。
        /// </summary>
        Task<IReadOnlyList<Market>> GetMarketsAsync(
            string? region = null,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// チャットログを取得します。
        /// </summary>
        Task<IReadOnlyList<Chat>> GetChatsAsync(
            string? fromDate = null,
            string? region = null,
            CancellationToken cancellationToken = default);

        Task<HealthResponse> GetHealthAsync(
            string productCode,
            CancellationToken cancellationToken = default);

        Task<BoardStateResponse> GetBoardStateAsync(
            string productCode,
            CancellationToken cancellationToken = default);

        Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

        Task<FundingRateResponse> GetFundingRateAsync(
            string productCode,
            CancellationToken cancellationToken = default);
    }
}
