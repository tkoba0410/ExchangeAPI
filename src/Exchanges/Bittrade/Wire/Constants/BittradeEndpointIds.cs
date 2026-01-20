// Exchanges/Bittrade/Wire/Constants/BittradeEndpointIds.cs
namespace ExchangeApi.Exchanges.Bittrade.Wire.Constants;

internal static class BittradeEndpointIds
{
    public const string GetSymbols = "GetSymbols";
    public const string GetCurrencys = "GetCurrencys";
    public const string GetTimestamp = "GetTimestamp";
    public const string GetHistoryKline = "GetHistoryKline";
    public const string GetDetailMerged = "GetDetailMerged";
    public const string GetTickers = "GetTickers";
    public const string GetDepth = "GetDepth";
    public const string GetTrade = "GetTrade";
    public const string GetHistoryTrade = "GetHistoryTrade";
    public const string GetAccounts = "GetAccounts";
    public const string GetAccountsBalanceByAccountId = "GetAccountsBalanceByAccountId";
    public const string PostOrdersPlace = "PostOrdersPlace";
    public const string GetOpenOrders = "GetOpenOrders";
    public const string PostOrdersSubmitCancelByOrderId = "PostOrdersSubmitCancelByOrderId";
    public const string PostOrdersBatchCancel = "PostOrdersBatchCancel";
    public const string PostOrdersBatchCancelOpenOrders = "PostOrdersBatchCancelOpenOrders";
    public const string GetOrdersByOrderId = "GetOrdersByOrderId";
    public const string GetOrdersMatchResultsByOrderId = "GetOrdersMatchResultsByOrderId";
    public const string GetOrders = "GetOrders";
    public const string GetMatchResults = "GetMatchResults";
    public const string PostWithdrawApiCreate = "PostWithdrawApiCreate";
    public const string PostWithdrawVirtualCancelByWithdrawId = "PostWithdrawVirtualCancelByWithdrawId";
    public const string GetDepositWithdraw = "GetDepositWithdraw";
    public const string PostOrderPlace = "PostOrderPlace";
    public const string GetOrderList = "GetOrderList";
    public const string GetMaintainTime = "GetMaintainTime";
}
