using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;

internal static class BittradeEndpoints
{
    public static WireCallSpec GetTicker(string symbol) =>
        Get(BittradeEndpointIds.GetDetailMerged, BittradePaths.MarketMergedPath, BuildQuery(
            (BittradeQueryKeys.Symbol, symbol)));

    public static WireCallSpec GetOrderBook(string symbol, string? type) =>
        Get(BittradeEndpointIds.GetDepth, BittradePaths.MarketDepthPath, BuildQuery(
            (BittradeQueryKeys.Symbol, symbol),
            (BittradeQueryKeys.Type, type)));

    public static WireCallSpec GetTrades(string symbol) =>
        Get(BittradeEndpointIds.GetTrade, BittradePaths.MarketTradePath, BuildQuery(
            (BittradeQueryKeys.Symbol, symbol)));

    public static WireCallSpec GetKlines(string symbol, string period, string? size = null) =>
        Get(BittradeEndpointIds.GetHistoryKline, BittradePaths.MarketKlinePath, BuildQuery(
            (BittradeQueryKeys.Period, period),
            (BittradeQueryKeys.Symbol, symbol),
            (BittradeQueryKeys.Size, size)));

    public static WireCallSpec GetTickers() =>
        Get(BittradeEndpointIds.GetTickers, BittradePaths.MarketTickersPath, query: null);

    public static WireCallSpec GetTradeHistory(string symbol) =>
        Get(BittradeEndpointIds.GetHistoryTrade, BittradePaths.MarketHistoryTradePath, BuildQuery(
            (BittradeQueryKeys.Symbol, symbol)));

    public static WireCallSpec GetTimestamp() =>
        Get(BittradeEndpointIds.GetTimestamp, BittradePaths.CommonTimestampPath, query: null);

    public static WireCallSpec GetSymbols() =>
        Get(BittradeEndpointIds.GetSymbols, BittradePaths.CommonSymbolsPath, query: null);

    public static WireCallSpec GetCurrencies() =>
        Get(BittradeEndpointIds.GetCurrencys, BittradePaths.CommonCurrenciesPath, query: null);

    public static WireCallSpec GetRetailMaintainTime() =>
        Get(BittradeEndpointIds.GetMaintainTime, BittradePaths.RetailMaintainTimePath, query: null);

    public static WireCallSpec GetAccounts() =>
        Get(BittradeEndpointIds.GetAccounts, BittradePaths.AccountsPath, query: null);

    public static WireCallSpec GetAccountBalance(string accountId)
    {
        return Get(BittradeEndpointIds.GetAccountsBalanceByAccountId, $"{BittradePaths.AccountsPath}/{accountId}/balance", query: null);
    }

    public static WireCallSpec GetOpenOrders(string symbol, string accountId)
    {
        return Get(BittradeEndpointIds.GetOpenOrders, BittradePaths.OrdersOpenPath, BuildQuery(
            (BittradeQueryKeys.Symbol, symbol),
            (BittradeQueryKeys.AccountId, accountId)));
    }

    public static WireCallSpec GetOrder(string orderId) =>
        Get(BittradeEndpointIds.GetOrdersByOrderId, $"{BittradePaths.OrdersPath}/{orderId}", query: null);

    public static WireCallSpec GetOrderMatchResults(string orderId) =>
        Get(BittradeEndpointIds.GetOrdersMatchResultsByOrderId, $"{BittradePaths.OrdersPath}/{orderId}/matchresults", query: null);

    public static WireCallSpec GetMatchResults(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        string? from = null,
        string? direct = null,
        string? size = null)
    {
        return Get(BittradeEndpointIds.GetMatchResults, BittradePaths.OrdersMatchResultsPath, BuildQuery(
            (BittradeQueryKeys.Symbol, symbol),
            (BittradeQueryKeys.Types, types),
            (BittradeQueryKeys.StartDate, startDate),
            (BittradeQueryKeys.EndDate, endDate),
            (BittradeQueryKeys.From, from),
            (BittradeQueryKeys.Direct, direct),
            (BittradeQueryKeys.Size, size)));
    }

    public static WireCallSpec GetDepositWithdraws(
        string type,
        string? currency = null,
        string? from = null,
        string? size = null,
        string? direct = null)
    {
        return Get(BittradeEndpointIds.GetDepositWithdraw, BittradePaths.DepositWithdrawPath, BuildQuery(
            (BittradeQueryKeys.Type, type),
            (BittradeQueryKeys.Currency, currency),
            (BittradeQueryKeys.From, from),
            (BittradeQueryKeys.Size, size),
            (BittradeQueryKeys.Direct, direct)));
    }

    public static WireCallSpec GetRetailOrders(
        string direct,
        string? status = null,
        string? startTime = null,
        string? endTime = null)
    {
        return Get(BittradeEndpointIds.GetOrderList, BittradePaths.RetailOrderListPath, BuildQuery(
            (BittradeQueryKeys.Direct, direct),
            (BittradeQueryKeys.Status, status),
            (BittradeQueryKeys.StartTime, startTime),
            (BittradeQueryKeys.EndTime, endTime)));
    }

    public static WireCallSpec PlaceOrder(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrdersPlace, BittradePaths.OrdersPlacePath, bodyJson);

    public static WireCallSpec CancelOrder(string orderId) =>
        Post(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId, $"{BittradePaths.OrdersPath}/{orderId}/submitcancel", bodyJson: null);

    public static WireCallSpec CancelOrders(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrdersBatchCancel, BittradePaths.OrdersBatchCancelPath, bodyJson);

    public static WireCallSpec CancelOpenOrders(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders, BittradePaths.OrdersBatchCancelOpenPath, bodyJson);

    public static WireCallSpec CreateWithdraw(string bodyJson) =>
        Post(BittradeEndpointIds.PostWithdrawApiCreate, BittradePaths.WithdrawCreatePath, bodyJson);

    public static WireCallSpec CancelWithdraw(string withdrawId) =>
        Post(BittradeEndpointIds.PostWithdrawVirtualCancelByWithdrawId, $"{BittradePaths.WithdrawVirtualPath}/{withdrawId}/cancel", bodyJson: null);

    public static WireCallSpec CreateRetailOrder(string bodyJson) =>
        Post(BittradeEndpointIds.PostOrderPlace, BittradePaths.RetailOrderPlacePath, bodyJson);

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
