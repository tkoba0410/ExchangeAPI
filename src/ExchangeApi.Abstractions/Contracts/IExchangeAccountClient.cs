using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 口座情報の読み取り API を表すインターフェース。
/// Stage2 では残高取得のみをサポートする。
/// </summary>
public interface IExchangeAccountClient
{
    /// <summary>
    /// 口座残高一覧を取得する。
    /// </summary>
    Task<IReadOnlyList<Balance>> GetBalancesAsync(
        CancellationToken cancellationToken = default);
}
