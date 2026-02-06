using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.Errors;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.GetExchangeInfoResponse;
using ExchangeInfoResponse = ExchangeApi.Contracts.Common.Dtos.GetExchangeInfoResponse;
namespace ExchangeApi.Composition.Providers.ExchangeInfo;

/// <summary>
/// JSON ファイルから ExchangeInfo を読み込む IPublicApi 実装。
/// 複数ファイル指定時は後勝ちでマージする。
/// </summary>
public sealed class JsonExchangeInfoApi : IPublicApi
{
    private readonly string[] _paths;
    private readonly JsonSerializerOptions _options;
    private readonly TimeSpan _cacheTtl;
    private readonly object _sync = new();

    private ExchangeInfoDto? _cached;
    private DateTimeOffset _lastLoaded;
    private DateTime _latestWriteTimeUtc;

    public JsonExchangeInfoApi(IEnumerable<string> paths, TimeSpan? cacheTtl = null, JsonSerializerOptions? options = null)
    {
        if (paths is null) throw new ArgumentNullException(nameof(paths));
        _paths = paths.ToArray();
        if (_paths.Length == 0) throw new ArgumentException("At least one path is required.", nameof(paths));

        _cacheTtl = cacheTtl ?? TimeSpan.FromMinutes(5);
        _options = options ?? CreateDefaultOptions();
    }

    public async Task<Call<GetExchangeInfoRequest, ExchangeInfoResponse>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetExchangeInfoRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var info = GetCachedInfo();
            var response = info;
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: "JsonExchangeInfo",
                EndpointId: CallMeta.InternalEndpointId,
                Tags: null,
                Children: null);
            return new Call<GetExchangeInfoRequest, ExchangeInfoResponse>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: DateTimeOffset.UtcNow - startedAt,
                Request: request,
                Result: new CallResult<ExchangeInfoResponse>.Ok(response),
                Meta: meta);
        }
        catch (Exception ex)
        {
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: "JsonExchangeInfo",
                EndpointId: CallMeta.InternalEndpointId,
                Tags: null,
                Children: null);
            return new Call<GetExchangeInfoRequest, ExchangeInfoResponse>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: DateTimeOffset.UtcNow - startedAt,
                Request: request,
                Result: new CallResult<ExchangeInfoResponse>.Err(
                    new CallError(CallErrorKind.Unknown, ex.Message, ex)),
                Meta: meta);
        }
    }

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTickerRequest(symbol);
        return Task.FromResult(NotSupportedCall.Create<GetTickerRequest, GetTickerResponse>(
            "Contracts",
            "JsonExchangeInfo",
            request,
            "Ticker"));
    }

    public Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetBoardRequest(symbol);
        return Task.FromResult(NotSupportedCall.Create<GetBoardRequest, GetBoardResponse>(
            "Contracts",
            "JsonExchangeInfo",
            request,
            "OrderBook"));
    }

    public Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetExecutionsPublicRequest(symbol);
        return Task.FromResult(NotSupportedCall.Create<GetExecutionsPublicRequest, GetExecutionsPublicResponse>(
            "Contracts",
            "JsonExchangeInfo",
            request,
            "MarketExecutions"));
    }

    private ExchangeInfoDto GetCachedInfo()
    {
        lock (_sync)
        {
            if (_cached is { } cached && !IsStale())
            {
                return cached;
            }

            var info = LoadAndMerge();
            _cached = info;
            _lastLoaded = DateTimeOffset.UtcNow;
            _latestWriteTimeUtc = GetLatestWriteTimeUtc();
            return info;
        }
    }

    private ExchangeInfoDto LoadAndMerge()
    {
        ExchangeInfoDto? merged = null;
        foreach (var path in _paths)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"ExchangeInfo JSON not found: {path}", path);
            }

            var json = File.ReadAllText(path);
            var doc = JsonSerializer.Deserialize<ExchangeInfoDocument>(json, _options);
            if (doc is null)
            {
                throw new InvalidOperationException($"Failed to deserialize ExchangeInfo JSON: {path}");
            }

            var current = doc.ToExchangeInfo();
            merged = merged is null ? current : Merge(merged, current);
        }

        if (merged is null)
        {
            throw new InvalidOperationException("No ExchangeInfo data loaded.");
        }

        return merged;
    }

    private bool IsStale()
    {
        if (_cached is null) return true;
        if (DateTimeOffset.UtcNow - _lastLoaded > _cacheTtl) return true;
        return GetLatestWriteTimeUtc() > _latestWriteTimeUtc;
    }

    private DateTime GetLatestWriteTimeUtc() =>
        _paths.Select(File.GetLastWriteTimeUtc).Max();

    private static ExchangeInfoDto Merge(ExchangeInfoDto baseline, ExchangeInfoDto overlay)
    {
        var dict = baseline.Markets.ToDictionary(
            k => GetKey(k),
            v => v,
            StringComparer.Ordinal);

        foreach (var market in overlay.Markets)
        {
            dict[GetKey(market)] = market;
        }

        var markets = dict.Values.ToList();
        var features = overlay.Features ?? baseline.Features;
        var rateLimits = overlay.RateLimits ?? baseline.RateLimits;
        var maintenance = overlay.Maintenance ?? baseline.Maintenance;

        return new ExchangeInfoDto(markets, features, rateLimits, maintenance);
    }

    private static string GetKey(ExchangeMarketInfo market) =>
        market.ProductCode.IsEmpty ? market.Symbol.Value : market.ProductCode.Value;

    private static JsonSerializerOptions CreateDefaultOptions() =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new PriceJsonConverter(),
                new SizeJsonConverter(),
            }
        };

    private sealed record ExchangeInfoDocument(
        IReadOnlyList<ExchangeMarketInfo>? Markets,
        ExchangeFeatureFlags? Features,
        ExchangeRateLimits? RateLimits,
        ExchangeMaintenance? Maintenance,
        string? Version,
        DateTimeOffset? LastUpdated,
        string? Notes)
    {
        public ExchangeInfoDto ToExchangeInfo()
        {
            if (Markets is null || Markets.Count == 0)
            {
                throw new InvalidOperationException("ExchangeInfo JSON must contain at least one market.");
            }

            return new ExchangeInfoDto(Markets, Features, RateLimits, Maintenance);
        }
    }
}
