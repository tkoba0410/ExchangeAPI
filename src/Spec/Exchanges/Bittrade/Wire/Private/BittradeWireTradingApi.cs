using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireTradingApi : IBittradeWireTradingApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly IRestClient _restClient;
    private readonly string _accountId;

    public BittradeWireTradingApi(IRestClient restClient, string accountId)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public Task<WireCall> PlaceOrderAsync(RawCreateOrderRequest request, CancellationToken ct = default) =>
        PostAsync("v1/order/orders/place", request, ct);

    public Task<WireCall> CancelOrderAsync(string orderId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        return PostAsync<object?>($"v1/order/orders/{orderId}/submitcancel", body: null, ct);
    }

    public Task<WireCall> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken ct = default) =>
        PostAsync("v1/order/orders/batchcancel", request, ct);

    public Task<WireCall> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken ct = default) =>
        PostAsync("v1/order/orders/batchCancelOpenOrders", request, ct);

    public Task<WireCall> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken ct = default) =>
        PostAsync("v1/dw/withdraw/api/create", request, ct);

    public Task<WireCall> CancelWithdrawAsync(string withdrawId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        return PostAsync<object?>($"v1/dw/withdraw-virtual/{withdrawId}/cancel", body: null, ct);
    }

    public Task<WireCall> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken ct = default) =>
        PostAsync("v1/retail/order/place", request, ct);

    public Task<WireCall> GetOpenOrdersAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = "v1/order/openOrders";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["symbol"] = ToApiSymbol(symbol),
            ["account-id"] = _accountId,
        };
        return GetAsync(path, query, ct);
    }

    public async Task<WireCall> GetOrderAsync(string orderId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        return await GetAsync($"v1/order/orders/{orderId}", query: null, ct).ConfigureAwait(false);
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

    private async Task<WireCall> PostAsync<TRequest>(
        string path,
        TRequest? body,
        CancellationToken ct)
    {
        var bodyJson = body is null ? null : JsonSerializer.Serialize(body);
        var request = new WireRequest(
            Method: "POST",
            Path: path,
            Query: null,
            BodyJson: bodyJson);
        var meta = await _restClient.PostRawAsync(path, body, ct).ConfigureAwait(false);
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
}
