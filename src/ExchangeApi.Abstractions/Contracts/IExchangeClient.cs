using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 取引所共通インターフェース。
/// Stage1 では Ticker 取得のみを対象とする。
/// 
/// ExchangeId / AccountId は Stage2 以降（Multi-Exchange / Multi-Account）で使用される
/// 拡張用プロパティであり、Stage1 では利用しない。
/// </summary>
public interface IExchangeClient : IExchangeAccountClient
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
}
