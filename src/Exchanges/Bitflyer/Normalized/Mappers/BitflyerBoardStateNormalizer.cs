using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerBoardStateNormalizer
{
    public static BitflyerBoardStateNormalized Normalize(BoardStateResponse wire) =>
        new(
            Health: wire.Health,
            State: wire.State,
            Data: wire.Data);
}
