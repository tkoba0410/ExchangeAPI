using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerMarketNormalizer
{
    public static BitflyerMarketNormalized Normalize(Market wire) =>
        new(
            ProductCode: wire.ProductCode.Value,
            Alias: wire.Alias);
}
