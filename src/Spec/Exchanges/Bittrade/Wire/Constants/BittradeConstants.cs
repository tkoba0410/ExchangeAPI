namespace ExchangeApi.Exchanges.Bittrade.Wire.Constants;

internal static class BittradeConstants
{
    internal static class Paths
    {
        public const string MarketMerged = "market/detail/merged";
        public const string MarketDepth = "market/depth";
        public const string MarketTrade = "market/trade";
        public const string MarketKline = "market/history/kline";
        public const string MarketTickers = "market/tickers";
        public const string MarketHistoryTrade = "market/history/trade";
        public const string CommonTimestamp = "v1/common/timestamp";
        public const string CommonSymbols = "v1/common/symbols";
        public const string CommonCurrencies = "v1/common/currencys";
        public const string RetailMaintainTime = "v1/retail/maintain/time";
        public const string Accounts = "v1/account/accounts";
        public const string OrdersOpen = "v1/order/openOrders";
        public const string Orders = "v1/order/orders";
        public const string OrdersMatchResults = "v1/order/matchresults";
        public const string OrdersPlace = "v1/order/orders/place";
        public const string OrdersBatchCancel = "v1/order/orders/batchcancel";
        public const string OrdersBatchCancelOpen = "v1/order/orders/batchCancelOpenOrders";
        public const string DepositWithdraw = "v1/query/deposit-withdraw";
        public const string RetailOrderList = "v1/retail/order/list";
        public const string RetailOrderPlace = "v1/retail/order/place";
        public const string WithdrawCreate = "v1/dw/withdraw/api/create";
        public const string WithdrawVirtual = "v1/dw/withdraw-virtual";
    }
}
