using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BitflyerHealthNormalizer
{
    public static bool TryNormalize(
        RawPublicDtos.GetHealthResponse wire,
        out BitflyerHealthNormalized? normalized,
        out CallError? error)
    {
        try
        {
            normalized = Build(wire);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer health response invalid.", ex);
            return false;
        }
    }

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;

    private static BitflyerHealthNormalized Build(RawPublicDtos.GetHealthResponse wire) =>
        new(ParseOptional(wire.Status));
}
