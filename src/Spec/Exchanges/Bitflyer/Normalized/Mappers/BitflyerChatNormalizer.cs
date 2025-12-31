using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerChatNormalizer
{
    public static BitflyerChatNormalized Normalize(Chat wire) =>
        new(
            Nickname: wire.Nickname,
            Message: wire.Message,
            Timestamp: wire.Date);
}
