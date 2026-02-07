namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Operations;

public static class BittradeOperations
{
    public static class MarketData
    {
        public const string GetTicker = "Bittrade.Market.GetTicker";
        public const string GetBoard = "Bittrade.Market.GetBoard";
        public const string GetExecutions = "Bittrade.Market.GetExecutions";
        public const string GetCandlesticks = "Bittrade.Market.GetCandlesticks";
        public const string GetTickers = "Bittrade.Market.GetTickers";
        public const string GetHistoryTrade = "Bittrade.Market.GetHistoryTrade";
    }

    public static class Trading
    {
        public const string PlaceOrder = "Bittrade.Trading.PlaceOrder";
        public const string CancelOrder = "Bittrade.Trading.CancelOrder";
        public const string GetOrders = "Bittrade.Trading.GetOrders";
        public const string GetOrder = "Bittrade.Trading.GetOrder";
    }

    public static class Account
    {
        public const string GetBalance = "Bittrade.Account.GetBalance";
    }

    public static class ExchangeInfo
    {
        public const string GetExchangeInfo = "Bittrade.ExchangeInfo.GetExchangeInfo";
        public const string GetCurrencys = "Bittrade.ExchangeInfo.GetCurrencys";
        public const string GetTimestamp = "Bittrade.ExchangeInfo.GetTimestamp";
    }

    public static class History
    {
        public const string GetOrders = "Bittrade.History.GetOrders";
        public const string GetExecutions = "Bittrade.History.GetExecutions";
    }
}
