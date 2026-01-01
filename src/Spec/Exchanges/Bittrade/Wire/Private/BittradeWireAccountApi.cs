using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireAccountApi : IBittradeWireAccountApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly IRestClient _restClient;

    public BittradeWireAccountApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<WireCall> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        GetAsync("v1/account/accounts", query: null, cancellationToken);

    public Task<WireCall> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(accountId, nameof(accountId));
        return GetAsync($"v1/account/accounts/{accountId}/balance", query: null, cancellationToken);
    }

    public Task<WireCall> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        EnsureRequired(accountId, nameof(accountId));
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
            ["account-id"] = accountId,
        };
        return GetAsync("v1/order/openOrders", query, cancellationToken);
    }

    public Task<WireCall> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        return GetAsync($"v1/order/orders/{orderId.Value}", query: null, cancellationToken);
    }

    public Task<WireCall> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        return GetAsync($"v1/order/orders/{orderId.Value}/matchresults", query: null, cancellationToken);
    }

    public Task<WireCall> GetOrdersAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        EnsureRequired(states, nameof(states));
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
            ["states"] = states,
            ["start-date"] = startDate,
            ["end-date"] = endDate,
            ["from"] = from?.ToString(),
            ["direct"] = direct,
            ["size"] = size?.ToString(),
        };

        return GetAsync("v1/order/orders", query, cancellationToken);
    }

    public Task<WireCall> GetMatchResultsAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = symbol.HasValue ? ToApiSymbol(symbol.Value) : null,
            ["types"] = types,
            ["start-date"] = startDate,
            ["end-date"] = endDate,
            ["from"] = from?.ToString(),
            ["direct"] = direct,
            ["size"] = size?.ToString(),
        };

        return GetAsync("v1/order/matchresults", query, cancellationToken);
    }

    public Task<WireCall> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(type, nameof(type));
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["type"] = type,
            ["currency"] = currency,
            ["from"] = from?.ToString(),
            ["size"] = size?.ToString(),
            ["direct"] = direct,
        };

        return GetAsync("v1/query/deposit-withdraw", query, cancellationToken);
    }

    public Task<WireCall> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var startTimeMs = startTime?.ToUnixTimeMilliseconds();
        var endTimeMs = endTime?.ToUnixTimeMilliseconds();
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["direct"] = direct.ToString(),
            ["status"] = status?.ToString(),
            ["start_time"] = startTimeMs?.ToString(),
            ["end_time"] = endTimeMs?.ToString(),
        };

        return GetAsync("v1/retail/order/list", query, cancellationToken);
    }

    private static string ToApiSymbol(RawSymbol symbol) =>
        symbol.Value.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureRequired(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} is required.", name);
        }
    }

    private static void EnsureSymbol(RawSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }

    private async Task<WireCall> GetAsync(
        string path,
        IReadOnlyDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        var request = new WireRequest(
            Method: "GET",
            Path: path,
            Query: BuildQuery(query));
        var meta = await _restClient.GetRawAsync(path, query, cancellationToken: cancellationToken).ConfigureAwait(false);
        var response = ToWire(meta);
        return new WireCall(request, response, CreateMeta(response));
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
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            parts.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
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
}
