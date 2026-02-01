using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerHealthNormalizer
{
    public static BitflyerHealthNormalized Normalize(RawPublicDtos.HealthResponse wire) =>
        new(wire.Status);
}
