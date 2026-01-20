using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;

internal static class BittradeEndpoints
{
    public static WireCallSpec GetTicker(string symbol) =>
        Get(BittradeEndpointIds.GetDetailMerged, BittradeConstants.Paths.MarketMerged, BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetOrderBook(string symbol, string? type) =>
        Get(BittradeEndpointIds.GetDepth, BittradeConstants.Paths.MarketDepth, BuildQuery(
            ("symbol", symbol),
            ("type", type)));

    public static WireCallSpec GetTrades(string symbol) =>
        Get(BittradeEndpointIds.GetTrade, BittradeConstants.Paths.MarketTrade, BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetKlines(string symbol, string period, string? size = null) =>
        Get(BittradeEndpointIds.GetHistoryKline, BittradeConstants.Paths.MarketKline, BuildQuery(
            ("period", period),
            ("symbol", symbol),
            ("size", size)));

    public static WireCallSpec GetTickers() =>
        Get(BittradeEndpointIds.GetTickers, BittradeConstants.Paths.MarketTickers, query: null);

    public static WireCallSpec GetTradeHistory(string symbol) =>
        Get(BittradeEndpointIds.GetHistoryTrade, BittradeConstants.Paths.MarketHistoryTrade, BuildQuery(
            ("symbol", symbol)));

    public static WireCallSpec GetTimestamp() =>
        Get(BittradeEndpointIds.GetTimestamp, BittradeConstants.Paths.CommonTimestamp, query: null);

    public static WireCallSpec GetSymbols() =>
        Get(BittradeEndpointIds.GetSymbols, BittradeConstants.Paths.CommonSymbols, query: null);

    public static WireCallSpec GetCurrencies() =>
        Get(BittradeEndpointIds.GetCurrencys, BittradeConstants.Paths.CommonCurrencies, query: null);

    public static WireCallSpec GetRetailMaintainTime() =>
        Get(BittradeEndpointIds.GetMaintainTime, BittradeConstants.Paths.RetailMaintainTime, query: null);

    public static WireCallSpec GetAccounts() =>
        Get(BittradeEndpointIds.GetAccounts, BittradeConstants.Paths.Accounts, query: null);

    public static WireCallSpec GetAccountBalance(string accountId)
    {
        return Get(BittradeEndpointIds.GetAccountsBalanceByAccountId, $"{BittradeConstants.Paths.Accounts}/{accountId}/balance", query: null);
    }

    public static WireCallSpec GetOpenOrders(string symbol, string accountId)
    {
        return Get(BittradeEndpointIds.GetOpenOrders, BittradeConstants.Paths.OrdersOpen, BuildQuery(
            ("symbol", symbol),
            ("account-id", accountId)));
    }

    public static WireCallSpec GetOrder(string orderId) =>
        Get(BittradeEndpointIds.GetOrdersByOrderId, $"{BittradeConstants.Paths.Orders}/{orderId}", query: null);

    public static WireCallSpec GetOrderMatchResults(string orderId) =>
        Get(BittradeEndpointIds.GetOrdersMatchResultsByOrderId, $"{BittradeConstants.Paths.Orders}/{orderId}/matchresults", query: null);

    public static WireCallSpec GetMatchResults(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        string? from = null,
        string? direct = null,
        string? size = null)
    {
        return Get(BittradeEndpointIds.GetMatchResults, BittradeConstants.Paths.OrdersMatchResults, BuildQuery(
            ("symbol", symbol),
            ("types", types),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from),
            ("direct", direct),
            ("size", size)));
    }

    public static WireCallSpec GetDepositWithdraws(
        string type,
        string? currency = null,
        string? from = null,
        string? size = null,
        string? direct = null)
    {
        return Get(BittradeEndpointIds.GetDepositWithdraw, BittradeConstants.Paths.DepositWithdraw, BuildQuery(
            ("type", type),
            ("currency", currency),
            ("from", from),
            ("size", size),
            ("direct", direct)));
    }

    public static WireCallSpec GetRetailOrders(
        string direct,
        string? status = null,
        string? startTime = null,
        string? endTime = null)
    {
        return Get(BittradeEndpointIds.GetOrderList, BittradeConstants.Paths.RetailOrderList, BuildQuery(
            ("direct", direct),
            ("status", status),
            ("start_time", startTime),
            ("end_time", endTime)));
    }

    public static WireCallSpec PlaceOrder(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrdersPlace, BittradeConstants.Paths.OrdersPlace, bodyJson);

    public static WireCallSpec CancelOrder(string orderId) =>
        Post(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId, $"{BittradeConstants.Paths.Orders}/{orderId}/submitcancel", bodyJson: null);

    public static WireCallSpec CancelOrders(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrdersBatchCancel, BittradeConstants.Paths.OrdersBatchCancel, bodyJson);

    public static WireCallSpec CancelOpenOrders(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders, BittradeConstants.Paths.OrdersBatchCancelOpen, bodyJson);

    public static WireCallSpec CreateWithdraw(string bodyJson) =>
        Post(BittradeEndpointIds.PostWithdrawApiCreate, BittradeConstants.Paths.WithdrawCreate, bodyJson);

    public static WireCallSpec CancelWithdraw(string withdrawId) =>
        Post(BittradeEndpointIds.PostWithdrawVirtualCancelByWithdrawId, $"{BittradeConstants.Paths.WithdrawVirtual}/{withdrawId}/cancel", bodyJson: null);

    public static WireCallSpec CreateRetailOrder(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrderPlace, BittradeConstants.Paths.RetailOrderPlace, bodyJson);

    private static WireCallSpec Get(string endpointId, string path, string? query) =>
        new(Method: "GET", Path: path, EndpointId: endpointId, Query: query);

    private static WireCallSpec Post(string endpointId, string path, string? bodyJson) =>
        new(Method: "POST", Path: path, EndpointId: endpointId, Query: null, BodyJson: bodyJson);

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
