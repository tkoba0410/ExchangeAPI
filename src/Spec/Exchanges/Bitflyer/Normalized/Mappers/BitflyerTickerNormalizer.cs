using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerTickerNormalizer
{
    public static BitflyerTickerNormalized Normalize(Ticker wire) =>
        new(
            ProductCode: wire.ProductCode,
            LastTradedPrice: wire.LastTradedPrice,
            Timestamp: wire.Timestamp);
}
