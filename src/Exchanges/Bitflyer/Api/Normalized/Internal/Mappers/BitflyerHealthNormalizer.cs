using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerHealthNormalizer
{
    public static BitflyerHealthNormalized Normalize(RawPublicDtos.HealthResponse wire) =>
        new(ParseOptional(wire.Status));

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;
}
