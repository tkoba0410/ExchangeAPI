namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Operations;

internal static class BittradeOperations
{
    internal static class Trading
    {
        public const string PlaceOrder = "Bittrade.Trading.PlaceOrder";
        public const string CancelOrder = "Bittrade.Trading.CancelOrder";
        public const string GetOpenOrders = "Bittrade.Trading.GetOpenOrders";
        public const string GetOrder = "Bittrade.Trading.GetOrder";
    }

    internal static class History
    {
        public const string GetOrders = "Bittrade.History.GetOrders";
        public const string GetExecutions = "Bittrade.History.GetExecutions";
    }
}
