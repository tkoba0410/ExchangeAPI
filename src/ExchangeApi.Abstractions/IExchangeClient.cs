using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Models;

namespace ExchangeApi.Abstractions
{
    public interface IExchangeClient
    {
        /// <summary>
        /// 現在価格情報（Ticker）を取得します。
        /// </summary>
        /// <param name="symbol">
        /// "BASE/QUOTE" 形式のシンボル（例: "BTC/JPY"）。
        /// </param>
        /// <param name="cancellationToken">
        /// キャンセル トークン。
        /// </param>
        /// <returns>Ticker 情報。</returns>
        /// <exception cref="ArgumentException">
        /// symbol の形式が不正、または未対応のシンボルの場合。
        /// </exception>
        Task<Ticker> GetTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default);
    }
}
