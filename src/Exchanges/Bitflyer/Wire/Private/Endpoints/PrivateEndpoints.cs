using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Private.Endpoints;

internal static class PrivateEndpoints
{
    public static WireCallSpec GetBalance() =>
        WireSpecBuilder.Get(EndpointIds.GetBalance, Paths.GetBalancePath, query: null);

    public static WireCallSpec GetExecutionsPrivate(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetExecutionsPrivate,
            Paths.GetExecutionsPrivatePath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.ProductCode, productCode),
                (QueryKeys.ChildOrderId, childOrderId),
                (QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));
    }

    public static WireCallSpec GetPositions(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetPositions,
            Paths.GetPositionsPath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCollateral() =>
        WireSpecBuilder.Get(EndpointIds.GetCollateral, Paths.GetCollateralPath, query: null);

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
        return WireSpecBuilder.Get(
            EndpointIds.GetChildOrders,
            Paths.GetChildOrdersPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.ProductCode, productCode),
                (QueryKeys.ChildOrderState, childOrderStatusState),
                (QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
                (QueryKeys.ChildOrderId, childOrderId),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after),
                (QueryKeys.ParentOrderId, parentOrderId)));
    }

    public static WireCallSpec GetParentOrders(
        string productCode,
        string? parentOrderState = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetParentOrders,
            Paths.GetParentOrdersPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.ProductCode, productCode),
                (QueryKeys.ParentOrderState, parentOrderState),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));
    }

    public static WireCallSpec GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetParentOrder,
            Paths.GetParentOrderPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.ParentOrderId, parentOrderId),
                (QueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireCallSpec GetTradingCommission(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetTradingCommission,
            Paths.GetTradingCommissionPath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetPermissions() =>
        WireSpecBuilder.Get(EndpointIds.GetPermissions, Paths.GetPermissionsPath, query: null);

    public static WireCallSpec GetCollateralAccounts() =>
        WireSpecBuilder.Get(
            EndpointIds.GetCollateralAccounts,
            Paths.GetCollateralAccountsPath,
            query: null);

    public static WireCallSpec GetBalanceHistory(
        string? currencyCode = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetBalanceHistory,
            Paths.GetBalanceHistoryPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.CurrencyCode, currencyCode),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));
    }

    public static WireCallSpec GetCollateralHistory(
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetCollateralHistory,
            Paths.GetCollateralHistoryPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));
    }

    public static WireCallSpec GetAddresses() =>
        WireSpecBuilder.Get(EndpointIds.GetAddresses, Paths.GetAddressesPath, query: null);

    public static WireCallSpec GetCoinIns(
        string? count = null,
        string? before = null,
        string? after = null) =>
        WireSpecBuilder.Get(
            EndpointIds.GetCoinIns,
            Paths.GetCoinInsPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));

    public static WireCallSpec GetCoinOuts(
        string? messageId = null,
        string? count = null,
        string? before = null,
        string? after = null) =>
        WireSpecBuilder.Get(
            EndpointIds.GetCoinOuts,
            Paths.GetCoinOutsPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.MessageId, messageId),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));

    public static WireCallSpec GetDeposits(
        string? count = null,
        string? before = null,
        string? after = null) =>
        WireSpecBuilder.Get(
            EndpointIds.GetDeposits,
            Paths.GetDepositsPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));

    public static WireCallSpec GetWithdrawals(
        string? messageId = null,
        string? count = null,
        string? before = null,
        string? after = null) =>
        WireSpecBuilder.Get(
            EndpointIds.GetWithdrawals,
            Paths.GetWithdrawalsPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.MessageId, messageId),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));

    public static WireCallSpec GetBankAccounts() =>
        WireSpecBuilder.Get(EndpointIds.GetBankAccounts, Paths.GetBankAccountsPath, query: null);

    public static WireCallSpec SendChildOrder(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.SendChildOrder, Paths.SendChildOrderPath, bodyJson);

    public static WireCallSpec SendParentOrder(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.SendParentOrder, Paths.SendParentOrderPath, bodyJson);

    public static WireCallSpec CancelChildOrder(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.CancelChildOrder, Paths.CancelChildOrderPath, bodyJson);

    public static WireCallSpec CancelParentOrder(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.CancelParentOrder, Paths.CancelParentOrderPath, bodyJson);

    public static WireCallSpec CancelAllChildOrders(string bodyJson) =>
        WireSpecBuilder.Post(
            EndpointIds.CancelAllChildOrders,
            Paths.CancelAllChildOrdersPath,
            bodyJson);

    public static WireCallSpec Withdraw(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.Withdraw, Paths.WithdrawPath, bodyJson);
}
