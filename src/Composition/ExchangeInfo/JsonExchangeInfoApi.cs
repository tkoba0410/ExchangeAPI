using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Dtos;
using Common.Enums;
using ExchangeInfoDto = Common.Dtos.ExchangeInfo;
namespace Composition.ExchangeInfo;

/// <summary>
/// JSON ファイルから ExchangeInfo を読み込む IExchangeInfoApi 実装。
/// 複数ファイル指定時は後勝ちでマージする。
/// </summary>
public sealed class JsonExchangeInfoApi : IExchangeInfoApi
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

    public Task<ExchangeInfoDto> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            if (_cached is { } cached && !IsStale())
            {
                return Task.FromResult(cached);
            }

            var info = LoadAndMerge();
            _cached = info;
            _lastLoaded = DateTimeOffset.UtcNow;
            _latestWriteTimeUtc = GetLatestWriteTimeUtc();
            return Task.FromResult(info);
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
        string.IsNullOrWhiteSpace(market.ProductCode) ? market.Symbol : market.ProductCode;

    private static JsonSerializerOptions CreateDefaultOptions() =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
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
