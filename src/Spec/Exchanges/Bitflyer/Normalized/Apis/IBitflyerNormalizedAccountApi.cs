using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

public interface IBitflyerNormalizedAccountApi
{
    Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<JsonElement> GetTradingCommissionAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<IReadOnlyList<Balance>, JsonElement>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<IReadOnlyList<ExecutionAccount>, JsonElement>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<JsonElement, JsonElement>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
