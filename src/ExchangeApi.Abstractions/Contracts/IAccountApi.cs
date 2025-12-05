using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 現物口座情報（REST）の抽象インターフェース。
/// </summary>
public interface IAccountApi
{
    Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default);
}
