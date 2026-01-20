using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;

internal static class BitflyerEndpoints
{
    public static WireCallSpec GetTicker(string productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerPaths.TickerPath : BitflyerPaths.GetTickerPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoard(string productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerPaths.BoardPath : BitflyerPaths.GetBoardPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetExecutions(
        string productCode,
        string? count = null,
        string? before = null,
        string? after = null,
        bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerPaths.ExecutionsPath : BitflyerPaths.GetExecutionsPublicPath;
        return Get(path, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetMarkets(string? region = null, bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerPaths.MarketsPath : BitflyerPaths.GetMarketsPath;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, query: null);
    }

    public static WireCallSpec GetChats(string? fromDate = null, string? region = null)
    {
        var path = BitflyerPaths.GetChatsPath;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, BuildQuery((BitflyerQueryKeys.FromDate, fromDate)));
    }

    public static WireCallSpec GetHealth(string productCode) =>
        Get(BitflyerPaths.GetHealthPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoardState(string productCode) =>
        Get(BitflyerPaths.GetBoardStatePath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCorporateLeverage() =>
        Get(BitflyerPaths.GetCorporateLeveragePath, query: null);

    public static WireCallSpec GetFundingRate(string productCode) =>
        Get(BitflyerPaths.GetFundingRatePath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBalances() =>
        Get(BitflyerPaths.GetBalancePath, query: null);

    public static WireCallSpec GetExecutions(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerPaths.GetExecutionsPrivatePath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.ChildOrderId, childOrderId),
            (BitflyerQueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetPositions(string productCode) =>
        Get(BitflyerPaths.GetPositionsPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCollateral() =>
        Get(BitflyerPaths.GetCollateralPath, query: null);

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
        return Get(BitflyerPaths.GetChildOrdersPath, BuildQuery(
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
        return Get(BitflyerPaths.GetParentOrdersPath, BuildQuery(
            (BitflyerQueryKeys.ProductCode, productCode),
            (BitflyerQueryKeys.ParentOrderState, parentOrderState),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        return Get(BitflyerPaths.GetParentOrderPath, BuildQuery(
            (BitflyerQueryKeys.ParentOrderId, parentOrderId),
            (BitflyerQueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireCallSpec GetTradingCommission(string productCode) =>
        Get(BitflyerPaths.GetTradingCommissionPath,
            BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetPermissions() =>
        Get(BitflyerPaths.GetPermissionsPath, query: null);

    public static WireCallSpec GetCollateralAccounts() =>
        Get(BitflyerPaths.GetCollateralAccountsPath, query: null);

    public static WireCallSpec GetBalanceHistory(
        string? currencyCode = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return Get(BitflyerPaths.GetBalanceHistoryPath, BuildQuery(
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
        return Get(BitflyerPaths.GetCollateralHistoryPath, BuildQuery(
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetAddresses() =>
        Get(BitflyerPaths.GetAddressesPath, query: null);

    public static WireCallSpec GetCoinIns(string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerPaths.GetCoinInsPath, BuildQuery(
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetCoinOuts(string? messageId = null, string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerPaths.GetCoinOutsPath, BuildQuery(
            (BitflyerQueryKeys.MessageId, messageId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetDeposits(string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerPaths.GetDepositsPath, BuildQuery(
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetWithdrawals(string? messageId = null, string? count = null, string? before = null, string? after = null) =>
        Get(BitflyerPaths.GetWithdrawalsPath, BuildQuery(
            (BitflyerQueryKeys.MessageId, messageId),
            (BitflyerQueryKeys.Count, count),
            (BitflyerQueryKeys.Before, before),
            (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetBankAccounts() =>
        Get(BitflyerPaths.GetBankAccountsPath, query: null);

    public static WireCallSpec SendChildOrder(string bodyJson) =>
        Post(BitflyerPaths.SendChildOrderPath, bodyJson);

    public static WireCallSpec SendParentOrder(string bodyJson) =>
        Post(BitflyerPaths.SendParentOrderPath, bodyJson);

    public static WireCallSpec CancelChildOrder(string bodyJson) =>
        Post(BitflyerPaths.CancelChildOrderPath, bodyJson);

    public static WireCallSpec CancelParentOrder(string bodyJson) =>
        Post(BitflyerPaths.CancelParentOrderPath, bodyJson);

    public static WireCallSpec CancelAllChildOrders(string bodyJson) =>
        Post(BitflyerPaths.CancelAllChildOrdersPath, bodyJson);

    public static WireCallSpec Withdraw(string bodyJson) =>
        Post(BitflyerPaths.WithdrawPath, bodyJson);

    private static WireCallSpec Get(string path, string? query) =>
        new(Method: "GET", Path: path, Query: query);

    private static WireCallSpec Post(string path, string? bodyJson) =>
        new(Method: "POST", Path: path, Query: null, BodyJson: bodyJson);

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
