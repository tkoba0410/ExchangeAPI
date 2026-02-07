using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;

internal static class BittradePrivateEndpoints
{
    public static WireCallSpec GetAccounts() =>
        BittradeWireSpecBuilder.Get(BittradeEndpointIds.GetAccounts, BittradePaths.AccountsPath, query: null);

    public static WireCallSpec GetAccountsBalanceByAccountId(string accountId) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetAccountsBalanceByAccountId,
            BittradePaths.AccountsBalancePath(accountId),
            query: null);

    public static WireCallSpec PostOrdersPlace(string bodyJson) =>
        BittradeWireSpecBuilder.Post(BittradeEndpointIds.PostOrdersPlace, BittradePaths.OrdersPlacePath, bodyJson);

    public static WireCallSpec GetOpenOrders(string symbol, string accountId) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetOpenOrders,
            BittradePaths.OrdersOpenPath,
            BittradeWireSpecBuilder.BuildQuery(
                (BittradeQueryKeys.Symbol, symbol),
                (BittradeQueryKeys.AccountId, accountId)));

    public static WireCallSpec PostOrdersSubmitCancelByOrderId(string orderId) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostOrdersSubmitCancelByOrderId,
            BittradePaths.OrdersSubmitCancelPath(orderId),
            bodyJson: null);

    public static WireCallSpec PostOrdersBatchCancel(string bodyJson) =>
        BittradeWireSpecBuilder.Post(BittradeEndpointIds.PostOrdersBatchCancel, BittradePaths.OrdersBatchCancelPath, bodyJson);

    public static WireCallSpec PostOrdersBatchCancelOpenOrders(string bodyJson) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostOrdersBatchCancelOpenOrders,
            BittradePaths.OrdersBatchCancelOpenPath,
            bodyJson);

    public static WireCallSpec GetOrdersByOrderId(string orderId) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetOrdersByOrderId,
            BittradePaths.OrdersByIdPath(orderId),
            query: null);

    public static WireCallSpec GetOrders() =>
        BittradeWireSpecBuilder.Get(BittradeEndpointIds.GetOrders, BittradePaths.OrdersPath, query: null);

    public static WireCallSpec GetOrdersMatchResultsByOrderId(string orderId) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetOrdersMatchResultsByOrderId,
            BittradePaths.OrdersMatchResultsByIdPath(orderId),
            query: null);

    public static WireCallSpec GetMatchResults(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        string? from = null,
        string? direct = null,
        string? size = null)
    {
        return BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetMatchResults,
            BittradePaths.OrdersMatchResultsPath,
            BittradeWireSpecBuilder.BuildQuery(
                (BittradeQueryKeys.Symbol, symbol),
                (BittradeQueryKeys.Types, types),
                (BittradeQueryKeys.StartDate, startDate),
                (BittradeQueryKeys.EndDate, endDate),
                (BittradeQueryKeys.From, from),
                (BittradeQueryKeys.Direct, direct),
                (BittradeQueryKeys.Size, size)));
    }

    public static WireCallSpec PostWithdrawApiCreate(string bodyJson) =>
        BittradeWireSpecBuilder.Post(BittradeEndpointIds.PostWithdrawApiCreate, BittradePaths.WithdrawCreatePath, bodyJson);

    public static WireCallSpec PostWithdrawVirtualByAddressIdCreate(string addressId) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostWithdrawVirtualByAddressIdCreate,
            BittradePaths.WithdrawVirtualByAddressCreatePath(addressId),
            bodyJson: null);

    public static WireCallSpec PostWithdrawVirtualByWithdrawIdCancel(string withdrawId) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdCancel,
            BittradePaths.WithdrawVirtualByIdCancelPath(withdrawId),
            bodyJson: null);

    public static WireCallSpec PostWithdrawVirtualByWithdrawIdPlace(string withdrawId) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdPlace,
            BittradePaths.WithdrawVirtualByIdPlacePath(withdrawId),
            bodyJson: null);

    public static WireCallSpec GetWithdrawVirtualAddresses() =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetWithdrawVirtualAddresses,
            BittradePaths.WithdrawVirtualAddressesPath,
            query: null);

    public static WireCallSpec GetDepositWithdraw(
        string type,
        string? currency = null,
        string? from = null,
        string? size = null,
        string? direct = null)
    {
        return BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetDepositWithdraw,
            BittradePaths.DepositWithdrawPath,
            BittradeWireSpecBuilder.BuildQuery(
                (BittradeQueryKeys.Type, type),
                (BittradeQueryKeys.Currency, currency),
                (BittradeQueryKeys.From, from),
                (BittradeQueryKeys.Size, size),
                (BittradeQueryKeys.Direct, direct)));
    }

    public static WireCallSpec PostRetailOrderPlace(string bodyJson) =>
        BittradeWireSpecBuilder.Post(BittradeEndpointIds.PostRetailOrderPlace, BittradePaths.RetailOrderPlacePath, bodyJson);

    public static WireCallSpec GetRetailOrderList(
        string direct,
        string? status = null,
        string? startTime = null,
        string? endTime = null)
    {
        return BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetRetailOrderList,
            BittradePaths.RetailOrderListPath,
            BittradeWireSpecBuilder.BuildQuery(
                (BittradeQueryKeys.Direct, direct),
                (BittradeQueryKeys.Status, status),
                (BittradeQueryKeys.StartTime, startTime),
                (BittradeQueryKeys.EndTime, endTime)));
    }

    public static WireCallSpec GetRetailOrderDetailByOrderId(string orderId) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetRetailOrderDetailByOrderId,
            BittradePaths.RetailOrderDetailByIdPath(orderId),
            query: null);

    public static WireCallSpec PostRetailOrderCancelByOrderId(string orderId) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostRetailOrderCancelByOrderId,
            BittradePaths.RetailOrderCancelByIdPath(orderId),
            bodyJson: null);

    public static WireCallSpec GetRetailAccountBalance() =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetRetailAccountBalance,
            BittradePaths.RetailAccountBalancePath,
            query: null);

    public static WireCallSpec PostRetailOrderHistory(string bodyJson) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostRetailOrderHistory,
            BittradePaths.RetailOrderHistoryPath,
            bodyJson);

    public static WireCallSpec PostRetailOrderDetail(string bodyJson) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostRetailOrderDetail,
            BittradePaths.RetailOrderDetailPath,
            bodyJson);

    public static WireCallSpec PostRetailOrderCreate(string bodyJson) =>
        BittradeWireSpecBuilder.Post(
            BittradeEndpointIds.PostRetailOrderCreate,
            BittradePaths.RetailOrderCreatePath,
            bodyJson);
}
