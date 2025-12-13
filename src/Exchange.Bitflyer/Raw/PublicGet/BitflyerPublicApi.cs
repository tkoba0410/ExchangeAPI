using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer 公開 REST API の実装。
/// Transport/Protocol 層に依存し、取引所固有のロジックのみを提供する。
/// </summary>
public sealed class BitflyerPublicApi : IBitflyerPublicApi
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
    /// 生の ticker JSON を取得し、<see cref="BitflyerTickerRaw"/> にデシリアライズして返す。
    /// </summary>
    public Task<BitflyerTickerRaw> GetTickerRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var path = useAliasPath ? "/v1/ticker" : "/v1/getticker";

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["product_code"] = productCode,
            };

        return _restClient.GetAsync<BitflyerTickerRaw>(
            path,
            query,
            cancellationToken);
    }

    /// <summary>
    /// 生の板情報 JSON を取得し、<see cref="BitflyerBoardRaw"/> にデシリアライズして返す。
    /// </summary>
    public Task<BitflyerBoardRaw> GetBoardRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var path = useAliasPath ? "/v1/board" : "/v1/getboard";

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["product_code"] = productCode,
            };

        return _restClient.GetAsync<BitflyerBoardRaw>(
            path,
            query,
            cancellationToken);
    }

    /// <summary>
    /// 生の約定履歴（市場全体の歩み値）を取得する。
    /// </summary>
    public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsRawAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("productCode is required.", nameof(productCode));

        var path = useAliasPath ? "/v1/executions" : "/v1/getexecutions";

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["product_code"] = productCode,
                ["count"] = count?.ToString(),
                ["before"] = before?.ToString(),
                ["after"] = after?.ToString(),
            };

        return _restClient.GetAsync<IReadOnlyList<BitflyerExecutionResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerMarket>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var path = useAliasPath ? "/v1/markets" : "/v1/getmarkets";
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return _restClient.GetAsync<IReadOnlyList<BitflyerMarket>>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerChat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var path = "/v1/getchats";
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["from_date"] = fromDate,
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerChat>>(
            path,
            query,
            cancellationToken);
    }

    public Task<BitflyerHealthResponse> GetHealthAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = "/v1/gethealth";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["product_code"] = productCode,
        };

        return _restClient.GetAsync<BitflyerHealthResponse>(path, query, cancellationToken);
    }

    public Task<BitflyerBoardStateResponse> GetBoardStateAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = "/v1/getboardstate";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["product_code"] = productCode,
        };

        return _restClient.GetAsync<BitflyerBoardStateResponse>(path, query, cancellationToken);
    }

    public Task<JsonElement> GetCorporateLeverageAsync(CancellationToken cancellationToken = default)
    {
        const string path = "/v1/getcorporateleverage";
        return _restClient.GetAsync<JsonElement>(path, query: null, cancellationToken);
    }

    public Task<BitflyerFundingRateResponse> GetFundingRateAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = "/v1/getfundingrate";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["product_code"] = productCode,
        };
        return _restClient.GetAsync<BitflyerFundingRateResponse>(path, query, cancellationToken);
    }
}
