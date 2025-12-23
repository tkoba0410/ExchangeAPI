using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw 実装。
/// </summary>
internal sealed class BitflyerRawMarketDataApi : IBitflyerRawMarketDataApi
{
    private readonly IRestClient _restClient;

    public BitflyerRawMarketDataApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<Ticker> GetTickerAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var path = useAliasPath ? BitflyerRawConstants.Paths.Ticker : BitflyerRawConstants.Paths.GetTicker;

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [BitflyerRawConstants.QueryKeys.ProductCode] = productCode,
            };

        return _restClient.GetAsync<Ticker>(path, query, cancellationToken);
    }

    public Task<Board> GetBoardAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var path = useAliasPath ? BitflyerRawConstants.Paths.Board : BitflyerRawConstants.Paths.GetBoard;

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [BitflyerRawConstants.QueryKeys.ProductCode] = productCode,
            };

        return _restClient.GetAsync<Board>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("productCode is required.", nameof(productCode));

        var path = useAliasPath ? BitflyerRawConstants.Paths.Executions : BitflyerRawConstants.Paths.GetExecutions;

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [BitflyerRawConstants.QueryKeys.ProductCode] = productCode,
                [BitflyerRawConstants.QueryKeys.Count] = count?.ToString(),
                [BitflyerRawConstants.QueryKeys.Before] = before?.ToString(),
                [BitflyerRawConstants.QueryKeys.After] = after?.ToString(),
            };

        return _restClient.GetAsync<IReadOnlyList<ExecutionPublicResponse>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var path = useAliasPath ? BitflyerRawConstants.Paths.Markets : BitflyerRawConstants.Paths.GetMarkets;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return _restClient.GetAsync<IReadOnlyList<Market>>(path, query: null, cancellationToken);
    }

    public Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var path = BitflyerRawConstants.Paths.GetChats;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerRawConstants.QueryKeys.FromDate] = fromDate,
        };

        return _restClient.GetAsync<IReadOnlyList<Chat>>(path, query, cancellationToken);
    }

    public Task<HealthResponse> GetHealthAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerRawConstants.Paths.GetHealth;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerRawConstants.QueryKeys.ProductCode] = productCode,
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

        const string path = BitflyerRawConstants.Paths.GetBoardState;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerRawConstants.QueryKeys.ProductCode] = productCode,
        };

        return _restClient.GetAsync<BoardStateResponse>(path, query, cancellationToken);
    }

    public Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default)
    {
        const string path = BitflyerRawConstants.Paths.GetCorporateLeverage;
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

        const string path = BitflyerRawConstants.Paths.GetFundingRate;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerRawConstants.QueryKeys.ProductCode] = productCode,
        };

        return _restClient.GetAsync<FundingRateResponse>(path, query, cancellationToken);
    }
}
