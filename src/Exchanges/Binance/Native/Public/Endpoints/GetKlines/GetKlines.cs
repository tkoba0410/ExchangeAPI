namespace ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

public static class GetKlines
{
    public sealed class Item
    {
        public required long OpenTime { get; init; }
        public required decimal OpenPrice { get; init; }
        public required decimal HighPrice { get; init; }
        public required decimal LowPrice { get; init; }
        public required decimal ClosePrice { get; init; }
        public required decimal Volume { get; init; }
        public required long CloseTime { get; init; }
        public required decimal QuoteAssetVolume { get; init; }
        public required int NumberOfTrades { get; init; }
        public required decimal TakerBuyBaseAssetVolume { get; init; }
        public required decimal TakerBuyQuoteAssetVolume { get; init; }
    }
}
