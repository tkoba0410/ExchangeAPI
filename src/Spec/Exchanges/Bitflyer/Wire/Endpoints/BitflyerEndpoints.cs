using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;

internal static class BitflyerEndpoints
{
    public static WireRequest GetTicker(RawProductCode productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerRawConstants.Paths.Ticker : BitflyerRawConstants.Paths.GetTicker,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetBoard(RawProductCode productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerRawConstants.Paths.Board : BitflyerRawConstants.Paths.GetBoard,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetExecutions(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerRawConstants.Paths.Executions : BitflyerRawConstants.Paths.GetExecutions;
        return Get(path, BuildQuery(
            (BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerRawConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerRawConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerRawConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetMarkets(string? region = null, bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerRawConstants.Paths.Markets : BitflyerRawConstants.Paths.GetMarkets;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, query: null);
    }

    public static WireRequest GetChats(string? fromDate = null, string? region = null)
    {
        var path = BitflyerRawConstants.Paths.GetChats;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, BuildQuery((BitflyerRawConstants.QueryKeys.FromDate, fromDate)));
    }

    public static WireRequest GetHealth(RawProductCode productCode) =>
        Get(BitflyerRawConstants.Paths.GetHealth,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetBoardState(RawProductCode productCode) =>
        Get(BitflyerRawConstants.Paths.GetBoardState,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetCorporateLeverage() =>
        Get(BitflyerRawConstants.Paths.GetCorporateLeverage, query: null);

    public static WireRequest GetFundingRate(RawProductCode productCode) =>
        Get(BitflyerRawConstants.Paths.GetFundingRate,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetBalances() =>
        Get(BitflyerConstants.Paths.GetBalance, query: null);

    public static WireRequest GetExecutions(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetPrivateExecutions, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerConstants.QueryKeys.ChildOrderId, childOrderId),
            (BitflyerConstants.QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetPositions(RawProductCode productCode) =>
        Get(BitflyerConstants.Paths.GetPositions,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetCollateral() =>
        Get(BitflyerConstants.Paths.GetCollateral, query: null);

    public static WireRequest GetChildOrders(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetChildOrders, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerConstants.QueryKeys.ChildOrderStatusState, childOrderStatusState),
            (BitflyerConstants.QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerConstants.QueryKeys.ChildOrderId, childOrderId),
            (BitflyerConstants.QueryKeys.ParentOrderId, parentOrderId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetTradingCommission(RawProductCode productCode) =>
        Get(BitflyerConstants.Paths.GetTradingCommission,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetPermissions() =>
        Get(BitflyerConstants.Paths.GetPermissions, query: null);

    public static WireRequest GetCollateralAccounts() =>
        Get(BitflyerConstants.Paths.GetCollateralAccounts, query: null);

    public static WireRequest GetParentOrders(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null)
    {
        return Get(BitflyerConstants.Paths.GetParentOrders, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString()),
            (BitflyerConstants.QueryKeys.ParentOrderStatusState, parentOrderStatusState)));
    }

    public static WireRequest GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        if (string.IsNullOrWhiteSpace(parentOrderId) && string.IsNullOrWhiteSpace(parentOrderAcceptanceId))
        {
            throw new ArgumentException("parentOrderId or parentOrderAcceptanceId is required.");
        }

        return Get(BitflyerConstants.Paths.GetParentOrder, BuildQuery(
            (BitflyerConstants.QueryKeys.ParentOrderId, parentOrderId),
            (BitflyerConstants.QueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireRequest GetBalanceHistory(
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

    public static WireRequest GetCollateralHistory(
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetCollateralHistory, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetAddresses() =>
        Get(BitflyerConstants.Paths.GetAddresses, query: null);

    public static WireRequest GetCoinIns(int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetCoinIns, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetCoinOuts(string? messageId = null, int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetCoinOuts, BuildQuery(
            (BitflyerConstants.QueryKeys.MessageId, messageId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetDeposits(int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetDeposits, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetWithdrawals(string? messageId = null, int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetWithdrawals, BuildQuery(
            (BitflyerConstants.QueryKeys.MessageId, messageId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetBankAccounts() =>
        Get(BitflyerConstants.Paths.GetBankAccounts, query: null);

    public static WireRequest SendChildOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.SendChildOrder, bodyJson);

    public static WireRequest CancelChildOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.CancelChildOrder, bodyJson);

    public static WireRequest CancelAllChildOrders(string bodyJson) =>
        Post(BitflyerConstants.Paths.CancelAllChildOrders, bodyJson);

    public static WireRequest SendParentOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.SendParentOrder, bodyJson);

    public static WireRequest CancelParentOrder(string bodyJson) =>
        Post(BitflyerConstants.Paths.CancelParentOrder, bodyJson);

    public static WireRequest Withdraw(string bodyJson) =>
        Post(BitflyerConstants.Paths.Withdraw, bodyJson);

    private static WireRequest Get(string path, string? query) =>
        new(Method: "GET", Path: path, Query: query);

    private static WireRequest Post(string path, string? bodyJson) =>
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

    private static string EnsureProductCode(RawProductCode productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return productCode.Value;
    }

}
