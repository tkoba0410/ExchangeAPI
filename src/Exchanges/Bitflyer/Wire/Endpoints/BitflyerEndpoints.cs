using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;

internal static class BitflyerEndpoints
{
    public static WireCallSpec GetTicker(string productCode) =>
        Get(
            BitflyerEndpointIds.GetTicker,
            BitflyerPaths.GetTickerPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec Ticker(string productCode) =>
        Get(
            BitflyerEndpointIds.Ticker,
            BitflyerPaths.TickerPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoard(string productCode) =>
        Get(
            BitflyerEndpointIds.GetBoard,
            BitflyerPaths.GetBoardPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec Board(string productCode) =>
        Get(
            BitflyerEndpointIds.Board,
            BitflyerPaths.BoardPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetExecutionsPublic(
        string productCode,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerEndpointIds.GetExecutionsPublic, BitflyerPaths.GetExecutionsPublicPath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec Executions(
        string productCode,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerEndpointIds.Executions, BitflyerPaths.ExecutionsPath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetMarkets(string? region = null)
    {
        var path = BitflyerPaths.GetMarketsPath;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(BitflyerEndpointIds.GetMarkets, path, query: null);
    }

    public static WireCallSpec Markets(string? region = null)
    {
        var path = BitflyerPaths.MarketsPath;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(BitflyerEndpointIds.Markets, path, query: null);
    }

    public static WireCallSpec GetChats(string? fromDate = null, string? region = null)
    {
        var path = BitflyerPaths.GetChatsPath;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(BitflyerEndpointIds.GetChats, path, BuildQuery((BitflyerQueryKeys.FromDate, fromDate)));
    }

    public static WireCallSpec GetHealth(string productCode) =>
        Get(BitflyerEndpointIds.GetHealth, BitflyerPaths.GetHealthPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoardState(string productCode) =>
        Get(BitflyerEndpointIds.GetBoardState, BitflyerPaths.GetBoardStatePath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCorporateLeverage() =>
        Get(BitflyerEndpointIds.GetCorporateLeverage, BitflyerPaths.GetCorporateLeveragePath, query: null);

    public static WireCallSpec GetFundingRate(string productCode) =>
        Get(BitflyerEndpointIds.GetFundingRate, BitflyerPaths.GetFundingRatePath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBalances() =>
        Get(BitflyerEndpointIds.GetBalance, BitflyerPaths.GetBalancePath, query: null);

    public static WireCallSpec GetExecutions(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerEndpointIds.GetExecutionsPrivate, BitflyerPaths.GetExecutionsPrivatePath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.ChildOrderId, childOrderId),
            (BitflyerQueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetPositions(string productCode) =>
        Get(BitflyerEndpointIds.GetPositions, BitflyerPaths.GetPositionsPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCollateral() =>
        Get(BitflyerEndpointIds.GetCollateral, BitflyerPaths.GetCollateralPath, query: null);

    public static WireCallSpec GetChildOrders(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? count = null,
        string? before = null,
        string? after = null,
        string? parentOrderId = null)
    {
        return Get(BitflyerEndpointIds.GetChildOrders, BitflyerPaths.GetChildOrdersPath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.ChildOrderStatusState, childOrderStatusState),
            (BitflyerQueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerQueryKeys.ChildOrderId, childOrderId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after),
            (BitflyerQueryKeys.ParentOrderId, parentOrderId)));
    }

    public static WireCallSpec GetParentOrders(
        string productCode,
        string? parentOrderState = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerEndpointIds.GetParentOrders, BitflyerPaths.GetParentOrdersPath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.ParentOrderState, parentOrderState),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        return Get(BitflyerEndpointIds.GetParentOrder, BitflyerPaths.GetParentOrderPath, BuildQuery(
            (BitflyerQueryKeys.ParentOrderId, parentOrderId),
            (BitflyerQueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireCallSpec GetTradingCommission(string productCode) =>
        Get(BitflyerEndpointIds.GetTradingCommission, BitflyerPaths.GetTradingCommissionPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetPermissions() =>
        Get(BitflyerEndpointIds.GetPermissions, BitflyerPaths.GetPermissionsPath, query: null);

    public static WireCallSpec GetCollateralAccounts() =>
        Get(BitflyerEndpointIds.GetCollateralAccounts, BitflyerPaths.GetCollateralAccountsPath, query: null);

    public static WireCallSpec GetBalanceHistory(
        string? currencyCode = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerEndpointIds.GetBalanceHistory, BitflyerPaths.GetBalanceHistoryPath, BuildQuery(
            (BitflyerQueryKeys.CurrencyCode, currencyCode),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetCollateralHistory(
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerEndpointIds.GetCollateralHistory, BitflyerPaths.GetCollateralHistoryPath, BuildQuery(
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetAddresses() =>
        Get(BitflyerEndpointIds.GetAddresses, BitflyerPaths.GetAddressesPath, query: null);

    public static WireCallSpec GetCoinIns(string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerEndpointIds.GetCoinIns, BitflyerPaths.GetCoinInsPath, BuildQuery(
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetCoinOuts(string? messageId = null, string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerEndpointIds.GetCoinOuts, BitflyerPaths.GetCoinOutsPath, BuildQuery(
            (BitflyerQueryKeys.MessageId, messageId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetDeposits(string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerEndpointIds.GetDeposits, BitflyerPaths.GetDepositsPath, BuildQuery(
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetWithdrawals(string? messageId = null, string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerEndpointIds.GetWithdrawals, BitflyerPaths.GetWithdrawalsPath, BuildQuery(
            (BitflyerQueryKeys.MessageId, messageId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetBankAccounts() =>
        Get(BitflyerEndpointIds.GetBankAccounts, BitflyerPaths.GetBankAccountsPath, query: null);

    public static WireCallSpec SendChildOrder(string bodyJson) =>
        Post(BitflyerEndpointIds.SendChildOrder, BitflyerPaths.SendChildOrderPath, bodyJson);

    public static WireCallSpec SendParentOrder(string bodyJson) =>
        Post(BitflyerEndpointIds.SendParentOrder, BitflyerPaths.SendParentOrderPath, bodyJson);

    public static WireCallSpec CancelChildOrder(string bodyJson) =>
        Post(BitflyerEndpointIds.CancelChildOrder, BitflyerPaths.CancelChildOrderPath, bodyJson);

    public static WireCallSpec CancelParentOrder(string bodyJson) =>
        Post(BitflyerEndpointIds.CancelParentOrder, BitflyerPaths.CancelParentOrderPath, bodyJson);

    public static WireCallSpec CancelAllChildOrders(string bodyJson) =>
        Post(BitflyerEndpointIds.CancelAllChildOrders, BitflyerPaths.CancelAllChildOrdersPath, bodyJson);

    public static WireCallSpec Withdraw(string bodyJson) =>
        Post(BitflyerEndpointIds.Withdraw, BitflyerPaths.WithdrawPath, bodyJson);

    private static WireCallSpec Get(string endpointId, string path, string? query) =>
        new(Method: "GET", Path: path, EndpointId: endpointId, Query: query);

    private static WireCallSpec Post(string endpointId, string path, string? bodyJson) =>
        new(Method: "POST", Path: path, EndpointId: endpointId, Query: null, BodyJson: bodyJson);

    private static string? BuildQuery(params (string Key, string? Value)[] entries)
    {
        var parts = new List<string>();
        foreach (var (key, value) in entries)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
    }

}
