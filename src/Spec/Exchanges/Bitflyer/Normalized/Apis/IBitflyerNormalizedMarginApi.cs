using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

public interface IBitflyerNormalizedMarginApi
{
    Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default);
}
