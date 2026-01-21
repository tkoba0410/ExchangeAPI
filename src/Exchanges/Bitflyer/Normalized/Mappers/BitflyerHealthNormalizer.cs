using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using RawPublicModels = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerHealthNormalizer
{
    public static BitflyerHealthNormalized Normalize(RawPublicModels.HealthResponse wire) =>
        new(wire.Status);
}
