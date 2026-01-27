using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BitflyerHealthNormalizer
{
    public static BitflyerHealthNormalized Normalize(RawPublicDtos.HealthResponse wire) =>
        new(wire.Status);
}
