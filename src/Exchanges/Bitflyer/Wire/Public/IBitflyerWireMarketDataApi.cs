using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Public;

public interface IBitflyerWireMarketDataApi
{
    Task<Ticker> GetTickerRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<Board> GetBoardRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsRawAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<HealthResponse> GetHealthAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<BoardStateResponse> GetBoardStateAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

    Task<FundingRateResponse> GetFundingRateAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
