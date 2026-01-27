using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BitflyerMarketNormalizer
{
    public static BitflyerMarketNormalized Normalize(RawPublicDtos.Market wire) =>
        new(
            ProductCode: wire.ProductCode,
            Alias: wire.Alias);
}
