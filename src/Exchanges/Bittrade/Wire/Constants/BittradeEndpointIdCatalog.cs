using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Constants;

public static class BittradeEndpointIdCatalog
{
    private static readonly string[] All =
    {
        BittradeEndpointIds.GetSymbols,
        BittradeEndpointIds.GetCurrencys,
        BittradeEndpointIds.GetTimestamp,
        BittradeEndpointIds.GetHistoryKline,
        BittradeEndpointIds.GetDetailMerged,
        BittradeEndpointIds.GetTickers,
        BittradeEndpointIds.GetDepth,
        BittradeEndpointIds.GetTrade,
        BittradeEndpointIds.GetHistoryTrade,
        BittradeEndpointIds.GetAccounts,
        BittradeEndpointIds.GetAccountsBalanceByAccountId,
        BittradeEndpointIds.PostOrdersPlace,
        BittradeEndpointIds.GetOpenOrders,
        BittradeEndpointIds.PostOrdersSubmitCancelByOrderId,
        BittradeEndpointIds.PostOrdersBatchCancel,
        BittradeEndpointIds.PostOrdersBatchCancelOpenOrders,
        BittradeEndpointIds.GetOrdersByOrderId,
        BittradeEndpointIds.GetOrdersMatchResultsByOrderId,
        BittradeEndpointIds.GetOrders,
        BittradeEndpointIds.GetMatchResults,
        BittradeEndpointIds.PostWithdrawApiCreate,
        BittradeEndpointIds.PostWithdrawVirtualCancelByWithdrawId,
        BittradeEndpointIds.GetDepositWithdraw,
        BittradeEndpointIds.PostOrderPlace,
        BittradeEndpointIds.GetOrderList,
        BittradeEndpointIds.GetMaintainTime,
    };

    public static IReadOnlyCollection<string> GetAllEndpointIds() => All;
}
