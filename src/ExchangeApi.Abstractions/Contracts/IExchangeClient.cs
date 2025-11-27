using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 取引所共通インターフェース。
/// Stage1: Ticker、Stage2: 残高、Stage3: 発注（MARKET）、Stage4: 注文拡張/キャンセル/ポジション系。
/// </summary>
public interface IExchangeClient : IExchangeAccountClient, IExchangeTradingClient
{
    /// <summary>
    /// 取引所識別子。
    /// Stage1 では使用されないが、Stage2（複数取引所対応）に備えて保持する。
    /// </summary>
    string ExchangeId { get; }

    /// <summary>
    /// アカウント識別子。
    /// Stage1 では使用されないが、Stage2（複数アカウント対応）に備えて保持する。
    /// </summary>
    string AccountId { get; }

    /// <summary>
    /// 現在の Ticker を取得する（Stage1 の主要 API）。
    /// </summary>
    Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 板情報を取得する。
    /// </summary>
    Task<Board> GetBoardAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
