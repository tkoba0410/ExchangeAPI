using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Private.Endpoints;

internal static class BitflyerPrivateEndpoints
{
    public static WireCallSpec GetBalance() =>
        BitflyerWireSpecBuilder.Get(BitflyerEndpointIds.GetBalance, BitflyerPaths.GetBalancePath, query: null);

    public static WireCallSpec GetExecutionsPrivate(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetExecutionsPrivate,
            BitflyerPaths.GetExecutionsPrivatePath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.ProductCode, productCode),
                (BitflyerQueryKeys.ChildOrderId, childOrderId),
                (BitflyerQueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetPositions(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetPositions,
            BitflyerPaths.GetPositionsPath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCollateral() =>
        BitflyerWireSpecBuilder.Get(BitflyerEndpointIds.GetCollateral, BitflyerPaths.GetCollateralPath, query: null);

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
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetChildOrders,
            BitflyerPaths.GetChildOrdersPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.ProductCode, productCode),
                (BitflyerQueryKeys.ChildOrderState, childOrderStatusState),
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
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetParentOrders,
            BitflyerPaths.GetParentOrdersPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.ProductCode, productCode),
                (BitflyerQueryKeys.ParentOrderState, parentOrderState),
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetParentOrder,
            BitflyerPaths.GetParentOrderPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.ParentOrderId, parentOrderId),
                (BitflyerQueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireCallSpec GetTradingCommission(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetTradingCommission,
            BitflyerPaths.GetTradingCommissionPath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetPermissions() =>
        BitflyerWireSpecBuilder.Get(BitflyerEndpointIds.GetPermissions, BitflyerPaths.GetPermissionsPath, query: null);

    public static WireCallSpec GetCollateralAccounts() =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetCollateralAccounts,
            BitflyerPaths.GetCollateralAccountsPath,
            query: null);

    public static WireCallSpec GetBalanceHistory(
        string? currencyCode = null,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetBalanceHistory,
            BitflyerPaths.GetBalanceHistoryPath,
            BitflyerWireSpecBuilder.BuildQuery(
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
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetCollateralHistory,
            BitflyerPaths.GetCollateralHistoryPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetAddresses() =>
        BitflyerWireSpecBuilder.Get(BitflyerEndpointIds.GetAddresses, BitflyerPaths.GetAddressesPath, query: null);

    public static WireCallSpec GetCoinIns(
        string? count = null,
        string? before = null,
        string? after = null) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetCoinIns,
            BitflyerPaths.GetCoinInsPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetCoinOuts(
        string? messageId = null,
        string? count = null,
        string? before = null,
        string? after = null) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetCoinOuts,
            BitflyerPaths.GetCoinOutsPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.MessageId, messageId),
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetDeposits(
        string? count = null,
        string? before = null,
        string? after = null) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetDeposits,
            BitflyerPaths.GetDepositsPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetWithdrawals(
        string? messageId = null,
        string? count = null,
        string? before = null,
        string? after = null) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetWithdrawals,
            BitflyerPaths.GetWithdrawalsPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.MessageId, messageId),
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));

    public static WireCallSpec GetBankAccounts() =>
        BitflyerWireSpecBuilder.Get(BitflyerEndpointIds.GetBankAccounts, BitflyerPaths.GetBankAccountsPath, query: null);

    public static WireCallSpec SendChildOrder(string bodyJson) =>
        BitflyerWireSpecBuilder.Post(BitflyerEndpointIds.SendChildOrder, BitflyerPaths.SendChildOrderPath, bodyJson);

    public static WireCallSpec SendParentOrder(string bodyJson) =>
        BitflyerWireSpecBuilder.Post(BitflyerEndpointIds.SendParentOrder, BitflyerPaths.SendParentOrderPath, bodyJson);

    public static WireCallSpec CancelChildOrder(string bodyJson) =>
        BitflyerWireSpecBuilder.Post(BitflyerEndpointIds.CancelChildOrder, BitflyerPaths.CancelChildOrderPath, bodyJson);

    public static WireCallSpec CancelParentOrder(string bodyJson) =>
        BitflyerWireSpecBuilder.Post(BitflyerEndpointIds.CancelParentOrder, BitflyerPaths.CancelParentOrderPath, bodyJson);

    public static WireCallSpec CancelAllChildOrders(string bodyJson) =>
        BitflyerWireSpecBuilder.Post(
            BitflyerEndpointIds.CancelAllChildOrders,
            BitflyerPaths.CancelAllChildOrdersPath,
            bodyJson);

    public static WireCallSpec Withdraw(string bodyJson) =>
        BitflyerWireSpecBuilder.Post(BitflyerEndpointIds.Withdraw, BitflyerPaths.WithdrawPath, bodyJson);
}
