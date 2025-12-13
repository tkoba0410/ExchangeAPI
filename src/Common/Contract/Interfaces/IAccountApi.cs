using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Common.Contract.Interfaces;

/// <summary>
/// 現物口座情報（REST）の抽象インターフェース。
/// </summary>
public interface IAccountApi
{
    Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 口座の約定履歴を取得する。
    /// </summary>
    Task<IReadOnlyList<AccountExecution>> GetAccountExecutionsAsync(string productCode, CancellationToken cancellationToken = default);
}
