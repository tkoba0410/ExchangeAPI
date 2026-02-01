namespace ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;

internal static class BittradePaths
{
    public const string MarketMergedPath = "/market/detail/merged";
    public const string MarketDepthPath = "/market/depth";
    public const string MarketTradePath = "/market/trade";
    public const string MarketKlinePath = "/market/history/kline";
    public const string MarketTickersPath = "/market/tickers";
    public const string MarketHistoryTradePath = "/market/history/trade";
    public const string CommonTimestampPath = "/v1/common/timestamp";
    public const string CommonSymbolsPath = "/v1/common/symbols";
    public const string CommonCurrenciesPath = "/v1/common/currencys";
    public const string AccountsPath = "/v1/account/accounts";
    public const string OrdersOpenPath = "/v1/order/openOrders";
    public const string OrdersPath = "/v1/order/orders";
    public const string OrdersMatchResultsPath = "/v1/order/matchresults";
    public const string OrdersPlacePath = "/v1/order/orders/place";
    public const string OrdersBatchCancelPath = "/v1/order/orders/batchcancel";
    public const string OrdersBatchCancelOpenPath = "/v1/order/orders/batchCancelOpenOrders";
    public const string DepositWithdrawPath = "/v1/query/deposit-withdraw";
    public const string WithdrawVirtualAddressesPath = "/v1/dw/withdraw-virtual/addresses";
    public const string RetailOrderListPath = "/v1/retail/order/list";
    public const string RetailOrderPlacePath = "/v1/retail/order/place";
    public const string RetailOrderDetailPath = "/v1/retail/order/detail";
    public const string RetailOrderCancelPath = "/v1/retail/order/cancel";
    public const string RetailOrderHistoryPath = "/v1/retail/order/history";
    public const string RetailOrderCreatePath = "/v1/retail/order/create";
    public const string RetailAccountBalancePath = "/v1/retail/account/balance";
    public const string WithdrawCreatePath = "/v1/dw/withdraw/api/create";
    public const string WithdrawVirtualPath = "/v1/dw/withdraw-virtual";

    public static string AccountsBalancePath(string accountId) =>
        $"{AccountsPath}/{accountId}/balance";

    public static string OrdersSubmitCancelPath(string orderId) =>
        $"{OrdersPath}/{orderId}/submitcancel";

    public static string OrdersByIdPath(string orderId) =>
        $"{OrdersPath}/{orderId}";

    public static string OrdersMatchResultsByIdPath(string orderId) =>
        $"{OrdersPath}/{orderId}/matchresults";

    public static string WithdrawVirtualByAddressCreatePath(string addressId) =>
        $"{WithdrawVirtualPath}/{addressId}/create";

    public static string WithdrawVirtualByIdCancelPath(string withdrawId) =>
        $"{WithdrawVirtualPath}/{withdrawId}/cancel";

    public static string WithdrawVirtualByIdPlacePath(string withdrawId) =>
        $"{WithdrawVirtualPath}/{withdrawId}/place";

    public static string RetailOrderDetailByIdPath(string orderId) =>
        $"{RetailOrderDetailPath}/{orderId}";

    public static string RetailOrderCancelByIdPath(string orderId) =>
        $"{RetailOrderCancelPath}/{orderId}";
}
