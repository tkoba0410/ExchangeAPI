using System;
using System.Collections.Generic;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;

internal static class BittradeEndpoints
{
    public static WireCallSpec GetTicker(string symbol) =>
        Get("market/detail/merged", BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetOrderBook(string symbol, string? type) =>
        Get("market/depth", BuildQuery(
            ("symbol", symbol),
            ("type", type)));

    public static WireCallSpec GetTrades(string symbol) =>
        Get("market/trade", BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetKlines(string symbol, string period, int? size = null) =>
        Get("market/history/kline", BuildQuery(
            ("period", period),
            ("symbol", symbol),
            ("size", size?.ToString())));

    public static WireCallSpec GetTickers() => Get("market/tickers", query: null);

    public static WireCallSpec GetTradeHistory(string symbol) =>
        Get("market/history/trade", BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetTimestamp() => Get("v1/common/timestamp", query: null);

    public static WireCallSpec GetSymbols() => Get("v1/common/symbols", query: null);

    public static WireCallSpec GetCurrencies() => Get("v1/common/currencys", query: null);

    public static WireCallSpec GetRetailMaintainTime() => Get("v1/retail/maintain/time", query: null);

    public static WireCallSpec GetAccounts() => Get("v1/account/accounts", query: null);

    public static WireCallSpec GetAccountBalance(string accountId)
    {
        return Get($"v1/account/accounts/{accountId}/balance", query: null);
    }

    public static WireCallSpec GetOpenOrders(string symbol, string accountId)
    {
        return Get("v1/order/openOrders", BuildQuery(
            ("symbol", symbol),
            ("account-id", accountId)));
    }

    public static WireCallSpec GetOrder(string orderId) =>
        Get($"v1/order/orders/{orderId}", query: null);

    public static WireCallSpec GetOrderMatchResults(string orderId) =>
        Get($"v1/order/orders/{orderId}/matchresults", query: null);

    public static WireCallSpec GetOrders(
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

    public static WireCallSpec GetMatchResults(
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

    public static WireCallSpec GetDepositWithdraws(
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

    public static WireCallSpec GetRetailOrders(
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

    public static WireCallSpec PlaceOrder(string bodyJson) =>
        Post("v1/order/orders/place", bodyJson);

    public static WireCallSpec CancelOrder(string orderId) =>
        Post($"v1/order/orders/{orderId}/submitcancel", bodyJson: null);

    public static WireCallSpec CancelOrders(string bodyJson) =>
        Post("v1/order/orders/batchcancel", bodyJson);

    public static WireCallSpec CancelOpenOrders(string bodyJson) =>
        Post("v1/order/orders/batchCancelOpenOrders", bodyJson);

    public static WireCallSpec CreateWithdraw(string bodyJson) =>
        Post("v1/dw/withdraw/api/create", bodyJson);

    public static WireCallSpec CancelWithdraw(string withdrawId) =>
        Post($"v1/dw/withdraw-virtual/{withdrawId}/cancel", bodyJson: null);

    public static WireCallSpec CreateRetailOrder(string bodyJson) =>
        Post("v1/retail/order/place", bodyJson);

    private static WireCallSpec Get(string path, string? query) =>
        new(Method: "GET", Path: path, Query: query);

    private static WireCallSpec Post(string path, string? bodyJson) =>
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
