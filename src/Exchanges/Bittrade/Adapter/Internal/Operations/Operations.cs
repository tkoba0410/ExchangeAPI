namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Operations;

internal static class Operations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bittrade.MarketData.GetTicker";
        public const string GetBoard = "Bittrade.MarketData.GetBoard";
        public const string GetExecutions = "Bittrade.MarketData.GetExecutions";
        public const string GetCandlesticks = "Bittrade.MarketData.GetCandlesticks";
        public const string GetTickers = "Bittrade.MarketData.GetTickers";
        public const string GetHistoryTrade = "Bittrade.MarketData.GetHistoryTrade";
    }

    internal static class Trading
    {
        public const string PlaceOrder = "Bittrade.Trading.PlaceOrder";
        public const string CancelOrder = "Bittrade.Trading.CancelOrder";
        public const string GetOrders = "Bittrade.Trading.GetOrders";
        public const string GetOrder = "Bittrade.Trading.GetOrder";
    }

    internal static class Account
    {
        public const string GetBalance = "Bittrade.Account.GetBalance";
    }

    internal static class ExchangeInfo
    {
        public const string GetExchangeInfo = "Bittrade.ExchangeInfo.GetExchangeInfo";
    }

    internal static class History
    {
        public const string GetOrders = "Bittrade.History.GetOrders";
        public const string GetExecutions = "Bittrade.History.GetExecutions";
    }
}
