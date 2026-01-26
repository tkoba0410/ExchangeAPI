using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using RawPublicModels = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerChatNormalizer
{
    public static BitflyerChatNormalized Normalize(RawPublicModels.Chat wire) =>
        new(
            Nickname: wire.Nickname,
            Message: wire.Message,
            Timestamp: wire.Date);
}
