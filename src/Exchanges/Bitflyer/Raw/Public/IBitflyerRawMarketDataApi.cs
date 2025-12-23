using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw インターフェース。
/// </summary>
public interface IBitflyerRawMarketDataApi
{
    Task<Ticker> GetTickerAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<Board> GetBoardAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsAsync(
        string productCode,
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
