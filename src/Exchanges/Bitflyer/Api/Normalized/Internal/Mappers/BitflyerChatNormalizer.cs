using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerChatNormalizer
{
    public static BitflyerChatNormalized Normalize(RawPublicDtos.Chat wire) =>
        new(
            Nickname: ParseOptional(wire.Nickname),
            Message: ParseOptional(wire.Message),
            Timestamp: wire.Date);

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;
}
