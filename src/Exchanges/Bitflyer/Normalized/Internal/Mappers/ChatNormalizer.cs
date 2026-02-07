using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class ChatNormalizer
{
    public static bool TryNormalize(
        RawPublicDtos.Chat wire,
        out ChatNormalized? normalized,
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
            error = new CallError(CallErrorKind.Mapping, "bitFlyer chat response invalid.", ex);
            return false;
        }
    }

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;

    private static ChatNormalized Build(RawPublicDtos.Chat wire) =>
        new(
            Nickname: ParseOptional(wire.Nickname),
            Message: ParseOptional(wire.Message),
            Timestamp: wire.Date);
}
