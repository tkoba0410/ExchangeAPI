using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerChatNormalizer
{
    public static BitflyerChatNormalized Normalize(RawPublicDtos.Chat wire) =>
        new(
            Nickname: wire.Nickname,
            Message: wire.Message,
            Timestamp: wire.Date);
}
