using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;

internal static class BittradeEndpoints
{
    public static WireRequest GetTicker(string symbol) =>
        Get("market/detail/merged", BuildQuery(
            ("symbol", ToApiSymbol(symbol))));

    public static WireRequest GetOrderBook(string symbol, string? type) =>
        Get("market/depth", BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("type", string.IsNullOrWhiteSpace(type) ? "step0" : type)));

    public static WireRequest GetTrades(string symbol) =>
        Get("market/trade", BuildQuery(
            ("symbol", ToApiSymbol(symbol))));

    public static WireRequest GetKlines(string symbol, string period, int? size = null)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new ArgumentException("period is required.", nameof(period));
        }

        return Get("market/history/kline", BuildQuery(
            ("period", period),
            ("symbol", ToApiSymbol(symbol)),
            ("size", size?.ToString())));
    }

    public static WireRequest GetTickers() => Get("market/tickers", query: null);

    public static WireRequest GetTradeHistory(string symbol) =>
        Get("market/history/trade", BuildQuery(
            ("symbol", ToApiSymbol(symbol))));

    public static WireRequest GetTimestamp() => Get("v1/common/timestamp", query: null);

    public static WireRequest GetSymbols() => Get("v1/common/symbols", query: null);

    public static WireRequest GetCurrencies() => Get("v1/common/currencys", query: null);

    public static WireRequest GetRetailMaintainTime() => Get("v1/retail/maintain/time", query: null);

    public static WireRequest GetAccounts() => Get("v1/account/accounts", query: null);

    public static WireRequest GetAccountBalance(string accountId)
    {
        EnsureRequired(accountId, nameof(accountId));
        return Get($"v1/account/accounts/{accountId}/balance", query: null);
    }

    public static WireRequest GetOpenOrders(RawSymbol symbol, string accountId)
    {
        EnsureSymbol(symbol);
        EnsureRequired(accountId, nameof(accountId));
        return Get("v1/order/openOrders", BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("account-id", accountId)));
    }

    public static WireRequest GetOrder(RawOrderId orderId)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        return Get($"v1/order/orders/{orderId.Value}", query: null);
    }

    public static WireRequest GetOrderMatchResults(RawOrderId orderId)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        return Get($"v1/order/orders/{orderId.Value}/matchresults", query: null);
    }

    public static WireRequest GetOrders(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null)
    {
        EnsureSymbol(symbol);
        EnsureRequired(states, nameof(states));
        return Get("v1/order/orders", BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("states", states),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString())));
    }

    public static WireRequest GetMatchResults(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null)
    {
        return Get("v1/order/matchresults", BuildQuery(
            ("symbol", symbol.HasValue ? ToApiSymbol(symbol.Value) : null),
            ("types", types),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString())));
    }

    public static WireRequest GetDepositWithdraws(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null)
    {
        EnsureRequired(type, nameof(type));
        return Get("v1/query/deposit-withdraw", BuildQuery(
            ("type", type),
            ("currency", currency),
            ("from", from?.ToString()),
            ("size", size?.ToString()),
            ("direct", direct)));
    }

    public static WireRequest GetRetailOrders(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null)
    {
        var startMs = startTime?.ToUnixTimeMilliseconds();
        var endMs = endTime?.ToUnixTimeMilliseconds();
        return Get("v1/retail/order/list", BuildQuery(
            ("direct", direct.ToString()),
            ("status", status?.ToString()),
            ("start_time", startMs?.ToString()),
            ("end_time", endMs?.ToString())));
    }

    public static WireRequest PlaceOrder(string bodyJson) =>
        Post("v1/order/orders/place", bodyJson);

    public static WireRequest CancelOrder(string orderId)
    {
        EnsureRequired(orderId, nameof(orderId));
        return Post($"v1/order/orders/{orderId}/submitcancel", bodyJson: null);
    }

    public static WireRequest CancelOrders(string bodyJson) =>
        Post("v1/order/orders/batchcancel", bodyJson);

    public static WireRequest CancelOpenOrders(string bodyJson) =>
        Post("v1/order/orders/batchCancelOpenOrders", bodyJson);

    public static WireRequest CreateWithdraw(string bodyJson) =>
        Post("v1/dw/withdraw/api/create", bodyJson);

    public static WireRequest CancelWithdraw(string withdrawId)
    {
        EnsureRequired(withdrawId, nameof(withdrawId));
        return Post($"v1/dw/withdraw-virtual/{withdrawId}/cancel", bodyJson: null);
    }

    public static WireRequest CreateRetailOrder(string bodyJson) =>
        Post("v1/retail/order/place", bodyJson);

    private static WireRequest Get(string path, string? query) =>
        new(Method: "GET", Path: path, Query: query);

    private static WireRequest Post(string path, string? bodyJson) =>
        new(Method: "POST", Path: path, Query: null, BodyJson: bodyJson);

    private static string? BuildQuery(params (string Key, string? Value)[] entries)
    {
        var parts = new List<string>();
        foreach (var (key, value) in entries)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            parts.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
    }

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

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static string ToApiSymbol(RawSymbol symbol) => ToApiSymbol(symbol.Value);
}
