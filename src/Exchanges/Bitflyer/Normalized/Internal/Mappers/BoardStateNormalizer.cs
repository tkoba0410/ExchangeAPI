using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BoardStateNormalizer
{
    public static bool TryNormalize(
        RawPublicDtos.GetBoardStateResponse wire,
        out BoardStateNormalized? normalized,
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
            error = new CallError(CallErrorKind.Mapping, "bitFlyer board state response invalid.", ex);
            return false;
        }
    }

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;

    private static BoardStateNormalized Build(RawPublicDtos.GetBoardStateResponse wire) =>
        new(
            Health: ParseOptional(wire.Health),
            State: ParseOptional(wire.State),
            Data: ParseOptional(wire.Data));
}
