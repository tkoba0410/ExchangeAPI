using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer 公開 REST API の実装。
/// Transport/Protocol 層に依存し、取引所固有のロジックのみを提供する。
/// </summary>
internal sealed class BitflyerPublicApi : IBitflyerPublicApi
{
    private readonly IRestClient _restClient;

    /// <summary>
    /// bitFlyer API のベース URL。
    /// </summary>
    private static readonly Uri BaseUri = new("https://api.bitflyer.com/");

    public BitflyerPublicApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    /// <summary>
    /// 生の ticker JSON を取得し、<see cref="Ticker"/> にデシリアライズして返す。
    /// </summary>
    public Task<Ticker> GetTickerRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var path = useAliasPath ? BitflyerConstants.Paths.Ticker : BitflyerConstants.Paths.GetTicker;

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [BitflyerConstants.QueryKeys.ProductCode] = productCode,
            };

        return _restClient.GetAsync<Ticker>(
            path,
            query,
            cancellationToken);
    }

    /// <summary>
    /// 生の板情報 JSON を取得し、<see cref="Board"/> にデシリアライズして返す。
    /// </summary>
    public Task<Board> GetBoardRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var path = useAliasPath ? BitflyerConstants.Paths.Board : BitflyerConstants.Paths.GetBoard;

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [BitflyerConstants.QueryKeys.ProductCode] = productCode,
            };

        return _restClient.GetAsync<Board>(
            path,
            query,
            cancellationToken);
    }

    /// <summary>
    /// 生の約定履歴（市場全体の歩み値）を取得する。
    /// </summary>
    public Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsRawAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("productCode is required.", nameof(productCode));

        var path = useAliasPath ? BitflyerConstants.Paths.Executions : BitflyerConstants.Paths.GetExecutions;

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [BitflyerConstants.QueryKeys.ProductCode] = productCode,
                [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
                [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
                [BitflyerConstants.QueryKeys.After] = after?.ToString(),
            };

        return _restClient.GetAsync<IReadOnlyList<ExecutionPublicResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var path = useAliasPath ? BitflyerConstants.Paths.Markets : BitflyerConstants.Paths.GetMarkets;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return _restClient.GetAsync<IReadOnlyList<Market>>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var path = BitflyerConstants.Paths.GetChats;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.FromDate] = fromDate,
        };

        return _restClient.GetAsync<IReadOnlyList<Chat>>(
            path,
            query,
            cancellationToken);
    }

    public Task<HealthResponse> GetHealthAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetHealth;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
        };

        return _restClient.GetAsync<HealthResponse>(path, query, cancellationToken);
    }

    public Task<BoardStateResponse> GetBoardStateAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetBoardState;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
        };

        return _restClient.GetAsync<BoardStateResponse>(path, query, cancellationToken);
    }

    public Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetCorporateLeverage;
        return _restClient.GetAsync<CorporateLeverageResponse>(path, query: null, cancellationToken);
    }

    public Task<FundingRateResponse> GetFundingRateAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetFundingRate;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["product_code"] = productCode,
        };
        return _restClient.GetAsync<FundingRateResponse>(path, query, cancellationToken);
    }
}
