using System.Collections.Generic;
using System.Text.Json;
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

    Task<BitflyerRawCall<Ticker, JsonElement>> GetTickerCallAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<Board> GetBoardAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<Board, JsonElement>> GetBoardCallAsync(
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

    Task<BitflyerRawCall<IReadOnlyList<ExecutionPublicResponse>, JsonElement>> GetExecutionsCallAsync(
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

    Task<BitflyerRawCall<IReadOnlyList<Market>, JsonElement>> GetMarketsCallAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<IReadOnlyList<Chat>, JsonElement>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default);

    Task<HealthResponse> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<HealthResponse, JsonElement>> GetHealthCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BoardStateResponse> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<BoardStateResponse, JsonElement>> GetBoardStateCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<CorporateLeverageResponse, JsonElement>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default);

    Task<FundingRateResponse> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<BitflyerRawCall<FundingRateResponse, JsonElement>> GetFundingRateCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}
