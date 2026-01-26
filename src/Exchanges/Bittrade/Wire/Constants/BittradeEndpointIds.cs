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
    public const string PostWithdrawVirtualByAddressIdCreate = "PostWithdrawVirtualByAddressIdCreate";
    public const string PostWithdrawVirtualByWithdrawIdCancel = "PostWithdrawVirtualByWithdrawIdCancel";
    public const string PostWithdrawVirtualByWithdrawIdPlace = "PostWithdrawVirtualByWithdrawIdPlace";
    public const string GetWithdrawVirtualAddresses = "GetWithdrawVirtualAddresses";
    public const string GetDepositWithdraw = "GetDepositWithdraw";
    public const string PostRetailOrderPlace = "PostRetailOrderPlace";
    public const string GetRetailOrderList = "GetRetailOrderList";
    public const string GetRetailOrderDetailByOrderId = "GetRetailOrderDetailByOrderId";
    public const string PostRetailOrderCancelByOrderId = "PostRetailOrderCancelByOrderId";
    public const string GetRetailAccountBalance = "GetRetailAccountBalance";
    public const string PostRetailOrderHistory = "PostRetailOrderHistory";
    public const string PostRetailOrderDetail = "PostRetailOrderDetail";
    public const string PostRetailOrderCreate = "PostRetailOrderCreate";
}
