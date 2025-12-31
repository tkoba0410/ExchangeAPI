using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerHealthNormalizer
{
    public static BitflyerHealthNormalized Normalize(HealthResponse wire) =>
        new(wire.Status);
}
