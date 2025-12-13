using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;

namespace Common.Contract.Contracts;

/// <summary>
/// マージン口座（REST）の抽象インターフェース。
/// </summary>
public interface IMarginAccountApi : IAccountApi
{
    Task<IReadOnlyList<Position>> GetOpenPositionsAsync(string productCode, CancellationToken cancellationToken = default);

    Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default);
}
