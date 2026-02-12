using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Vocabulary;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;

internal static class PrivateEndpoints
{
    public static WireCallSpec GetAccounts() =>
        WireSpecBuilder.Get(EndpointIds.GetAccounts, Paths.AccountsPath, query: null);

    public static WireCallSpec GetAccountsBalanceByAccountId(string accountId) =>
        WireSpecBuilder.Get(
            EndpointIds.GetAccountsBalanceByAccountId,
            Paths.AccountsBalancePath(accountId),
            query: null);

    public static WireCallSpec PostOrdersPlace(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.PostOrdersPlace, Paths.OrdersPlacePath, bodyJson);

    public static WireCallSpec GetOpenOrders(string symbol, string accountId) =>
        WireSpecBuilder.Get(
            EndpointIds.GetOpenOrders,
            Paths.OrdersOpenPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Symbol, symbol),
                (QueryKeys.AccountId, accountId)));

    public static WireCallSpec PostOrdersSubmitCancelByOrderId(string orderId) =>
        WireSpecBuilder.Post(
            EndpointIds.PostOrdersSubmitCancelByOrderId,
            Paths.OrdersSubmitCancelPath(orderId),
            bodyJson: null);

    public static WireCallSpec PostOrdersBatchCancel(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.PostOrdersBatchCancel, Paths.OrdersBatchCancelPath, bodyJson);

    public static WireCallSpec PostOrdersBatchCancelOpenOrders(string bodyJson) =>
        WireSpecBuilder.Post(
            EndpointIds.PostOrdersBatchCancelOpenOrders,
            Paths.OrdersBatchCancelOpenPath,
            bodyJson);

    public static WireCallSpec GetOrdersByOrderId(string orderId) =>
        WireSpecBuilder.Get(
            EndpointIds.GetOrdersByOrderId,
            Paths.OrdersByIdPath(orderId),
            query: null);

    public static WireCallSpec GetOrders() =>
        WireSpecBuilder.Get(EndpointIds.GetOrders, Paths.OrdersPath, query: null);

    public static WireCallSpec GetOrdersMatchResultsByOrderId(string orderId) =>
        WireSpecBuilder.Get(
            EndpointIds.GetOrdersMatchResultsByOrderId,
            Paths.OrdersMatchResultsByIdPath(orderId),
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
        return WireSpecBuilder.Get(
            EndpointIds.GetMatchResults,
            Paths.OrdersMatchResultsPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Symbol, symbol),
                (QueryKeys.Types, types),
                (QueryKeys.StartDate, startDate),
                (QueryKeys.EndDate, endDate),
                (QueryKeys.From, from),
                (QueryKeys.Direct, direct),
                (QueryKeys.Size, size)));
    }

    public static WireCallSpec PostWithdrawApiCreate(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.PostWithdrawApiCreate, Paths.WithdrawCreatePath, bodyJson);

    public static WireCallSpec PostWithdrawVirtualByAddressIdCreate(string addressId) =>
        WireSpecBuilder.Post(
            EndpointIds.PostWithdrawVirtualByAddressIdCreate,
            Paths.WithdrawVirtualByAddressCreatePath(addressId),
            bodyJson: null);

    public static WireCallSpec PostWithdrawVirtualByWithdrawIdCancel(string withdrawId) =>
        WireSpecBuilder.Post(
            EndpointIds.PostWithdrawVirtualByWithdrawIdCancel,
            Paths.WithdrawVirtualByIdCancelPath(withdrawId),
            bodyJson: null);

    public static WireCallSpec PostWithdrawVirtualByWithdrawIdPlace(string withdrawId) =>
        WireSpecBuilder.Post(
            EndpointIds.PostWithdrawVirtualByWithdrawIdPlace,
            Paths.WithdrawVirtualByIdPlacePath(withdrawId),
            bodyJson: null);

    public static WireCallSpec GetWithdrawVirtualAddresses() =>
        WireSpecBuilder.Get(
            EndpointIds.GetWithdrawVirtualAddresses,
            Paths.WithdrawVirtualAddressesPath,
            query: null);

    public static WireCallSpec GetDepositWithdraw(
        string type,
        string? currency = null,
        string? from = null,
        string? size = null,
        string? direct = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetDepositWithdraw,
            Paths.DepositWithdrawPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Type, type),
                (QueryKeys.Currency, currency),
                (QueryKeys.From, from),
                (QueryKeys.Size, size),
                (QueryKeys.Direct, direct)));
    }

    public static WireCallSpec PostRetailOrderPlace(string bodyJson) =>
        WireSpecBuilder.Post(EndpointIds.PostRetailOrderPlace, Paths.RetailOrderPlacePath, bodyJson);

    public static WireCallSpec GetRetailOrderList(
        string direct,
        string? status = null,
        string? startTime = null,
        string? endTime = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetRetailOrderList,
            Paths.RetailOrderListPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Direct, direct),
                (QueryKeys.Status, status),
                (QueryKeys.StartTime, startTime),
                (QueryKeys.EndTime, endTime)));
    }

    public static WireCallSpec GetRetailOrderDetailByOrderId(string orderId) =>
        WireSpecBuilder.Get(
            EndpointIds.GetRetailOrderDetailByOrderId,
            Paths.RetailOrderDetailByIdPath(orderId),
            query: null);

    public static WireCallSpec PostRetailOrderCancelByOrderId(string orderId) =>
        WireSpecBuilder.Post(
            EndpointIds.PostRetailOrderCancelByOrderId,
            Paths.RetailOrderCancelByIdPath(orderId),
            bodyJson: null);

    public static WireCallSpec GetRetailAccountBalance() =>
        WireSpecBuilder.Get(
            EndpointIds.GetRetailAccountBalance,
            Paths.RetailAccountBalancePath,
            query: null);

    public static WireCallSpec PostRetailOrderHistory(string bodyJson) =>
        WireSpecBuilder.Post(
            EndpointIds.PostRetailOrderHistory,
            Paths.RetailOrderHistoryPath,
            bodyJson);

    public static WireCallSpec PostRetailOrderDetail(string bodyJson) =>
        WireSpecBuilder.Post(
            EndpointIds.PostRetailOrderDetail,
            Paths.RetailOrderDetailPath,
            bodyJson);

    public static WireCallSpec PostRetailOrderCreate(string bodyJson) =>
        WireSpecBuilder.Post(
            EndpointIds.PostRetailOrderCreate,
            Paths.RetailOrderCreatePath,
            bodyJson);
}
