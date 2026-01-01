using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
namespace ExchangeApi.Contracts.Interfaces;

/// <summary>
/// マージン口座（REST）の抽象インターフェース。
/// </summary>
public interface IMarginAccountApi : IAccountApi
{
    Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default);

    Task<ApiCall<GetOpenPositionsRequest, IReadOnlyList<Position>, ApiError>> GetOpenPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<ApiCall<GetCollateralRequest, Collateral, ApiError>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);
}
