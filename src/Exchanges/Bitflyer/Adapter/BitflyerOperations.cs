namespace ExchangeApi.Exchanges.Bitflyer.Adapter;

internal static class BitflyerOperations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bitflyer.MarketData.GetTicker";
        public const string GetOrderBook = "Bitflyer.MarketData.GetOrderBook";
        public const string GetExecutions = "Bitflyer.MarketData.GetExecutions";
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
        public const string GetAccountExecutions = "Bitflyer.Account.GetAccountExecutions";
        public const string GetTradingCommission = "Bitflyer.Account.GetTradingCommission";
    }

    internal static class Margin
    {
        public const string GetBalances = "Bitflyer.Margin.GetBalances";
        public const string GetAccountExecutions = "Bitflyer.Margin.GetAccountExecutions";
        public const string GetOpenPositions = "Bitflyer.Margin.GetOpenPositions";
        public const string GetCollateral = "Bitflyer.Margin.GetCollateral";
    }
}
