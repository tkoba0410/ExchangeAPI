using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
namespace ExchangeApi.Contracts.Interfaces;

/// <summary>
/// 現物口座情報（REST）の抽象インターフェース。
/// </summary>
public interface IAccountApi
{
    Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 口座の約定履歴を取得する。
    /// </summary>
    Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<ApiCall<GetBalancesRequest, IReadOnlyList<Balance>, ApiError>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default);

    Task<ApiCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>, ApiError>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
