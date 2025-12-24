using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;

public interface IBitflyerWireMarketDataApi
{
    Task<Ticker> GetTickerRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<Board> GetBoardRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsRawAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<HealthResponse> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BoardStateResponse> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

    Task<FundingRateResponse> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}
