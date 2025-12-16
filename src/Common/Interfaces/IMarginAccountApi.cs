using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Enums;
namespace Common.Interfaces;

/// <summary>
/// マージン口座（REST）の抽象インターフェース。
/// </summary>
public interface IMarginAccountApi : IAccountApi
{
    Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default);
}
