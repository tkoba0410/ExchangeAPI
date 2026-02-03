using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerBoardStateNormalizer
{
    public static BitflyerBoardStateNormalized Normalize(RawPublicDtos.BoardStateResponse wire) =>
        new(
            Health: ParseOptional(wire.Health),
            State: ParseOptional(wire.State),
            Data: ParseOptional(wire.Data));

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;
}
