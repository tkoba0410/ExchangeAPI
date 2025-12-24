using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw インターフェース。
/// </summary>
public interface IBitflyerRawMarketDataApi
{
    Task<Ticker> GetTickerAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<Board> GetBoardAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
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
