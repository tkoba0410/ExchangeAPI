namespace ExchangeApi.Contracts.Facade.Operations;

public static class ContractOperations
{
    public static class MarketData
    {
        public const string GetTicker = "MarketData.GetTicker";
        public const string GetBoard = "MarketData.GetBoard";
        public const string GetExecutions = "MarketData.GetExecutions";
        public const string GetCandlesticks = "MarketData.GetCandlesticks";
        public const string GetTickers = "MarketData.GetTickers";
        public const string GetHistoryTrade = "MarketData.GetHistoryTrade";
        public const string GetHealth = "MarketData.GetHealth";
        public const string GetBoardState = "MarketData.GetBoardState";
    }

    public static class Trading
    {
        public const string PlaceOrder = "Trading.PlaceOrder";
        public const string CancelOrder = "Trading.CancelOrder";
        public const string GetOrders = "Trading.GetOrders";
        public const string GetOrder = "Trading.GetOrder";
    }

    public static class Account
    {
        public const string GetBalance = "Account.GetBalance";
        public const string GetTradingCommission = "Account.GetTradingCommission";
    }

    public static class History
    {
        public const string GetOrders = "History.GetOrders";
        public const string GetExecutions = "History.GetExecutions";
    }
}
