using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Constants;

public static class EndpointIdCatalog
{
    private static readonly string[] All =
    {
        EndpointIds.GetSymbols,
        EndpointIds.GetCurrencies,
        EndpointIds.GetTimestamp,
        EndpointIds.GetHistoryKline,
        EndpointIds.GetDetailMerged,
        EndpointIds.GetTickers,
        EndpointIds.GetDepth,
        EndpointIds.GetTrade,
        EndpointIds.GetHistoryTrade,
        EndpointIds.GetAccounts,
        EndpointIds.GetAccountsBalanceByAccountId,
        EndpointIds.PostOrdersPlace,
        EndpointIds.GetOpenOrders,
        EndpointIds.PostOrdersSubmitCancelByOrderId,
        EndpointIds.PostOrdersBatchCancel,
        EndpointIds.PostOrdersBatchCancelOpenOrders,
        EndpointIds.GetOrdersByOrderId,
        EndpointIds.GetOrdersMatchResultsByOrderId,
        EndpointIds.GetOrders,
        EndpointIds.GetMatchResults,
        EndpointIds.PostWithdrawApiCreate,
        EndpointIds.PostWithdrawVirtualByAddressIdCreate,
        EndpointIds.PostWithdrawVirtualByWithdrawIdCancel,
        EndpointIds.PostWithdrawVirtualByWithdrawIdPlace,
        EndpointIds.GetWithdrawVirtualAddresses,
        EndpointIds.GetDepositWithdraw,
        EndpointIds.PostRetailOrderPlace,
        EndpointIds.GetRetailOrderList,
        EndpointIds.GetRetailOrderDetailByOrderId,
        EndpointIds.PostRetailOrderCancelByOrderId,
        EndpointIds.GetRetailAccountBalance,
        EndpointIds.PostRetailOrderHistory,
        EndpointIds.PostRetailOrderDetail,
        EndpointIds.PostRetailOrderCreate,
    };

    public static IReadOnlyCollection<string> GetAllEndpointIds() => All;
}
