using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

internal interface IBitflyerWireMarketDataApi
{
    Task<WireResponse<Ticker>> GetTickerRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse<Board>> GetBoardRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse<IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsRawAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse<HealthResponse>> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireResponse<BoardStateResponse>> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireResponse<CorporateLeverageResponse>> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

    Task<WireResponse<FundingRateResponse>> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}
