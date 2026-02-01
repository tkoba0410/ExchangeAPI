namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;

internal static class BitflyerOperations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bitflyer.MarketData.GetTicker";
        public const string GetOrderBook = "Bitflyer.MarketData.GetOrderBook";
        public const string GetExecutions = "Bitflyer.MarketData.GetExecutions";
        public const string GetCandlesticks = "Bitflyer.MarketData.GetCandlesticks";
        public const string GetTickers = "Bitflyer.MarketData.GetTickers";
        public const string GetHistoryTrade = "Bitflyer.MarketData.GetHistoryTrade";
        public const string GetHealth = "Bitflyer.MarketData.GetHealth";
        public const string GetBoardState = "Bitflyer.MarketData.GetBoardState";
    }

    internal static class Trading
    {
        public const string PlaceOrder = "Bitflyer.Trading.PlaceOrder";
        public const string CancelOrder = "Bitflyer.Trading.CancelOrder";
        public const string GetOpenOrders = "Bitflyer.Trading.GetOpenOrders";
        public const string GetOrder = "Bitflyer.Trading.GetOrder";
    }

    internal static class Account
    {
        public const string GetBalances = "Bitflyer.Account.GetBalances";
        public const string GetTradingCommission = "Bitflyer.Account.GetTradingCommission";
    }

    internal static class ExchangeInfo
    {
        public const string GetExchangeInfo = "Bitflyer.ExchangeInfo.GetExchangeInfo";
        public const string GetCurrencys = "Bitflyer.ExchangeInfo.GetCurrencys";
        public const string GetTimestamp = "Bitflyer.ExchangeInfo.GetTimestamp";
    }

    internal static class History
    {
        public const string GetOrders = "Bitflyer.History.GetOrders";
        public const string GetExecutions = "Bitflyer.History.GetExecutions";
    }
}
