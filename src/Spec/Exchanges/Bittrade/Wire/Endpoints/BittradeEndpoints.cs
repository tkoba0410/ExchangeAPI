using System;
using System.Collections.Generic;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;

internal static class BittradeEndpoints
{
    public static WireRequest GetTicker(string symbol) =>
        Get("market/detail/merged", BuildQuery(
            ("symbol", symbol)));

    public static WireRequest GetOrderBook(string symbol, string? type) =>
        Get("market/depth", BuildQuery(
            ("symbol", symbol),
            ("type", type)));

    public static WireRequest GetTrades(string symbol) =>
        Get("market/trade", BuildQuery(
            ("symbol", symbol)));

    public static WireRequest GetKlines(string symbol, string period, int? size = null) =>
        Get("market/history/kline", BuildQuery(
            ("period", period),
            ("symbol", symbol),
            ("size", size?.ToString())));

    public static WireRequest GetTickers() => Get("market/tickers", query: null);

    public static WireRequest GetTradeHistory(string symbol) =>
        Get("market/history/trade", BuildQuery(
            ("symbol", symbol)));

    public static WireRequest GetTimestamp() => Get("v1/common/timestamp", query: null);

    public static WireRequest GetSymbols() => Get("v1/common/symbols", query: null);

    public static WireRequest GetCurrencies() => Get("v1/common/currencys", query: null);

    public static WireRequest GetRetailMaintainTime() => Get("v1/retail/maintain/time", query: null);

    public static WireRequest GetAccounts() => Get("v1/account/accounts", query: null);

    public static WireRequest GetAccountBalance(string accountId)
    {
        return Get($"v1/account/accounts/{accountId}/balance", query: null);
    }

    public static WireRequest GetOpenOrders(string symbol, string accountId)
    {
        return Get("v1/order/openOrders", BuildQuery(
            ("symbol", symbol),
            ("account-id", accountId)));
    }

    public static WireRequest GetOrder(string orderId) =>
        Get($"v1/order/orders/{orderId}", query: null);

    public static WireRequest GetOrderMatchResults(string orderId) =>
        Get($"v1/order/orders/{orderId}/matchresults", query: null);

    public static WireRequest GetOrders(
        string symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null)
    {
        return Get("v1/order/orders", BuildQuery(
            ("symbol", symbol),
            ("states", states),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString())));
    }

    public static WireRequest GetMatchResults(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null)
    {
        return Get("v1/order/matchresults", BuildQuery(
            ("symbol", symbol),
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

    public static WireRequest CancelOrder(string orderId) =>
        Post($"v1/order/orders/{orderId}/submitcancel", bodyJson: null);

    public static WireRequest CancelOrders(string bodyJson) =>
        Post("v1/order/orders/batchcancel", bodyJson);

    public static WireRequest CancelOpenOrders(string bodyJson) =>
        Post("v1/order/orders/batchCancelOpenOrders", bodyJson);

    public static WireRequest CreateWithdraw(string bodyJson) =>
        Post("v1/dw/withdraw/api/create", bodyJson);

    public static WireRequest CancelWithdraw(string withdrawId) =>
        Post($"v1/dw/withdraw-virtual/{withdrawId}/cancel", bodyJson: null);

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

}
