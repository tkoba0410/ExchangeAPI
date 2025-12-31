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

    public async Task<WireResponse> PlaceOrderAsync(RawCreateOrderRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var meta = await _restClient.PostRawAsync("v1/order/orders/place", request, ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelOrderAsync(string orderId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var meta = await _restClient
            .PostRawAsync<object?>($"v1/order/orders/{orderId}/submitcancel", body: null, ct)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var meta = await _restClient.PostRawAsync("v1/order/orders/batchcancel", request, ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var meta = await _restClient.PostRawAsync("v1/order/orders/batchCancelOpenOrders", request, ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var meta = await _restClient.PostRawAsync("v1/dw/withdraw/api/create", request, ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelWithdrawAsync(string withdrawId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        var meta = await _restClient
            .PostRawAsync<object?>($"v1/dw/withdraw-virtual/{withdrawId}/cancel", body: null, ct)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var meta = await _restClient.PostRawAsync("v1/retail/order/place", request, ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOpenOrdersAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var query = BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("account-id", _accountId));
        var meta = await _restClient
            .GetRawAsync($"v1/order/openOrders?{query}", cancellationToken: ct)
            .ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOrderAsync(string orderId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var meta = await _restClient.GetRawAsync($"v1/order/orders/{orderId}", cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
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
