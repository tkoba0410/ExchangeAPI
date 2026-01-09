using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;

internal static class BitflyerEndpoints
{
    public static WireCallSpec GetTicker(string productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerConstants.Paths.Ticker : BitflyerConstants.Paths.GetTicker,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoard(string productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerConstants.Paths.Board : BitflyerConstants.Paths.GetBoard,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetExecutions(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerConstants.Paths.Executions : BitflyerConstants.Paths.GetExecutions;
        return Get(path, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, productCode),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireCallSpec GetMarkets(string? region = null, bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerConstants.Paths.Markets : BitflyerConstants.Paths.GetMarkets;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, query: null);
    }

    public static WireCallSpec GetChats(string? fromDate = null, string? region = null)
    {
        var path = BitflyerConstants.Paths.GetChats;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, BuildQuery((BitflyerConstants.QueryKeys.FromDate, fromDate)));
    }

    public static WireCallSpec GetHealth(string productCode) =>
        Get(BitflyerConstants.Paths.GetHealth,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoardState(string productCode) =>
        Get(BitflyerConstants.Paths.GetBoardState,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCorporateLeverage() =>
        Get(BitflyerConstants.Paths.GetCorporateLeverage, query: null);

    public static WireCallSpec GetFundingRate(string productCode) =>
        Get(BitflyerConstants.Paths.GetFundingRate,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBalances() =>
        Get(BitflyerConstants.Paths.GetBalance, query: null);

    public static WireCallSpec GetExecutions(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetPrivateExecutions, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, productCode),
            (BitflyerConstants.QueryKeys.ChildOrderId, childOrderId),
            (BitflyerConstants.QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireCallSpec GetPositions(string productCode) =>
        Get(BitflyerConstants.Paths.GetPositions,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCollateral() =>
        Get(BitflyerConstants.Paths.GetCollateral, query: null);

    public static WireCallSpec GetChildOrders(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderId = null)
    {
        return Get(BitflyerConstants.Paths.GetChildOrders, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, productCode),
            (BitflyerConstants.QueryKeys.ChildOrderStatusState, childOrderStatusState),
            (BitflyerConstants.QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerConstants.QueryKeys.ChildOrderId, childOrderId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString()),
            (BitflyerConstants.QueryKeys.ParentOrderId, parentOrderId)));
    }

    public static WireCallSpec GetParentOrders(
        string productCode,
        string? parentOrderState = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetParentOrders, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, productCode),
            (BitflyerConstants.QueryKeys.ParentOrderState, parentOrderState),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireCallSpec GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        return Get(BitflyerConstants.Paths.GetParentOrder, BuildQuery(
            (BitflyerConstants.QueryKeys.ParentOrderId, parentOrderId),
            (BitflyerConstants.QueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireCallSpec GetTradingCommission(string productCode) =>
        Get(BitflyerConstants.Paths.GetTradingCommission,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetPermissions() =>
        Get(BitflyerConstants.Paths.GetPermissions, query: null);

    public static WireCallSpec GetCollateralAccounts() =>
        Get(BitflyerConstants.Paths.GetCollateralAccounts, query: null);

    public static WireCallSpec GetBalanceHistory(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetBalanceHistory, BuildQuery(
            (BitflyerConstants.QueryKeys.CurrencyCode, currencyCode),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireCallSpec GetCollateralHistory(
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetCollateralHistory, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireCallSpec GetAddresses() =>
        Get(BitflyerConstants.Paths.GetAddresses, query: null);

    public static WireCallSpec GetCoinIns(int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetCoinIns, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireCallSpec GetCoinOuts(string? messageId = null, int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetCoinOuts, BuildQuery(
            (BitflyerConstants.QueryKeys.MessageId, messageId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireCallSpec GetDeposits(int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetDeposits, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireCallSpec GetWithdrawals(string? messageId = null, int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetWithdrawals, BuildQuery(
            (BitflyerConstants.QueryKeys.MessageId, messageId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireCallSpec GetBankAccounts() =>
        Get(BitflyerConstants.Paths.GetBankAccounts, query: null);

    public static WireCallSpec SendChildOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.SendChildOrder, bodyJson);

    public static WireCallSpec SendParentOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.SendParentOrder, bodyJson);

    public static WireCallSpec CancelChildOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.CancelChildOrder, bodyJson);

    public static WireCallSpec CancelParentOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.CancelParentOrder, bodyJson);

    public static WireCallSpec CancelAllChildOrders(string bodyJson) =>
        Post(BitflyerConstants.Paths.CancelAllChildOrders, bodyJson);

    public static WireCallSpec Withdraw(string bodyJson) =>
        Post(BitflyerConstants.Paths.Withdraw, bodyJson);

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
