using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerMarketNormalizer
{
    public static BitflyerMarketNormalized Normalize(RawPublicDtos.Market wire) =>
        new(
            ProductCode: wire.ProductCode,
            Alias: wire.Alias);
}
