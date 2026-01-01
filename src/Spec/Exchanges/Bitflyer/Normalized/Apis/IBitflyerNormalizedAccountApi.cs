using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

public interface IBitflyerNormalizedAccountApi
{
    Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<JsonElement> GetTradingCommissionAsync(Symbol symbol, CancellationToken cancellationToken = default);
}
