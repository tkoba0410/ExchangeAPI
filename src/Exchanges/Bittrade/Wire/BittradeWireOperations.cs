namespace ExchangeApi.Exchanges.Bittrade.Wire;

internal static class BittradeWireOperations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bittrade.MarketData.GetTicker";
        public const string GetOrderBook = "Bittrade.MarketData.GetOrderBook";
    }

    internal static class Trading
    {
        public const string PlaceOrder = "Bittrade.Trading.PlaceOrder";
        public const string CancelOrder = "Bittrade.Trading.CancelOrder";
        public const string GetOpenOrders = "Bittrade.Trading.GetOpenOrders";
        public const string GetOrder = "Bittrade.Trading.GetOrder";
    }
}
