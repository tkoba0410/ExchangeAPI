using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<WireResponse> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        var meta = await _restClient.GetRawAsync("v1/account/accounts", cancellationToken: cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(accountId, nameof(accountId));
        var meta = await _restClient
            .GetRawAsync($"v1/account/accounts/{accountId}/balance", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        EnsureRequired(accountId, nameof(accountId));
        var query = BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("account-id", accountId));
        var meta = await _restClient
            .GetRawAsync($"v1/order/openOrders?{query}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        var meta = await _restClient
            .GetRawAsync($"v1/order/orders/{orderId.Value}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        var meta = await _restClient
            .GetRawAsync($"v1/order/orders/{orderId.Value}/matchresults", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOrdersAsync(
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
        var query = BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("states", states),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString()));

        var meta = await _restClient
            .GetRawAsync($"v1/order/orders?{query}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetMatchResultsAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            ("symbol", symbol.HasValue ? ToApiSymbol(symbol.Value) : null),
            ("types", types),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString()));

        var meta = await _restClient
            .GetRawAsync($"v1/order/matchresults?{query}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(type, nameof(type));
        var query = BuildQuery(
            ("type", type),
            ("currency", currency),
            ("from", from?.ToString()),
            ("size", size?.ToString()),
            ("direct", direct));

        var meta = await _restClient
            .GetRawAsync($"v1/query/deposit-withdraw?{query}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var startTimeMs = startTime?.ToUnixTimeMilliseconds();
        var endTimeMs = endTime?.ToUnixTimeMilliseconds();
        var query = BuildQuery(
            ("direct", direct.ToString()),
            ("status", status?.ToString()),
            ("start_time", startTimeMs?.ToString()),
            ("end_time", endTimeMs?.ToString()));

        var meta = await _restClient
            .GetRawAsync($"v1/retail/order/list?{query}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return ToWire(meta);
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

    private static string BuildQuery(params (string Key, string? Value)[] items)
    {
        var parts = items
            .Where(i => !string.IsNullOrWhiteSpace(i.Value))
            .Select(i => $"{i.Key}={Uri.EscapeDataString(i.Value!)}");
        return string.Join("&", parts);
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
}
