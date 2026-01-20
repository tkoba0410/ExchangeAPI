namespace ExchangeApi.Exchanges.Bittrade.Wire.Constants;

internal static class BittradePaths
{
    public const string MarketMergedPath = "market/detail/merged";
    public const string MarketDepthPath = "market/depth";
    public const string MarketTradePath = "market/trade";
    public const string MarketKlinePath = "market/history/kline";
    public const string MarketTickersPath = "market/tickers";
    public const string MarketHistoryTradePath = "market/history/trade";
    public const string CommonTimestampPath = "v1/common/timestamp";
    public const string CommonSymbolsPath = "v1/common/symbols";
    public const string CommonCurrenciesPath = "v1/common/currencys";
    public const string RetailMaintainTimePath = "v1/retail/maintain/time";
    public const string AccountsPath = "v1/account/accounts";
    public const string OrdersOpenPath = "v1/order/openOrders";
    public const string OrdersPath = "v1/order/orders";
    public const string OrdersMatchResultsPath = "v1/order/matchresults";
    public const string OrdersPlacePath = "v1/order/orders/place";
    public const string OrdersBatchCancelPath = "v1/order/orders/batchcancel";
    public const string OrdersBatchCancelOpenPath = "v1/order/orders/batchCancelOpenOrders";
    public const string DepositWithdrawPath = "v1/query/deposit-withdraw";
    public const string RetailOrderListPath = "v1/retail/order/list";
    public const string RetailOrderPlacePath = "v1/retail/order/place";
    public const string WithdrawCreatePath = "v1/dw/withdraw/api/create";
    public const string WithdrawVirtualPath = "v1/dw/withdraw-virtual";
}
