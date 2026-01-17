namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Operations;

public static class BittradeOperations
{
    public static class MarketData
    {
        public const string GetTicker = "Bittrade.Market.GetTicker";
        public const string GetOrderBook = "Bittrade.Market.GetOrderBook";
        public const string GetExecutions = "Bittrade.Market.GetExecutions";
    }

    public static class Trading
    {
        public const string PlaceOrder = "Bittrade.Trading.PlaceOrder";
        public const string CancelOrder = "Bittrade.Trading.CancelOrder";
        public const string GetOpenOrders = "Bittrade.Trading.GetOpenOrders";
        public const string GetOrder = "Bittrade.Trading.GetOrder";
    }

    public static class Account
    {
        public const string GetBalances = "Bittrade.Account.GetBalances";
    }

    public static class ExchangeInfo
    {
        public const string GetExchangeInfo = "Bittrade.ExchangeInfo.GetExchangeInfo";
    }

    public static class History
    {
        public const string GetOrders = "Bittrade.History.GetOrders";
        public const string GetExecutions = "Bittrade.History.GetExecutions";
    }
}
