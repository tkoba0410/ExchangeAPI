using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;

internal static class BittradeEndpoints
{
    public static WireCallSpec GetTicker(string symbol) =>
        Get(BittradeConstants.Paths.MarketMerged, BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetOrderBook(string symbol, string? type) =>
        Get(BittradeConstants.Paths.MarketDepth, BuildQuery(
            ("symbol", symbol),
            ("type", type)));

    public static WireCallSpec GetTrades(string symbol) =>
        Get(BittradeConstants.Paths.MarketTrade, BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetKlines(string symbol, string period, int? size = null) =>
        Get(BittradeConstants.Paths.MarketKline, BuildQuery(
            ("period", period),
            ("symbol", symbol),
            ("size", size?.ToString())));

    public static WireCallSpec GetTickers() => Get(BittradeConstants.Paths.MarketTickers, query: null);

    public static WireCallSpec GetTradeHistory(string symbol) =>
        Get(BittradeConstants.Paths.MarketHistoryTrade, BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetTimestamp() => Get(BittradeConstants.Paths.CommonTimestamp, query: null);

    public static WireCallSpec GetSymbols() => Get(BittradeConstants.Paths.CommonSymbols, query: null);

    public static WireCallSpec GetCurrencies() => Get(BittradeConstants.Paths.CommonCurrencies, query: null);

    public static WireCallSpec GetRetailMaintainTime() => Get(BittradeConstants.Paths.RetailMaintainTime, query: null);

    public static WireCallSpec GetAccounts() => Get(BittradeConstants.Paths.Accounts, query: null);

    public static WireCallSpec GetAccountBalance(string accountId)
    {
        return Get($"{BittradeConstants.Paths.Accounts}/{accountId}/balance", query: null);
    }

    public static WireCallSpec GetOpenOrders(string symbol, string accountId)
    {
        return Get(BittradeConstants.Paths.OrdersOpen, BuildQuery(
            ("symbol", symbol),
            ("account-id", accountId)));
    }

    public static WireCallSpec GetOrder(string orderId) =>
        Get($"{BittradeConstants.Paths.Orders}/{orderId}", query: null);

    public static WireCallSpec GetOrderMatchResults(string orderId) =>
        Get($"{BittradeConstants.Paths.Orders}/{orderId}/matchresults", query: null);

    public static WireCallSpec GetMatchResults(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null)
    {
        return Get(BittradeConstants.Paths.OrdersMatchResults, BuildQuery(
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
        return Get(BittradeConstants.Paths.DepositWithdraw, BuildQuery(
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
        return Get(BittradeConstants.Paths.RetailOrderList, BuildQuery(
            ("direct", direct.ToString()),
            ("status", status?.ToString()),
            ("start_time", startMs?.ToString()),
            ("end_time", endMs?.ToString())));
    }

    public static WireCallSpec PlaceOrder(string bodyJson) =>
        Post(BittradeConstants.Paths.OrdersPlace, bodyJson);

    public static WireCallSpec CancelOrder(string orderId) =>
        Post($"{BittradeConstants.Paths.Orders}/{orderId}/submitcancel", bodyJson: null);

    public static WireCallSpec CancelOrders(string bodyJson) =>
        Post(BittradeConstants.Paths.OrdersBatchCancel, bodyJson);

    public static WireCallSpec CancelOpenOrders(string bodyJson) =>
        Post(BittradeConstants.Paths.OrdersBatchCancelOpen, bodyJson);

    public static WireCallSpec CreateWithdraw(string bodyJson) =>
        Post(BittradeConstants.Paths.WithdrawCreate, bodyJson);

    public static WireCallSpec CancelWithdraw(string withdrawId) =>
        Post($"{BittradeConstants.Paths.WithdrawVirtual}/{withdrawId}/cancel", bodyJson: null);

    public static WireCallSpec CreateRetailOrder(string bodyJson) =>
        Post(BittradeConstants.Paths.RetailOrderPlace, bodyJson);

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
