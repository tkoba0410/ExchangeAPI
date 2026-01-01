using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireMarketDataApi : IBittradeWireMarketDataApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly IRestClient _restClient;

    public BittradeWireMarketDataApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<WireCall> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = "market/detail/merged";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
        };
        return await GetAsync(path, query, ct).ConfigureAwait(false);
    }

    public async Task<WireCall> GetOrderBookAsync(string symbol, string? type = null, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var depthType = string.IsNullOrWhiteSpace(type) ? "step0" : type;
        var path = "market/depth";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
            ["type"] = depthType,
        };
        return await GetAsync(path, query, ct).ConfigureAwait(false);
    }

    public async Task<WireCall> GetTradesAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = "market/trade";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
        };
        return await GetAsync(path, query, ct).ConfigureAwait(false);
    }

    public async Task<WireCall> GetKlinesAsync(string symbol, string period, int? size = null, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new ArgumentException("period is required.", nameof(period));
        }

        var path = "market/history/kline";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["period"] = period,
            ["symbol"] = ToApiSymbol(symbol),
        };
        if (size.HasValue)
        {
            query["size"] = size.Value.ToString();
        }

        return await GetAsync(path, query, ct).ConfigureAwait(false);
    }

    public Task<WireCall> GetTickersAsync(CancellationToken ct = default) =>
        GetAsync("market/tickers", query: null, ct);

    public async Task<WireCall> GetTradeHistoryAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = "market/history/trade";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
        };
        return await GetAsync(path, query, ct).ConfigureAwait(false);
    }

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }

    private async Task<WireCall> GetAsync(
        string path,
        IReadOnlyDictionary<string, string?>? query,
        CancellationToken ct)
    {
        var request = new WireRequest(
            Method: "GET",
            Path: path,
            Query: BuildQuery(query));
        var meta = await _restClient.GetRawAsync(path, query, cancellationToken: ct).ConfigureAwait(false);
        var response = ToWire(meta);
        return new WireCall(request, response, CreateMeta(response));
    }

    private static WireResponse ToWire(HttpResponseMeta meta)
    {
        var headers = meta.Headers is null
            ? null
            : new Dictionary<string, string>(meta.Headers, StringComparer.OrdinalIgnoreCase);
        return new WireResponse(
            Exchange,
            meta.StatusCode,
            meta.Body ?? string.Empty,
            headers);
    }

    private static CallMeta CreateMeta(WireResponse response)
    {
        var elapsed = response.ElapsedMs is { } ms ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;
        var startedAt = DateTimeOffset.UtcNow - elapsed;
        return new CallMeta(startedAt, elapsed, response.RequestId);
    }

    private static string? BuildQuery(IReadOnlyDictionary<string, string?>? query)
    {
        if (query is null || query.Count == 0)
        {
            return null;
        }

        var parts = new List<string>();
        foreach (var (key, value) in query)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
    }
}
