using ExchangeApi.Contracts.Facade.Operations;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Operations;

internal static class Operations
{
    private const string ExchangePrefix = "Bitflyer";

    internal static class MarketData
    {
        public const string GetTicker = ExchangePrefix + "." + ContractOperations.MarketData.GetTicker;
        public const string GetBoard = ExchangePrefix + "." + ContractOperations.MarketData.GetBoard;
        public const string GetExecutions = ExchangePrefix + "." + ContractOperations.MarketData.GetExecutions;
        public const string GetCandlesticks = ExchangePrefix + "." + ContractOperations.MarketData.GetCandlesticks;
        public const string GetTickers = ExchangePrefix + "." + ContractOperations.MarketData.GetTickers;
        public const string GetHistoryTrade = ExchangePrefix + "." + ContractOperations.MarketData.GetHistoryTrade;
        public const string GetHealth = ExchangePrefix + "." + ContractOperations.MarketData.GetHealth;
        public const string GetBoardState = ExchangePrefix + "." + ContractOperations.MarketData.GetBoardState;
    }

    internal static class Trading
    {
        public const string PlaceOrder = ExchangePrefix + "." + ContractOperations.Trading.PlaceOrder;
        public const string CancelOrder = ExchangePrefix + "." + ContractOperations.Trading.CancelOrder;
        public const string GetOrders = ExchangePrefix + "." + ContractOperations.Trading.GetOrders;
        public const string GetOrder = ExchangePrefix + "." + ContractOperations.Trading.GetOrder;
    }

    internal static class Account
    {
        public const string GetBalance = ExchangePrefix + "." + ContractOperations.Account.GetBalance;
        public const string GetTradingCommission = ExchangePrefix + "." + ContractOperations.Account.GetTradingCommission;
    }

    internal static class ExchangeInfo
    {
        public const string GetExchangeInfo = ExchangePrefix + "." + ContractOperations.ExchangeInfo.GetExchangeInfo;
    }

    internal static class History
    {
        public const string GetOrders = ExchangePrefix + "." + ContractOperations.History.GetOrders;
        public const string GetExecutions = ExchangePrefix + "." + ContractOperations.History.GetExecutions;
    }
}
