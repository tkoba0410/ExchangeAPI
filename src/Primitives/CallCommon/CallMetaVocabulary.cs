namespace ExchangeApi.Primitives.CallCommon;

public static class CallMetaVocabulary
{
    public static class Layer
    {
        public const string Contracts = "Contracts";
        public const string Raw = "Raw";
        public const string Normalized = "Normalized";
        public const string Adapter = "Adapter";
        public const string Wire = "Wire";
        public const string Tests = "Tests";
    }

    public static class Component
    {
        public const string MarketCatalogResolver = "MarketCatalogResolver";
        public const string NormalizedMarketResolver = "NormalizedMarketResolver";
        public const string WireSendRawAsync = "Wire.SendRawAsync";
    }
}
